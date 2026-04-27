using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeathState : MonsterStateBase
{
    public override void Enter(MonsterController monster)
    {
        monster.NetSetMonsterBool("Isdeath", true);
    }

    public override void Exit(MonsterController monster)
    {
        
    }

    public override void Update(MonsterController monster)
    {
       
    }
}
