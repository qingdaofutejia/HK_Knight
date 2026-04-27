using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    //人物属性
    public int maxHp;
    public int currentHp;

    public float playerSpeed;
    public float playerJumpHeight;
    public float playerAttack;
    public float playerRange;

    //玩家存档位置
    public float posx;
    public float posy;
    public float posz;

    [JsonIgnore]
    //最大血量变化事件
    public Action<int,int> OnMaxHpChanged;
    [JsonIgnore]
    //血量变化事件
    public Action<int,int> OnHpChanged;
    [JsonIgnore]
    //死亡事件
    public Action OnDeath;

    public Player()
    {
        maxHp = 5;
        currentHp = 5;
        playerSpeed = 4f;
        playerJumpHeight = 6f;
        playerAttack = 20f;
        playerRange = 2f;
        posx = 0f;
        posy = 0f;
        posz = 0f;

    }

    // 增加最大血量
    public void AddMaxHp()
    {
        maxHp += 1;
        currentHp = maxHp;

        OnMaxHpChanged?.Invoke(maxHp, currentHp);
    }
    //扣血
    public void TakeDamage()
    {   
        currentHp -= 1;
        if (currentHp <=0)
        {
            OnDeath?.Invoke();
        }
            

        OnHpChanged?.Invoke(maxHp,currentHp);
    }
    //存档
    public void SavePos(Transform transform)
    {
        posx = transform.position.x;
        posy = transform.position.y;
        posz = transform.position.z;
    }
}

