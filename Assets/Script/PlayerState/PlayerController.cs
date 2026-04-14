using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{

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

    //无敌时间
    private bool isInvincible = false;
    public float invincibleTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        attackEffectUp = Resources.Load<GameObject>("Eff/Attack_Up");
        attackEffectDown = Resources.Load<GameObject>("Eff/Attack_Down");
        attackEffect = Resources.Load<GameObject>("Eff/Attack");
        dashEffect= Resources.Load<GameObject>("Eff/DashEff");

        animator = this.GetComponent<Animator>();
        if(animator == null)
        {
            Debug.Log("没有找到Animator组件");
        }
        rb=this.GetComponent<Rigidbody2D>();
        if(rb == null)
        {
            Debug.Log("没有找到Rigidbody2D组件");
        }
        //初始为待机动画
        ChangeState(new IdleState());
    }

    // Update is called once per frame
    void Update()
    {
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
            hit.GetComponent<MonsterController>()?.BeAttacked(this);
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
            Debug.Log("下劈命中: " + hit.name);
            hit.GetComponent<MonsterController>()?.BeAttacked(this);
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
            hit.GetComponent<MonsterController>()?.BeAttacked(this);
        }
    }
    /// <summary>
    /// 被攻击
    /// </summary>
    public void BeAttack(Transform monster)
    {
        if (isInvincible) return;
        GameDateMana.Instance.currentPlayer.TakeDamage();
      
        ChangeState(new HitState());
        BeRetreat(monster);
    }
    //被击退
    public void BeRetreat(Transform monster)
    {
        float dir = Mathf.Sign(transform.position.x - monster.position.x);
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


}
