using UnityEngine;

public class BossAttack : BossStateBase
{
    private float timer;
    private bool groundWaveReleased;

    public override void Enter(BossController boss)
    {
        timer = 0f;
        groundWaveReleased = false;

        boss.StopMove();

        if (boss.currentTarget == null)
        {
            boss.ChangeState(new BossIdle());
            return;
        }

        boss.FaceToTarget();

        if (Random.value < boss.boss.attack1Probability)
        {
            boss.currentAttackType = 1; // 20%，跳向玩家
        }
        else
        {
            boss.currentAttackType = 2; // 80%，释放地波
        }

        boss.PlayAttackAnimation(boss.currentAttackType);

        if (boss.currentAttackType == 1)
        {
            boss.StartJumpAttack();
        }
    }

    public override void Exit(BossController boss)
    {
        if (boss.currentAttackType == 1)
        {
            boss.EndJumpAttack();
        }
    }

    public override void Update(BossController boss)
    {
        timer += Time.deltaTime;

        if (boss.currentTarget != null)
        {
            boss.FaceToTarget();
        }

        if (boss.currentAttackType == 2)
        {
            if (!groundWaveReleased && timer >= boss.GetGroundWaveReleaseDelay())
            {
                groundWaveReleased = true;
                boss.ReleaseGroundWave();
            }
        }

        if (timer >= boss.GetAttackDuration())
        {
            boss.RecordOneAttackFinished();
            boss.ChangeState(new BossIdle());
        }
    }
}