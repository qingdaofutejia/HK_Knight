using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GroundWave : MonoBehaviourPun
{
    private Rigidbody2D rb;

    private int direction;
    private float speed;
    private int damage;
    private LayerMask playerLayer;

    private readonly HashSet<Transform> damagedPlayers = new HashSet<Transform>();

    private const float LifeTime = 6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(int dir, float moveSpeed, int waveDamage, LayerMask targetLayer)
    {
        direction = dir >= 0 ? 1 : -1;
        speed = moveSpeed;
        damage = waveDamage;
        playerLayer = targetLayer;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        Destroy(gameObject, LifeTime);
    }

    [PunRPC]
    public void RPC_Init(int dir, float moveSpeed, int waveDamage, int targetLayerMaskValue)
    {
        direction = dir >= 0 ? 1 : -1;
        speed = moveSpeed;
        damage = waveDamage;
        playerLayer = targetLayerMaskValue;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(DestroyNetworkObject), LifeTime);
        }
    }

    private void Update()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        }
        else
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            return;

        if (!IsInLayerMask(other.gameObject.layer, playerLayer))
            return;

        if (damagedPlayers.Contains(other.transform))
            return;

        damagedPlayers.Add(other.transform);

        DamagePlayer(other.gameObject);
    }
    private void DamagePlayer(GameObject playerObj)
    {
        if (playerObj == null)
            return;

        PlayerController playerController = playerObj.GetComponent<PlayerController>();

        if (playerController == null)
            return;

        PhotonView playerView = playerObj.GetComponent<PhotonView>();

        // 联机模式：只让被击中的玩家自己执行受击逻辑
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode && playerView != null)
        {
            playerView.RPC(
                nameof(PlayerController.RPC_BeAttack),
                playerView.Owner,
                transform.position.x
            );
        }
        // 单机 / 离线模式
        else
        {
            playerController.BeAttackByPosition(transform.position);
        }
    }
    private void DestroyNetworkObject()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}