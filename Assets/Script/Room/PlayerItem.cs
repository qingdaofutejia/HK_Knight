using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    Text playerCount;
    void Awake()
    {
        if (playerCount == null)
            playerCount = transform.Find("PlayerCountText").GetComponent<Text>();
    }

    public void Init(int index)
    {
        playerCount.text="Íæ¼Ò" + (index + 1);
    }
}
