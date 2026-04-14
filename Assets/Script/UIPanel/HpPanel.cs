using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpPanel : MonoBehaviour
{
    //血条
    GameObject HpPrefabs;

    Transform hpTransform;

    public List<Image> hpList = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {
        //添加刷新事件
        GameDateMana.Instance.currentPlayer.OnHpChanged += RefreshHp;
        GameDateMana.Instance.currentPlayer.OnMaxHpChanged += RefreshAll;
        HpPrefabs =Resources.Load<GameObject>("Prefabs/Hp");
        hpTransform = transform.Find("Hp");
        InitHp(GameDateMana.Instance.currentPlayer.maxHp, GameDateMana.Instance.currentPlayer.currentHp);
    }
    
    //初始化血量
    public void InitHp(int  maxHp,int currentHp)
    {

        // 清空旧的
        foreach (Transform child in hpTransform)
        {
            Destroy(child.gameObject);
        }

        hpList.Clear();

        for (int i = 0; i < maxHp; i++)
        {
            GameObject hp = Instantiate(HpPrefabs, hpTransform);
            hpList.Add(hp.GetComponent<Image>());
        }
    }

    //刷新血量
    public void RefreshHp(int maxHp, int currentHp) 
    {
        for(int i=0;i<hpList.Count;i++)
        {
            //正常血量
            if(i<currentHp)
            {
                hpList[i].color= Color.white;
            }
            //掉的血量
            else
            {
                hpList[i].color = Color.black;
             }
        }
    }
    public void RefreshAll(int maxHp, int currentHp)
    {

        // 重新生成血条
        InitHp(maxHp, currentHp);

        // 刷新显示
        RefreshHp(maxHp, currentHp);
    }
}
