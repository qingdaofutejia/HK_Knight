using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUIList : MonoBehaviourPunCallbacks
{
    public Transform content;       
    public GameObject roomPrefab;   
    // Start is called before the first frame update
    void Start()
    {
        content = transform;
        roomPrefab = Resources.Load<GameObject>("Prefabs/RoomItemPrefab");
    }
    public override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("RoomUIList OnEnable, InLobby = " + PhotonNetwork.InLobby);

        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible) continue;

            GameObject obj = Instantiate(roomPrefab, content);
            obj.GetComponent<RoomItem>().Init(room);
        }
    }
}
