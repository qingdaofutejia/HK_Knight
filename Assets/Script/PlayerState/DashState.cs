using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerStateBase
{

    public float dashDuration = 0.3f;
    private float dashSpeed = 10f;
    private float timer;
    private Vector2 dashDir;

    public override void Enter(PlayerController player)
    {
        timer = 0;
        player.NetSetTrigger("Dash");
        dashDir = player.direction;
        dashDir.y = 0f;
    }

    public override void Exit(PlayerController player)
    {
        //清除冲刺残留速度
        player.rb.velocity = new Vector2(0f, player.rb.velocity.y);
    }

    public override void Update(PlayerController player)
    {
        timer += Time.deltaTime;

        player.rb.velocity = new Vector2(dashDir.x * dashSpeed, 0f);

        // 冲刺结束
        if (timer >= dashDuration)
        {
            player.ChangeState(new IdleState());
        }
    }
}
