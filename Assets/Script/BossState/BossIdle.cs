using UnityEngine;

public class BossIdle : BossStateBase
{
    public override void Enter(BossController boss)
    {
        boss.StopMove();
        boss.PlayIdle();
    }

    public override void Exit(BossController boss)
    {
    }

    public override void Update(BossController boss)
    {
        Transform target = boss.FindTargetInSearchRange();

        if (target == null)
        {
            boss.currentTarget = null;
            return;
        }

        boss.currentTarget = target;
        boss.FaceToTarget();

        if (boss.CanAttack())
        {
            boss.ChangeState(new BossAttack());
        }
    }
}