using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerStateBase
{
    public override void Enter(PlayerController player)
    {
       player.animator.Play("Idle");
    }

    public override void Exit(PlayerController player)
    {
       
    }

    public override void Update(PlayerController player)
    {
        //获取水平移动输入
        float h = Input.GetAxis("Horizontal");

        if (Mathf.Abs(h) > 0.1f)
        {
            player.ChangeState(new RunState());
        }

        if (Input.GetKeyDown(KeyCode.K) && player.IsGrounded())
        {
            player.ChangeState(new JumpState());
        }
        //攻击
        if(Input.GetKeyDown(KeyCode.J) && !player.isAttack)
        {
            player.ChangeState(new AttackState());
        }
    }
}
