using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class FileSaveAndLoad : MonoBehaviour
{
    public static FileSaveAndLoad instance;

    public static FileSaveAndLoad Instance { get { return instance; } }

    //文件夹目录和保存文件目录
    string folderPath;
    string filePath;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitPath()
    {
        //初始化文件路径
        folderPath = Path.Combine(Application.dataPath, "SaveData");
        filePath = Path.Combine(folderPath, "player.json");

        //如果没有SaveDate文件夹，创建路径
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("创建SaveDate文件夹");
        }
        //第一次进入游戏，创建存档
        if(!File.Exists(filePath))
        {
            Player player = new Player();
            string json = JsonConvert.SerializeObject(player);

            File.WriteAllText(filePath, json);
            Debug.Log("创建存档");
        }
        else
        {
            Debug.Log("存档已存在");
        }
    }

    //读取存档
    public Player LoadPlayer()
    {
        return JsonConvert.DeserializeObject<Player>(File.ReadAllText(filePath));
    }

    //存档
    public void SavePlayer(Player player)
    {
        string json = JsonConvert.SerializeObject(player);
        File.WriteAllText(filePath, json);
    }

    //删除存档
    public void DeletePlayer()
    {

    }
}
