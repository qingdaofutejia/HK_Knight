using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviourPun
{
    //怪物受击动画
    GameObject hitEffect;


    //动画控制器
    public Animator animator;

    //碰撞体
    public Rigidbody2D rb;

    private SpriteRenderer sr;

    //当前状态
    private MonsterStateBase currentState;

    Monster monster=new Monster();


    //怪物是否死亡
    public bool IsDeath = false;


    public float moveDistance = 3f; // 左右巡逻范围
    private Vector3 startPos;
    private int direction = 1; // 1=右，-1=左


    // Start is called before the first frame update
    void Start()
    {
        hitEffect = Resources.Load<GameObject>("Eff/MonsterHitEff");
        startPos = transform.position;
        animator = this.GetComponent<Animator>();
        sr= this.GetComponent<SpriteRenderer>();
        if (animator == null)
        {
            Debug.Log("没有找到Animator组件");
        }
        rb = this.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log("没有找到Rigidbody2D组件");
        }
        //初始为移动动画
        ChangeState(new MonsterMoveState());
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        currentState?.Update(this);
    }
    public void ChangeState(MonsterStateBase newState)
    {
        //退出当前状态
        currentState?.Exit(this);
        //设置新状态
        currentState = newState;
        //进入新状态
        currentState.Enter(this);
    }
    //移动代码
    public void Move()
    {
        // 移动
        transform.Translate(Vector2.right * direction * monster.monster_Speed * Time.deltaTime);

        // 判断是否超出范围
        float offset = transform.position.x - startPos.x;

        if (offset >= moveDistance && direction == 1)
        {
            direction = -1; // 向左
            Flip();
        }
        else if (offset <= -moveDistance && direction == -1)
        {
            direction = 1; // 向右
            Flip();
        }
    }
    // 翻转朝向
    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //被攻击
    public void BeAttacked(PlayerController player)
    {
        if (player == null) return;

        float damage = GameDateMana.Instance.currentPlayer.playerAttack;
        Vector2 hitDir = player.direction;

        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.MasterClient, damage, hitDir.x, hitDir.y);
    }
    [PunRPC]
    void RPC_TakeDamage(float damage, float dirX, float dirY)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (IsDeath)
            return;

        monster.monster_HP -= damage;

        photonView.RPC(nameof(RPC_PlayHitEffect), RpcTarget.All);
        photonView.RPC(nameof(RPC_BeRetreat), RpcTarget.All, dirX, dirY);

        if (monster.monster_HP <= 0)
        {
            IsDeath = true;
            photonView.RPC(nameof(RPC_Die), RpcTarget.All);
        }
    }
    public void TakeDamage(float damage)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RPC_TakeSkillDamage), RpcTarget.MasterClient, damage);
        }
        else
        {
            TakeSkillDamageInternal(damage);
        }
    }

    [PunRPC]
    private void RPC_TakeSkillDamage(float damage)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        TakeSkillDamageInternal(damage);
    }

    private void TakeSkillDamageInternal(float damage)
    {
        if (IsDeath)
            return;

        monster.monster_HP -= damage;

        photonView.RPC(nameof(RPC_PlayHitEffect), RpcTarget.All);

        if (monster.monster_HP <= 0)
        {
            IsDeath = true;
            photonView.RPC(nameof(RPC_Die), RpcTarget.All);
        }
    }
    [PunRPC]
    void RPC_PlayHitEffect()
    {
        if (hitEffect != null)
        {
            GameObject eff = Instantiate(hitEffect, transform.position, Quaternion.identity);
            StartCoroutine(DestroyEff(0.2f, eff));
        }

        StartCoroutine(HitFlash());
    }

    [PunRPC]
    void RPC_BeRetreat(float dirX, float dirY)
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(dirX * 5f, 2f);
        }
    }

    [PunRPC]
    void RPC_Die()
    {
        if (IsDeath == false)
            IsDeath = true;

        ChangeState(new MonsterDeathState());
        StartCoroutine(DestroyObject(0.5f));
    }

    //被击退
    public void BeRetreat(Vector2 vec)
    {
        rb.velocity = new Vector2(vec.x * 5f, 2f);
    }
    IEnumerator DestroyObject(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(this.gameObject);
    }
    IEnumerator DestroyEff(float timer,GameObject eff)
    {
        yield return new WaitForSeconds(timer);
        Destroy(eff);
    }
    /// <summary>
    ///受击反馈
    /// </summary>
    /// <returns></returns>
    IEnumerator HitFlash()
    {
        sr.color = Color.red;

        yield return new WaitForSeconds(0.3f);

        sr.color = Color.white;
    }

    [PunRPC]
    void RPC_SetMonsterBool(string paramName, bool value)
    {
        if (animator != null)
            animator.SetBool(paramName, value);
    }

    public void NetSetMonsterBool(string paramName, bool value)
    {
        if (animator != null)
            animator.SetBool(paramName, value);

        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_SetMonsterBool), RpcTarget.Others, paramName, value);
    }
}
