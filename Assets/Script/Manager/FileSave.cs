using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FileSave : MonoBehaviour
{
    PlayerController player;

    private void Start()
    {
        //player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.W))
        {
            //保存当前位置
            GameDateMana.Instance.Save();
            //写入存档
            FileSaveAndLoad.Instance.SavePlayer(GameDateMana.Instance.currentSlot, GameDateMana.Instance.currentPlayer);
            Debug.Log("存档成功");
        }
    }
}
