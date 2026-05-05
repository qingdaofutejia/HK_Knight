using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public static SettingPanel Instance;

    public Slider bgmSlider;
    public Slider sfxSlider;

    Button exitBtn;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        transform.GetComponent<CanvasGroup>().alpha = 0;
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.GetComponent<CanvasGroup>().interactable = false;

        float bgmValue = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (bgmSlider != null)
        {
            bgmSlider.value = bgmValue;
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxValue;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        AudioManager.Instance?.SetBGMVolume(bgmValue);
        AudioManager.Instance?.SetSFXVolume(sfxValue);

        exitBtn = transform.Find("Return").GetComponent<Button>();
        exitBtn.onClick.AddListener(OnExit);
    }
    public void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance?.SetBGMVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //打开界面
    public void OnEnter()
    {
        UIMana.Instance.currentState = UIState.Setting;
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
