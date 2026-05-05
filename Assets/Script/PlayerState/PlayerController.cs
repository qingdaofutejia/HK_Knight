using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    //人物朝向
    public Vector2 direction = Vector2.left;

    //动画控制器
    public Animator animator;

    //碰撞体
    public Rigidbody2D rb;

    //是否在地上
    private bool isGrounded = false;

    //攻击间隔
    public bool isAttack = false;

    //当前状态
    private PlayerStateBase currentState;

    //攻击特效
    public GameObject attackEffectUp;
    public GameObject attackEffectDown;
    public GameObject attackEffect;
    //冲刺特效
    public GameObject dashEffect;

    //技能特效
    public GameObject skillWavePrefab;
    public float skillWaveSpawnOffsetX = 1.2f;

    //无敌时间
    private bool isInvincible = false;
    public float invincibleTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        animator = this.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.Log("没有找到Animator组件");
        }

        rb = this.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log("没有找到Rigidbody2D组件");
        }

        attackEffectUp = Resources.Load<GameObject>("Eff/Attack_Up");
        attackEffectDown = Resources.Load<GameObject>("Eff/Attack_Down");
        attackEffect = Resources.Load<GameObject>("Eff/Attack");
        dashEffect = Resources.Load<GameObject>("Eff/DashEff");
        skillWavePrefab= Resources.Load<GameObject>("Eff/PlayerSkillWave");

        // 远端玩家：不执行本地存档
        if (pv != null && !pv.IsMine)
        {
            ChangeState(new IdleState());
            return;
        }

        GameDateMana.Instance.playerTransform = this.transform;

        GameDateMana.Instance.currentPlayer.OnDeath += OnPlayerDead;

        // 每次进入游戏时，当前血量重置为最大血量
        GameDateMana.Instance.currentPlayer.currentHp = GameDateMana.Instance.currentPlayer.maxHp;
        GameDateMana.Instance.currentPlayer.OnHpChanged?.Invoke(
            GameDateMana.Instance.currentPlayer.maxHp,
            GameDateMana.Instance.currentPlayer.currentHp
        );


        transform.position = new Vector3(
            GameDateMana.Instance.currentPlayer.posx,
            GameDateMana.Instance.currentPlayer.posy,
            GameDateMana.Instance.currentPlayer.posz
        );
        //初始为待机动画
        ChangeState(new IdleState());
    }

    //玩家死亡事件
    private void OnPlayerDead()
    {
        //切换死亡动画
        ChangeState(new DeathState());
        DeathPanel.Instance.Play(OnBlackFull);
    }

    private void OnBlackFull()
    {
        // 复活位置
        transform.position = new Vector3(GameDateMana.Instance.currentPlayer.posx, GameDateMana.Instance.currentPlayer.posy, GameDateMana.Instance.currentPlayer.posz);
        GameDateMana.Instance.currentPlayer.currentHp = GameDateMana.Instance.currentPlayer.maxHp;
        //血量恢复UI
        GameDateMana.Instance.currentPlayer.OnHpChanged?.Invoke(GameDateMana.Instance.currentPlayer.maxHp, GameDateMana.Instance.currentPlayer.currentHp);
    }

    // Update is called once per frame
    void Update()
    {
        if (pv != null && !pv.IsMine)
            return;
        currentState?.Update(this);
        CheckDirection();
    }
    
    //改变状态
    public void ChangeState(PlayerStateBase newState)
    {
        //退出当前状态
        currentState?.Exit(this);
        //设置新状态
        currentState = newState;
        //进入新状态
        currentState.Enter(this);
    }
    //判断朝向
    private void CheckDirection()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            direction = Vector2.right;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            direction = Vector2.left;
        }
    }


    //移动
    public void Move(float dir)
    {
        rb.velocity = new Vector2(dir * GameDateMana.Instance.currentPlayer.playerSpeed, rb.velocity.y);
    }
    //跳跃
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, GameDateMana.Instance.currentPlayer.playerJumpHeight);
    }
    //是否在地面
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    //攻击
    public void Attack()
    {
       
        float distance = GameDateMana.Instance.currentPlayer.playerRange;

        Vector2 origin = (Vector2)transform.position + direction * distance * 0.5f;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            origin,
            new Vector2(distance, 1f), // 宽 = 攻击距离
            0f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            //普通怪物
            hit.GetComponent<MonsterController>()?.BeAttacked(this);
            //boss
            hit.GetComponent<BossController>()?.BeAttacked(this);
        }
    }
    public void AttackDown()
    {
        float range = GameDateMana.Instance.currentPlayer.playerRange;

        Vector2 center = (Vector2)transform.position + Vector2.down * range * 0.5f;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            new Vector2(1f, range), // 竖着的攻击范围
            0f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            //普通怪物
            hit.GetComponent<MonsterController>()?.BeAttacked(this);
            //boss
            hit.GetComponent<BossController>()?.BeAttacked(this);
        }
    }
    public void AttackUp()
    {
        float range = GameDateMana.Instance.currentPlayer.playerRange;

        Vector2 center = (Vector2)transform.position + Vector2.up * range * 0.5f;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            new Vector2(1f, range), // 竖着的攻击范围
            0f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            Debug.Log("上劈命中: " + hit.name);
            //普通怪物
            hit.GetComponent<MonsterController>()?.BeAttacked(this);
            //boss
            hit.GetComponent<BossController>()?.BeAttacked(this);
        }
    }
    [PunRPC]
    public void RPC_BeAttack(float monsterX)
    {
        Vector3 monsterPos = new Vector3(monsterX, transform.position.y, transform.position.z);
        BeAttackByPosition(monsterPos);
    }

    public void BeAttack(Transform monster)
    {
        if (monster == null) return;
        BeAttackByPosition(monster.position);
    }

    /// <summary>
    /// 按怪物位置处理受击
    /// </summary>
    public void BeAttackByPosition(Vector3 monsterPos)
    {
        if (isInvincible) return;

        GameDateMana.Instance.currentPlayer.TakeDamage();
        if (GameDateMana.Instance.currentPlayer.currentHp <= 0) return;

        ChangeState(new HitState());
        BeRetreatByPosition(monsterPos);
    }

    /// <summary>
    /// 按怪物位置击退
    /// </summary>
    public void BeRetreatByPosition(Vector3 monsterPos)
    {
        float dir = Mathf.Sign(transform.position.x - monsterPos.x);
        rb.velocity = new Vector2(dir * 5f, 2f);

        StartCoroutine(InvincibleCoroutine());
    }
    //无敌时间
    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    //攻击特效帧事件
    //向上攻击
    public void AttackUpEff()
    {
        GameObject effect = Instantiate(attackEffectUp, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        StartCoroutine(DestroyAttackEffect(effect, 0.15f));
    }
    //向下攻击
    public void AttackDownEff()
    {
        GameObject effect = Instantiate(attackEffectDown, transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity);
        StartCoroutine(DestroyAttackEffect(effect, 0.15f));
    }
    //普通攻击
    public void AttackEff()
    {
        GameObject effect = Instantiate(attackEffect);
        //朝右
        if (direction==Vector2.right)
        {
            effect.transform.localScale=new Vector3(-1,1,1);
            effect.transform.position=transform.position+new Vector3(1.5f,0f,0);
        }
        //朝左
        else if (direction == Vector2.left)
        {
            effect.transform.position = transform.position + new Vector3(-1.5f, 0f, 0);
        }
        StartCoroutine(DestroyAttackEffect(effect, 0.15f));
    }
    //冲刺特效
    public void DashEff()
    {
        GameObject effect = Instantiate(dashEffect);
        //朝右
        if (direction == Vector2.right)
        {
            effect.transform.localScale = new Vector3(-1, 1, 1);
        }
        //朝左
        else if (direction == Vector2.left)
        {
            effect.transform.localScale = new Vector3(1, 1, 1);
        }
        effect.transform.SetParent(transform);
        effect.transform.localPosition = Vector3.zero;
        StartCoroutine(DestroyAttackEffect(effect, 0.3f));
    }


    //销毁特效
    IEnumerator DestroyAttackEffect(GameObject effect,float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(effect);
    }

    [PunRPC]
    void RPC_SetAnimTrigger(string triggerName)
    {
        if (animator != null)
            animator.SetTrigger(triggerName);
    }

    [PunRPC]
    void RPC_SetAnimFloat(string paramName, float value)
    {
        if (animator != null)
            animator.SetFloat(paramName, value);
    }

    [PunRPC]
    void RPC_PlayAnim(string stateName)
    {
        if (animator != null)
            animator.Play(stateName);
    }

    public void NetSetTrigger(string triggerName)
    {
        if (animator != null)
            animator.SetTrigger(triggerName);

        if (pv != null && pv.IsMine)
            pv.RPC(nameof(RPC_SetAnimTrigger), RpcTarget.Others, triggerName);
    }

    public void NetSetFloat(string paramName, float value)
    {
        if (animator != null)
            animator.SetFloat(paramName, value);

        if (pv != null && pv.IsMine)
            pv.RPC(nameof(RPC_SetAnimFloat), RpcTarget.Others, paramName, value);
    }

    public void NetPlay(string stateName)
    {
        if (animator != null)
            animator.Play(stateName);

        if (pv != null && pv.IsMine)
            pv.RPC(nameof(RPC_PlayAnim), RpcTarget.Others, stateName);
    }

    //技能
    public void ReleaseSkillWave()
    {
        int dir = direction == Vector2.right ? 1 : -1;

        Vector3 spawnPos = transform.position + new Vector3(
            dir * skillWaveSpawnOffsetX,
            0f,
            0f
        );

        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            GameObject waveObj = PhotonNetwork.Instantiate(
                "Eff/PlayerSkillWave",
                spawnPos,
                Quaternion.identity
            );

            SkillWave wave = waveObj.GetComponent<SkillWave>();
            if (wave != null)
            {
                PhotonView waveView = waveObj.GetComponent<PhotonView>();
                waveView.RPC(
                    nameof(SkillWave.RPC_Init),
                    RpcTarget.AllBuffered,
                    dir,
                    LayerMask.GetMask("Enemy")
                );
            }
        }
        else
        {
            if (skillWavePrefab == null)
            {
                Debug.LogWarning("没有设置 skillWavePrefab");
                return;
            }

            GameObject waveObj = Instantiate(
                skillWavePrefab,
                spawnPos,
                Quaternion.identity
            );

            SkillWave wave = waveObj.GetComponent<SkillWave>();
            if (wave != null)
            {
                wave.Init(dir, LayerMask.GetMask("Enemy"));
            }
        }
    }
}
