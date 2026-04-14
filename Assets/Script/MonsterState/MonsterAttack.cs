using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    //Åö×²¼ì²â£¬¹¥»÷Íæ¼Ò
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.BeAttack(transform);
        }
    }   
}
