using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomList : MonoBehaviourPunCallbacks
{
    public Transform playerListParent;
    public GameObject playerItemPrefab;
    public Button startBtn;

    private Dictionary<int, GameObject> playerItems = new Dictionary<int, GameObject>();

    void Start()
    {
        playerListParent = transform.Find("List");
        playerItemPrefab = Resources.Load<GameObject>("Prefabs/PlayerItem");
        startBtn = transform.Find("start").GetComponent<Button>();

        if (startBtn != null)
        {
            startBtn.onClick.RemoveAllListeners();
            startBtn.onClick.AddListener(OnClickStartGame);
        }

        RefreshStartButtonState();
    }

    public void InitRoomUI()
    {
        RefreshAllPlayers();
        RefreshStartButtonState();
    }

    public void RefreshAllPlayers()
    {
        ClearAllItems();

        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("还没进入房间，不能刷新玩家列表");
            return;
        }

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            AddPlayerItem(player);
        }
    }

    void AddPlayerItem(Photon.Realtime.Player player)
    {
        if (player == null) return;

        if (playerItems.ContainsKey(player.ActorNumber))
            return;

        if (playerListParent == null || playerItemPrefab == null)
        {
            Debug.LogError("playerListParent 或 playerItemPrefab 没有赋值");
            return;
        }

        GameObject item = Instantiate(playerItemPrefab, playerListParent);
        item.name = "Player_" + player.ActorNumber;
        item.SetActive(true);

        int index = playerItems.Count;
        PlayerItem playerItem = item.GetComponent<PlayerItem>();
        if (playerItem != null)
        {
            playerItem.Init(index);
        }

        playerItems.Add(player.ActorNumber, item);
    }

    void RemovePlayerItem(Photon.Realtime.Player player)
    {
        if (player == null) return;

        if (playerItems.TryGetValue(player.ActorNumber, out GameObject item))
        {
            Destroy(item);
            playerItems.Remove(player.ActorNumber);
        }
    }

    void ClearAllItems()
    {
        foreach (var kv in playerItems)
        {
            if (kv.Value != null)
                Destroy(kv.Value);
        }

        playerItems.Clear();
    }

    void RefreshStartButtonState()
    {
        if (startBtn == null) return;

        bool isMaster = PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient;
        startBtn.gameObject.SetActive(isMaster);
        startBtn.interactable = isMaster;
    }

    void OnClickStartGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("还没进入房间，不能开始游戏");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("只有房主可以开始游戏");
            return;
        }

        // 这里先进入加载场景
        Asynchronous.nextScene = 3;
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        AddPlayerItem(newPlayer);
        RefreshStartButtonState();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RemovePlayerItem(otherPlayer);
        RefreshStartButtonState();
    }

    public override void OnLeftRoom()
    {
        ClearAllItems();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        RefreshStartButtonState();
    }
}
