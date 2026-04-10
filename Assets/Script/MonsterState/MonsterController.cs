using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterController : MonoBehaviour
{

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
        monster.monster_HP-=player.player.playerAttack;
        StartCoroutine(HitFlash());
        BeRetreat(player.direction);
        if(monster.monster_HP<=0)
        {
            ChangeState(new MonsterDeathState());
        }
    }
    //被击退
    public void BeRetreat(Vector2 vec)
    {
        rb.velocity = new Vector2(vec.x * 5f, 2f);
    }

    //怪物死亡销毁
    public void Die()
    {
        StartCoroutine(DestroyObject(0.5f));
    }
    IEnumerator DestroyObject(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(this.gameObject);
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
}
