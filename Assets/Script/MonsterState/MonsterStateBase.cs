using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterStateBase
{
    public abstract void Enter(MonsterController monster);
    public abstract void Update(MonsterController monster);
    public abstract void Exit(MonsterController monster);
}
