using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BossController : MonoBehaviourPunCallbacks, IPunObservable
{
    //组件
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sr;

    //boss数值
    public Boss boss = new Boss();

    //玩家层
    public LayerMask playerLayer;

    //地波预制体
    public GameObject groundWavePrefab;

    //动画名称和触发器名称
    private string idleAnimName = "Boss_Idle";
    private string attack1TriggerName = "Attack1";
    private string attack2TriggerName = "Attack2";

    //当前攻击目标
    public Transform currentTarget;
    public int currentAttackType;
    public int currentComboCount;
    public float cannotAttackTimer;

    //当前状态
    private BossStateBase currentState;

    //是否攻击
    private bool isJumpAttacking;

    //碰到的玩家
    private readonly HashSet<Transform> damagedPlayersThisJump = new HashSet<Transform>();

    private const float JumpAttackDuration = 1f;
    private const float GroundWaveReleaseDelay = 0.5f;
    private const float GroundWaveAttackDuration = 1f;

    private Vector3 networkPosition;
    private Vector3 networkScale;

    private bool HasAuthority
    {
        get
        {
            return !PhotonNetwork.IsConnected ||
                   PhotonNetwork.OfflineMode ||
                   PhotonNetwork.IsMasterClient;
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        playerLayer = LayerMask.GetMask("Player");

        groundWavePrefab = Resources.Load<GameObject>("Prefabs/GroundWave");

        networkPosition = transform.position;
        networkScale = transform.localScale;

        if (HasAuthority)
        {
            ChangeState(new BossIdle());
        }
        else
        {
            PlayIdleLocal();
        }
    }

    private void Update()
    {
        if (HasAuthority)
        {
            UpdateAttackCooldown();
            currentState?.Update(this);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.localScale = networkScale;
        }
    }

    private void UpdateAttackCooldown()
    {
        if (cannotAttackTimer <= 0f)
            return;

        cannotAttackTimer -= Time.deltaTime;

        if (cannotAttackTimer <= 0f)
        {
            cannotAttackTimer = 0f;
            currentComboCount = 0;
        }
    }

    public void ChangeState(BossStateBase newState)
    {
        if (!HasAuthority)
            return;

        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public bool CanAttack()
    {
        return cannotAttackTimer <= 0f && currentComboCount < boss.maxComboCount;
    }

    public void RecordOneAttackFinished()
    {
        currentComboCount++;

        if (currentComboCount >= boss.maxComboCount)
        {
            cannotAttackTimer = boss.comboCooldown;
        }
    }

    public float GetAttackDuration()
    {
        return currentAttackType == 1 ? JumpAttackDuration : GroundWaveAttackDuration;
    }

    public float GetGroundWaveReleaseDelay()
    {
        return GroundWaveReleaseDelay;
    }

    public Transform FindTargetInSearchRange()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(
            transform.position,
            boss.searchRange,
            playerLayer
        );

        if (players == null || players.Length == 0)
            return null;

        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = player.transform;
            }
        }

        return nearestTarget;
    }

    public void FaceToTarget()
    {
        if (currentTarget == null)
            return;

        float dir = currentTarget.position.x - transform.position.x;

        if (Mathf.Abs(dir) < 0.01f)
            return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir);
        transform.localScale = scale;
    }

    public int GetFacingDir()
    {
        return transform.localScale.x >= 0 ? 1 : -1;
    }

    public void PlayIdle()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RPC_PlayIdle), RpcTarget.All);
        }
        else
        {
            PlayIdleLocal();
        }
    }

    private void PlayIdleLocal()
    {
        if (animator == null)
            return;

        animator.Play(idleAnimName);
    }

    public void PlayAttackAnimation(int attackType)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RPC_PlayAttackAnimation), RpcTarget.All, attackType);
        }
        else
        {
            PlayAttackAnimationLocal(attackType);
        }
    }

    private void PlayAttackAnimationLocal(int attackType)
    {
        if (animator == null)
            return;

        animator.ResetTrigger(attack1TriggerName);
        animator.ResetTrigger(attack2TriggerName);

        if (attackType == 1)
        {
            animator.SetTrigger(attack1TriggerName);
        }
        else
        {
            animator.SetTrigger(attack2TriggerName);
        }
    }

    [PunRPC]
    private void RPC_PlayIdle()
    {
        PlayIdleLocal();
    }

    [PunRPC]
    private void RPC_PlayAttackAnimation(int attackType)
    {
        PlayAttackAnimationLocal(attackType);
    }

    public void StopMove()
    {
        if (rb == null)
            return;

        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public void StartJumpAttack()
    {
        if (!HasAuthority)
            return;

        if (rb == null || currentTarget == null)
            return;

        FaceToTarget();

        isJumpAttacking = true;
        damagedPlayersThisJump.Clear();

        float dir = currentTarget.position.x - transform.position.x;

        if (Mathf.Abs(dir) < 0.01f)
        {
            dir = GetFacingDir();
        }
        else
        {
            dir = Mathf.Sign(dir);
        }

        rb.velocity = new Vector2(
            dir * boss.jumpAttackSpeedX,
            boss.jumpAttackSpeedY
        );
    }

    public void EndJumpAttack()
    {
        isJumpAttacking = false;
        damagedPlayersThisJump.Clear();
        StopMove();
    }

    public void ReleaseGroundWave()
    {
        if (!HasAuthority)
            return;

        int dir = GetFacingDir();

        Vector3 spawnPos = transform.position + new Vector3(dir * 1.0f, -1.0f, 0f);

        if (PhotonNetwork.IsConnected)
        {
            GameObject waveObj = PhotonNetwork.Instantiate(
                "GroundWave",
                spawnPos,
                Quaternion.identity
            );

            GroundWave wave = waveObj.GetComponent<GroundWave>();

            if (wave != null)
            {
                PhotonView waveView = waveObj.GetComponent<PhotonView>();
                waveView.RPC(nameof(GroundWave.RPC_Init), RpcTarget.AllBuffered, dir, boss.monster_Speed, 1, playerLayer.value);
            }
        }
        else
        {
            if (groundWavePrefab == null)
            {
                Debug.LogWarning("没有设置 groundWavePrefab");
                return;
            }

            GameObject waveObj = Instantiate(
                groundWavePrefab,
                spawnPos,
                Quaternion.identity
            );

            GroundWave wave = waveObj.GetComponent<GroundWave>();

            if (wave != null)
            {
                wave.Init(dir, boss.monster_Speed, 1, playerLayer);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!HasAuthority)
            return;


        if (!IsInLayerMask(other.gameObject.layer, playerLayer))
            return;

        if (damagedPlayersThisJump.Contains(other.transform))
            return;

        damagedPlayersThisJump.Add(other.transform);

        DamagePlayer(other.gameObject, 1);
    }

    public void DamagePlayer(GameObject playerObj, int damage)
    {
        if (playerObj == null)
            return;

        PlayerController playerController = playerObj.GetComponent<PlayerController>();

        if (playerController == null)
            return;

        PhotonView playerView = playerObj.GetComponent<PhotonView>();

        // 联机模式,只通知被打玩家的拥有者执行受击逻辑
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode && playerView != null)
        {
            playerView.RPC(
                nameof(PlayerController.RPC_BeAttack),
                playerView.Owner,
                transform.position.x
            );
        }
        //直接调用
        else
        {
            playerController.BeAttackByPosition(transform.position);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    public void TakeDamage(float damage)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.MasterClient, damage);
        }
        else
        {
            TakeDamageInternal(damage);
        }
    }

    public void NetworkDestroySelf()
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

    private void OnDrawGizmosSelected()
    {
        if (boss == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, boss.searchRange);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(boss.monster_HP);
            stream.SendNext(currentAttackType);
            stream.SendNext(currentComboCount);
            stream.SendNext(cannotAttackTimer);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkScale = (Vector3)stream.ReceiveNext();
            boss.monster_HP = (float)stream.ReceiveNext();
            currentAttackType = (int)stream.ReceiveNext();
            currentComboCount = (int)stream.ReceiveNext();
            cannotAttackTimer = (float)stream.ReceiveNext();
        }
    }
    public void BeAttacked(PlayerController player)
    {
        if (player == null)
            return;

        float damage = GameDateMana.Instance.currentPlayer.playerAttack;

        // 联机：玩家客户端通知 MasterClient 扣 Boss 血
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.MasterClient, damage);
        }
        //直接扣血
        else
        {
            TakeDamageInternal(damage);
        }
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage)
    {
        if (!HasAuthority)
            return;

        TakeDamageInternal(damage);
    }

    private void TakeDamageInternal(float damage)
    {
        boss.monster_HP -= damage;

        StartCoroutine(HitFlash());

        Debug.Log("Boss HP = " + boss.monster_HP);

        if (boss.monster_HP <= 0f)
        {
            boss.monster_HP = 0f;
            ChangeState(new BossDeath());
        }
    }

    //受击特效
    IEnumerator HitFlash()
    {
        sr.color = Color.red;

        yield return new WaitForSeconds(0.3f);

        sr.color = Color.white;
    }
}