using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    public static StartPanel Instance;

    //三个背景板
    Transform Bg1;
    Transform Bg2;
    Transform Bg3;

    Transform icon;
    Transform cover;

    Button LoginBtn;
    Button SettingBtn;
    Button ExitBtn;


    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;

        Bg1 = transform.Find("Bg").transform;
        Bg2 = transform.Find("Bg2").transform;
        Bg3 = transform.Find("Bg3").transform;
        icon = Bg1.Find("Icon");
        cover = transform.Find("cover").transform;

        LoginBtn = Bg3.Find("Login").GetComponent<Button>();
        ExitBtn = Bg3.Find("Exit").GetComponent<Button>();
        SettingBtn = Bg3.Find("Setting").GetComponent<Button>();

        LoginBtn.onClick.AddListener(OnclickLoginBtn);
        ExitBtn.onClick.AddListener(OnclickExitBtn);  
        SettingBtn.onClick.AddListener(OnclickSettingBtn);
        //开始设置背景板为透明,遮盖
        cover.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Bg2.GetComponent<CanvasGroup>().alpha = 0;
        Bg3.GetComponent<CanvasGroup>().alpha = 0;
        //背景1下的图标设置为透明
        icon.GetComponent<CanvasGroup>().alpha = 0;

        //动画开局
        StartPanelShow();
    }

    private void OnclickSettingBtn()
    {
        //点击设置,跳转到设置界面
        OnExit(() =>
        {
            SettingPanel.Instance.OnEnter();
        });
    }

    private void OnclickExitBtn()
    {
        //退出游戏
        
    }

    private void OnclickLoginBtn()
    {
        //点击开始游戏,跳转到读档界面
        OnExit(() =>
        {
            ChoicePanel.Instance.OnEnter();
        });
    }

    private void  StartPanelShow()
    {
        //动画效果
        icon.GetComponent<CanvasGroup>().DOFade(1f, 1.5f)
       .OnComplete(() => 
       {
           icon.GetComponent<CanvasGroup>().DOFade(0f, 1.5f)
               .OnComplete(() => 
               {
                   Bg2.GetComponent<CanvasGroup>().DOFade(1f, 1.5f)
                      .OnComplete(() =>
                      {
                          Bg2.GetComponent<CanvasGroup>().DOFade(0f, 1.5f)
                          .OnComplete(() =>
                          {
                              Bg3.GetComponent<CanvasGroup>().DOFade(1f, 1.5f)
                              .OnComplete(() =>
                              {
                                  cover.GetComponent<CanvasGroup>().blocksRaycasts = false;
                                  UIMana.Instance.currentState=UIState.Start;
                              });
                              
                          });
                      });
               });
       });
    }

    //打开界面
    public void OnEnter()
    {
        UIMana.Instance.currentState = UIState.Start;
        Bg3.GetComponent<CanvasGroup>().DOFade(1f, 1.5f)
        .OnComplete(() =>
        {
            cover.GetComponent<CanvasGroup>().blocksRaycasts = false;
        });
    }

    //关闭界面
    public void OnExit(Action onComplete = null)
    {
        Bg3.GetComponent<CanvasGroup>().DOFade(0f, 1f)
        .OnComplete(() =>
        {
            cover.GetComponent<CanvasGroup>().blocksRaycasts = true;
            onComplete?.Invoke(); //回调
        });
    }
}
