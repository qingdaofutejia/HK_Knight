using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
            return;

        Vector3 spawnPos = Vector3.zero;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int index = 0;

            if (PhotonNetwork.InRoom)
            {
                index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
            }

            spawnPos = spawnPoints[index].position;
        }

        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}
