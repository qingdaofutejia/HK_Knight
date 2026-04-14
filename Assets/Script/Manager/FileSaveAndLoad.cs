using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class FileSaveAndLoad : MonoBehaviour
{
    private static FileSaveAndLoad instance;
    public static FileSaveAndLoad Instance { get { return instance; } }

    //存档发生变化的事件
    public static event Action OnSaveChanged;

    //文件夹目录
    string folderPath;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            //创建文件夹
            InitPath();
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

        //如果没有SaveDate文件夹，创建路径
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("创建SaveDate文件夹");
        }
    }

    //创建存档
    public void CreatePlayer(int slot)
    {
        string path=GetFilePath(slot);

        Player player = new Player();
        string json = JsonConvert.SerializeObject(player);
        File.WriteAllText(path, json);

        //创建了新的存档，触发事件
        OnSaveChanged?.Invoke();
    }
    //读取存档
    public Player LoadPlayer(int slot)
    {
        string path = GetFilePath(slot);

        if(!File.Exists(path))
        {
            Debug.Log("存档存在");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Player>(json);
    }

    //存档
    public void SavePlayer(int slot,Player player)
    {
        string path = GetFilePath(slot);

        string json = JsonConvert.SerializeObject(player, Formatting.Indented);
        File.WriteAllText(path, json);

        //更新了存档，触发事件
        OnSaveChanged?.Invoke();
    }

    //删除存档
    public void DeletePlayer(int slot)
    {
        string path = GetFilePath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
            //删除了存档，触发事件
            OnSaveChanged?.Invoke();
            Debug.Log($"删除存档 {slot}");
        }
    }

    //多个存档,获取存档id
    string GetFilePath(int slot)
    {
        return Path.Combine(folderPath,$"save_{slot}.json");
    }

    //判断是否有存档
    public bool HasSave(int slot)
    {
        return File.Exists(GetFilePath(slot));
    }
}
