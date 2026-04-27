using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting_sPanel : MonoBehaviour
{
    Button returnBtn;
    Button roomBtn;
    Button exitBrn;
    CanvasGroup cg;
    // Start is called before the first frame update
    void Start()
    {
        cg = gameObject.GetComponent<CanvasGroup>();
        //¿´²»¼û
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        returnBtn = transform.Find("Return").GetComponent<Button>();
        roomBtn = transform.Find("Room").GetComponent<Button>();
        exitBrn = transform.Find("Exit").GetComponent<Button>();

        returnBtn.onClick.AddListener(ReturnBtnOnClick);
        roomBtn.onClick.AddListener(RoomBtnOnClick);
        exitBrn.onClick.AddListener(ExitBtnOnClick);
    }

    private void ExitBtnOnClick()
    {
      
    }

    private void RoomBtnOnClick()
    {
        
    }

    private void ReturnBtnOnClick()
    {
        Asynchronous.nextScene = 0;
        SceneManager.LoadScene(1);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit();
        }
    }
    public void OnEnter()
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    public void OnExit()
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    public bool IsOpen()
    {
        return cg.alpha > 0.99f;
    }
}
