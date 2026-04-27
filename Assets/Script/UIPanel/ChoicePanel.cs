using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    public static ChoicePanel Instance;

    //记录4个存档的数组
    Archive[] slots = new Archive[4];    

    //退出按钮
    Button exitBtn;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;

        transform.GetComponent<CanvasGroup>().alpha = 0;
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.GetComponent<CanvasGroup>().interactable = false;

        for (int i=1;i<=4;i++)
        {
            int index = i;
            slots[i-1] = transform.Find("List/Archive" + i).GetComponent<Archive>();
            slots[i-1].GetComponent<Button>().onClick.AddListener(delegate { OnArchive(index); });
        }
        exitBtn = transform.Find("Return").GetComponent<Button>();
        exitBtn.onClick.AddListener(OnExit);
    }

    private void OnArchive(int slot)
    {
        //如果有存档,读取存档
        if (FileSaveAndLoad.Instance.HasSave(slot))
        {
            GameDateMana.Instance.Load(slot);

            SceneManager.LoadScene(2);
        }
        //如果没有存档，创建存档
        else
        {
            FileSaveAndLoad.Instance.CreatePlayer(slot);

            GameDateMana.Instance.Load(slot);
        }
    }



    //打开界面
    public void OnEnter()
    {
        UIMana.Instance.currentState = UIState.Choice;
        RefreshUI();
        //慢慢显示，并开启点击
        transform.GetComponent<CanvasGroup>().DOFade(1f, 1f)
        .OnComplete(() =>
        {
            transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.GetComponent<CanvasGroup>().interactable = true;
        });
       
    }
    //关闭界面
    public void OnExit()
    {
        //慢慢隐藏，并关闭点击
        transform.GetComponent<CanvasGroup>().DOFade(0f, 1f)
        .OnComplete(() =>
        {
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.GetComponent<CanvasGroup>().interactable = false;
            StartPanel.Instance.OnEnter();
        });
    }


    //刷新UI
    public void RefreshUI()
    {
        if (UIMana.Instance.currentState == UIState.Choice)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Refresh();
            }
        }
    }
    //添加刷新事件
    private void OnEnable()
    {
        FileSaveAndLoad.OnSaveChanged += RefreshUI;
    }

    private void OnDisable()
    {
        FileSaveAndLoad.OnSaveChanged -= RefreshUI;
    }
}
