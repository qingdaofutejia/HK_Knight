using UnityEngine;

public class BossDeath : BossStateBase
{
    public override void Enter(BossController boss)
    {
        boss.StopMove();
        boss.NetworkDestroySelf();
    }

    public override void Exit(BossController boss)
    {
    }

    public override void Update(BossController boss)
    {
    }
}