using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss 
{
    public float monster_HP = 500f;
    public float monster_Speed = 2f;

    //Ë÷µÐ
    public float searchRange = 8f;

    [Range(0f, 1f)]
    public float attack1Probability = 0.2f; // ¹¥»÷1£º20%£¬¹¥»÷2£º80%

    
    public int maxComboCount = 3;        // Á¬Ðø¹¥»÷3´Î
    public float comboCooldown = 3f;     // È»ºó3Ãë²»ÄÜ¹¥»÷

    [Header("¹¥»÷1£ºÌøÏòÍæ¼Ò")]
    public float jumpAttackSpeedX = 7f;
    public float jumpAttackSpeedY = 10f;
}
    