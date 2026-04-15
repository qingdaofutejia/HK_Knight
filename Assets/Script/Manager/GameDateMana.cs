using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDateMana
{
    public Transform playerTransform;   

    public static GameDateMana Instance = new GameDateMana();

    public Player player;   // 当前存档人物数据
    public int currentSlot; // 当前存档位

    public Player currentPlayer; // 当前玩家数据

    // 加载存档
    public void Load(int slot)
    {
        currentSlot = slot;

        player = FileSaveAndLoad.Instance.LoadPlayer(slot);

        // 克隆一份用于游戏
        currentPlayer = ClonePlayer(player);
    }

    // 保存游戏
    public void Save()
    {
        // 记录位置
        currentPlayer.posx = playerTransform.position.x;
        currentPlayer.posy = playerTransform.position.y;
        currentPlayer.posz = playerTransform.position.z;

        player = ClonePlayer(currentPlayer);
        FileSaveAndLoad.Instance.SavePlayer(currentSlot, player);
    }

    private Player ClonePlayer(Player p)
    {
        string json = JsonConvert.SerializeObject(p);
        return JsonConvert.DeserializeObject<Player>(json);
    }
}
