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

    //最大血量变化事件
    public Action<int,int> OnMaxHpChanged;
    //血量变化事件
    public Action<int,int> OnHpChanged;

    public Player()
    {
        maxHp = 5;
        currentHp = 5;
        playerSpeed = 4f;
        playerJumpHeight = 6f;
        playerAttack = 20f;
        playerRange = 2f;
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
        Debug.Log("掉血对象：" + this);
        currentHp -= 1;
        if (currentHp < 0)
            currentHp = 0;

        OnHpChanged?.Invoke(maxHp,currentHp);
    }
}

