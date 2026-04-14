using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archive : MonoBehaviour
{

    public int slotIndex;
    Transform BG;
    Transform Avatar;
    Transform Timer;
    Transform text;

    Button clearButton;

    GameObject HpPrefab;

    // Start is called before the first frame update
    void Start()
    {
        BG = transform.Find("BG");
        Avatar = transform.Find("Avatar");
        Timer = transform.Find("Timer");
        text = transform.Find("Text");
        clearButton = transform.Find("Clear").GetComponent<Button>();
        HpPrefab = Resources.Load<GameObject>("Prefabs/Hp");

        clearButton.onClick.AddListener(OnClickClear);
        Refresh();
    }

    private void OnClickClear()
    {
        //删除当前存档
        FileSaveAndLoad.Instance.DeletePlayer(slotIndex);
    }

    public void Refresh()
    {      
        if (FileSaveAndLoad.Instance.HasSave(slotIndex))
        {
            BG.gameObject.SetActive(true);
            Avatar.gameObject.SetActive(true);
            Timer.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
            //当前存档数据
            Player player = FileSaveAndLoad.Instance.LoadPlayer(slotIndex);
            if (player == null)
            {
                Debug.LogError("读取存档失败 slot=" + slotIndex);
                return;
            }
            //清空旧血条
            foreach (Transform child in Avatar)
            {
                Destroy(child.gameObject);
            }
            //生成血条
            for (int i = 0; i < player.maxHp; i++)
            {
                GameObject hp = Instantiate(HpPrefab, Avatar);
            }
        }
        else
        {
            text.gameObject.SetActive(true);
            BG.gameObject.SetActive(false);
            Avatar.gameObject.SetActive(false);
            Timer.gameObject.SetActive(false);
        }
    }
}
