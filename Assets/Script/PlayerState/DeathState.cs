using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : PlayerStateBase
{
    float timer;
    public override void Enter(PlayerController player)
    {
        timer = 0;
        player.NetSetTrigger("Death");
    }

    public override void Exit(PlayerController player)
    {
        
    }

    public override void Update(PlayerController player)
    {
        timer+=Time.deltaTime;
        if(timer>=1.2f)
        {
            player.ChangeState(new IdleState());
        }
    }
}
