using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : MonoBehaviour
{

    //三个背景板
    Transform Bg1;
    Transform Bg2;
    Transform Bg3;

    Transform icon;
    // Start is called before the first frame update
    private void Awake()
    {
        Bg1 = transform.Find("Bg").transform;
        Bg2 = transform.Find("Bg2").transform;
        Bg3 = transform.Find("Bg3").transform;
        icon = Bg1.Find("Icon");
        //开始设置背景板为透明
        Bg2.GetComponent<CanvasGroup>().alpha = 0;
        Bg3.GetComponent<CanvasGroup>().alpha = 0;
        //背景1下的图标设置为透明
        icon.GetComponent<CanvasGroup>().alpha = 0;

        //动画开局
        StartPanelShow();
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
                              Bg3.GetComponent<CanvasGroup>().DOFade(1f, 1.5f);
                          });
                      });
               });
       });
    }
}
