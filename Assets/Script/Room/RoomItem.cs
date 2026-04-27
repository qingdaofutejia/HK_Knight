using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text roomNameText;
    public Text playerCountText;

    private string roomName;

    private void Awake()
    {
        roomNameText=transform.Find("RoomNameText").GetComponent<Text>();
        if(roomNameText==null)
        {
            Debug.Log("RoomNameText not found");
        }
        playerCountText=transform.Find("PlayerCountText").GetComponent<Text>();
        if(playerCountText==null)
        {
            Debug.Log("PlayerCountText not found");
        }
    }

    public void Init(RoomInfo info)
    {
        roomName = info.Name;

        // 房间名
        roomNameText.text = info.Name;

        playerCountText.text = info.PlayerCount + " / " + info.MaxPlayers;

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClickJoin);
    }
    public void OnClickJoin()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
