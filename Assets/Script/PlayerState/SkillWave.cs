using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class SkillWave : MonoBehaviourPun
{
    private SpriteRenderer spriteRenderer;
    // Lua 下载 AB 包后，会把新 Sprite 设置到这里
    public static Sprite HotUpdateSprite;

    private Rigidbody2D rb;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 50f;
    [SerializeField] private float lifeTime = 3f;

    private int direction = 1;

    private readonly HashSet<Transform> damagedTargets = new HashSet<Transform>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError("PlayerSkillWave 缺少 Rigidbody2D");

        if (spriteRenderer == null)
            Debug.LogError("PlayerSkillWave 缺少 SpriteRenderer");
    }

    public void Init(int dir, LayerMask targetLayer)
    {
        direction = dir >= 0 ? 1 : -1;

        ApplyDirection();
        ApplyHotUpdateSprite();

        Destroy(gameObject, lifeTime);
    }

    [PunRPC]
    public void RPC_Init(int dir, int targetLayerMaskValue)
    {
        direction = dir >= 0 ? 1 : -1;

        ApplyDirection();
        ApplyHotUpdateSprite();

        if (photonView.IsMine)
        {
            Invoke(nameof(DestroyNetworkObject), lifeTime);
        }
    }
    private void ApplyHotUpdateSprite()
    {
        if (HotUpdateSprite == null)
            return;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = HotUpdateSprite;
    }
    private void Update()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(direction * speed, 0f);
        }
        else
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("技能波碰到：" + other.name + "，Layer = " + LayerMask.LayerToName(other.gameObject.layer));

        // 联机时只让 MasterClient 判定伤害，避免重复扣血
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            return;

        BossController boss = other.GetComponentInParent<BossController>();
        if (boss != null)
        {
            Transform targetRoot = boss.transform;

            if (damagedTargets.Contains(targetRoot))
                return;

            damagedTargets.Add(targetRoot);

            Debug.Log("技能波命中 Boss，造成伤害：" + damage);
            boss.TakeDamage(damage);
            return;
        }

        MonsterController monster = other.GetComponentInParent<MonsterController>();
        if (monster != null)
        {
            Transform targetRoot = monster.transform;

            if (damagedTargets.Contains(targetRoot))
                return;

            damagedTargets.Add(targetRoot);

            Debug.Log("技能波命中普通怪，造成伤害：" + damage);
            monster.TakeDamage(damage);
            return;
        }
    }

    private void ApplyDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void DestroyNetworkObject()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}