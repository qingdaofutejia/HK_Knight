using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //人物属性
    Player player = new Player();


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


    // Start is called before the first frame update
    void Start()
    {
        attackEffectUp = Resources.Load<GameObject>("Attack_Eff/Attack_Up");
        attackEffectDown = Resources.Load<GameObject>("Attack_Eff/Attack_Down");
        attackEffect = Resources.Load<GameObject>("Attack_Eff/Attack");

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

    //移动
    public void Move(float dir)
    {
        rb.velocity = new Vector2(dir * player.playerSpeed, rb.velocity.y);
    }
    //跳跃
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, player.playerJumpHeight);
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

    //攻击特效帧事件
    //向上攻击
    public void AttackUp()
    {
        GameObject effect = Instantiate(attackEffectUp, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        StartCoroutine(DestroyAttackEffect(effect, 0.15f));
    }
    //向下攻击
    public void AttackDown()
    {
        GameObject effect = Instantiate(attackEffectDown, transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity);
        StartCoroutine(DestroyAttackEffect(effect, 0.15f));
    }
    //普通攻击
    public void Attack()
    {
        GameObject effect = Instantiate(attackEffect);
        //朝右
        if (transform.localScale.x < 0)
        {
            effect.transform.localScale=new Vector3(-1,1,1);
            effect.transform.position=transform.position+new Vector3(1.5f,0f,0);
        }
        //朝左
        else
        {
            effect.transform.position = transform.position + new Vector3(-1.5f, 0f, 0);
        }
        StartCoroutine(DestroyAttackEffect(effect, 0.15f));
    }
    //销毁特效
    IEnumerator DestroyAttackEffect(GameObject effect,float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(effect);
    }


}
