using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    public static ChoicePanel Instance;

    //退出按钮
    Button exitBtn;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;

        transform.GetComponent<CanvasGroup>().alpha = 0;
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.GetComponent<CanvasGroup>().interactable = false;

        exitBtn = transform.Find("Return").GetComponent<Button>();
        exitBtn.onClick.AddListener(OnExit);
    }
    


    //打开界面
    public void OnEnter()
    {
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
}
