using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : PlayerStateBase
{
    public override void Enter(PlayerController player)
    {
        player.animator.SetFloat("Speed", 1f);
    }

    public override void Exit(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        //移动
        float h = Input.GetAxis("Horizontal");
        player.animator.SetFloat("Speed", Mathf.Abs(h));
        //向右移动，人物转向
        if (h>=0.1f)
        {
            player.transform.localScale = new Vector3(-1f, 1f, 1f);
            player.Move(h);
        }
        else if(h<=-0.1f)
        {
            player.transform.localScale = new Vector3(1f, 1f, 1f);
            player.Move(h);
        }
        if (Mathf.Abs(h) < 0.1f)
        {
            player.ChangeState(new IdleState());
        }

        if (Input.GetKeyDown(KeyCode.K) && player.IsGrounded())
        {
            player.ChangeState(new JumpState());
        }
        //攻击
        if (Input.GetKeyDown(KeyCode.J) && !player.isAttack)
        {
            player.ChangeState(new AttackState());
        }
    }
}
