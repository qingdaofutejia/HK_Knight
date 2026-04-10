using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerStateBase
{

    private bool Isgrounded;
    public override void Enter(PlayerController player)
    {
        player.animator.SetTrigger("Jump");
        player.Jump();
        Isgrounded=false;
    }

    public override void Exit(PlayerController player)
    {
       
    }

    public override void Update(PlayerController player)
    {

        //获取y轴输入
        float h = Input.GetAxis("Horizontal");
        player.Move(h);
        if(!player.IsGrounded())
        {
            Isgrounded = true;
        }
        //攻击
        if (Input.GetKeyDown(KeyCode.J) && !player.isAttack)
        {
            player.ChangeState(new AttackState());
            return;
        }

        // 落地后切回
        if (Isgrounded&&player.IsGrounded())
        {
            player.ChangeState(new IdleState());
        }
    }
}
