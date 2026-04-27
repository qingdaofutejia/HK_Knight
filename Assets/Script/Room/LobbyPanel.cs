using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class LobbyPanel : MonoBehaviourPunCallbacks
{
    GameObject listUI;
    GameObject roomUI;

    Button createBtn;
    Button joinBtn;

    void Start()
    {
        listUI = transform.Find("ListUI").gameObject;
        roomUI = transform.Find("RoomUI").gameObject;
        createBtn = listUI.transform.Find("Create").GetComponent<Button>();
        joinBtn = listUI.transform.Find("Join").GetComponent<Button>();

        createBtn.onClick.AddListener(CreateBtnOnClick);
        joinBtn.onClick.AddListener(JoinBtnOnClick);
        roomUI.gameObject.SetActive(false);
    }

    // 创建联机房间
    private void CreateBtnOnClick()
    {
        if (!PhotonNetwork.IsConnectedAndReady || PhotonNetwork.OfflineMode)
        {
            Debug.LogWarning("当前不在联机状态，无法创建在线房间");
            return;
        }

        string roomName = "Room_" + Random.Range(0, 1000);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
    }

    // Join 按钮：切离线，单机进入游戏
    private void JoinBtnOnClick()
    {
        StartCoroutine(StartOfflineGame());
    }

    IEnumerator StartOfflineGame()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
                yield return null;
        }

        PhotonNetwork.OfflineMode = false;

        // 单机直接进入异步加载场景，不创建房间，不显示房间UI
        Asynchronous.nextScene = 3;
        SceneManager.LoadScene(1);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("成功进入房间");
        listUI.SetActive(false);
        roomUI.SetActive(true);

        RoomList roomList = roomUI.GetComponent<RoomList>();
        if (roomList != null)
        {
            roomList.InitRoomUI();
        }
        else
        {
            Debug.LogError("RoomUI 上没有找到 RoomList 脚本");
        }
    }

    public override void OnCreateRoomFailed(short code, string msg)
    {
        Debug.LogError("创建房间失败: " + msg);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("房间创建成功");
    }

    public override void OnJoinRoomFailed(short code, string msg)
    {
        Debug.LogError("加入失败: " + msg);
    }
}
