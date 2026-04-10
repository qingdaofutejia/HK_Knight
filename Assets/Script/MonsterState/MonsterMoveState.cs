using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveState : MonsterStateBase
{
    public override void Enter(MonsterController monster)
    {
        monster.animator.SetBool("Isdeath", false);
    }

    public override void Exit(MonsterController monster)
    {
        
    }

    public override void Update(MonsterController monster)
    {
        monster.Move();
        //Èç¹û¹ÖÎïËÀÍö
        if (monster.IsDeath)
        {
            monster.ChangeState(new MonsterDeathState());
        }
    }
}
