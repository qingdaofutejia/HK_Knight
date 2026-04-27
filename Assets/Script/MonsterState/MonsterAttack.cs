using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // 从当前碰到的物体或它的父物体里找 PlayerController
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            player = other.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        // 再从玩家根物体上找 PhotonView
        PhotonView targetPv = player.GetComponent<PhotonView>();
        if (targetPv == null)
        {
            Debug.LogWarning("MonsterAttack: 没找到目标玩家的 PhotonView, other = " + other.name);
            return;
        }

        targetPv.RPC("RPC_BeAttack", targetPv.Owner, transform.position.x);
    }
}
