using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerStateBase
{

    float timer;

    public override void Enter(PlayerController player)
    {
        player.isAttack = true;
        timer = 0;

        float v = Input.GetAxis("Vertical");  

        // ÏòÉÏ¹¥»÷
        if (v > 0.5f)
        {
            player.animator.SetTrigger("Attack_Up");
        }
        // ÏòÏÂ¹¥»÷
        else if (v < -0.5f)
        {
            player.animator.SetTrigger("Attack_Down");
        }
        else
        {
            //³¯ÓÒ
            if (player.transform.localScale.x < 0)
            {
                player.animator.SetTrigger("Attack_Right");
            }
            //³¯×ó
            else
            {
                player.animator.SetTrigger("Attack_Left");
            }
        }
    }

    public override void Exit(PlayerController player)
    {
        player.isAttack = false;
    }

    public override void Update(PlayerController player)
    {
        timer += Time.deltaTime;
        if(timer>0.3f)
        {
            player.ChangeState(new IdleState());
        }
    }
}
