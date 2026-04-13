using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    //»ÀŒÔ Ù–‘
    public float playerHp;
    public float playerSpeed;
    public float playerJumpHeight;
    public float playerAttack;
    public float playerRange;

    public Player()
    {
        playerHp = 5;
        playerSpeed = 4f;
        playerJumpHeight = 6f;
        playerAttack = 20f;
        playerRange = 2f;
    }
}
