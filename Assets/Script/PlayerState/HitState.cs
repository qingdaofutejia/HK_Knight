using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : PlayerStateBase
{
    private float timer;
    public override void Enter(PlayerController player)
    {
        player.animator.SetTrigger("Hit");
    }

    public override void Exit(PlayerController player)
    {
        
    }

    public override void Update(PlayerController player)
    {

        timer += Time.deltaTime;
        // 受击持续时间
        if (timer > 0.3f)
        {
            player.ChangeState(new IdleState());
        }
    }
}
