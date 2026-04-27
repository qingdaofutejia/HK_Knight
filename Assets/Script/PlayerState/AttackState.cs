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
            player.NetSetTrigger("Attack_Up");
            // ÉÔÎ¢¼õÂıÏÂÂä
            player.rb.velocity = Vector2.zero;
            player.AttackUp();
        }
        // ÏòÏÂ¹¥»÷
        else if (v < -0.5f)
        {
            player.NetSetTrigger("Attack_Down");
            // ÉÔÎ¢¼õÂıÏÂÂä
            player.rb.velocity = Vector2.zero;
            player.AttackDown();
        }
        else
        {
            //³¯ÓÒ
            if (player.transform.localScale.x < 0)
            {
                player.NetSetTrigger("Attack_Right");
            }
            //³¯×ó
            else
            {
                player.NetSetTrigger("Attack_Left");
            }
            player.Attack();
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
