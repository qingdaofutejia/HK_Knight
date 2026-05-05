using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Source")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clip")]
    public AudioClip bgmClip;
    public AudioClip attackClip;

    private const string BGM_VOLUME = "BGM";
    private const string SFX_VOLUME = "SFX";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBGM();

        SetBGMVolume(PlayerPrefs.GetFloat("BGM", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFX", 1f));
    }

    public void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null)
            return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayAttackSFX()
    {
        PlaySFX(attackClip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float value)
    {
        //value = Mathf.Clamp01(value);

        //float db = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;

        //audioMixer.SetFloat(BGM_VOLUME, db);
        //PlayerPrefs.SetFloat("BGMVolume", value);
        value = Mathf.Clamp01(value);

        if (audioMixer == null)
        {
            Debug.LogError("AudioManager 没有绑定 audioMixer");
            return;
        }

        float db = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;

        bool result = audioMixer.SetFloat("BGM", db);

        Debug.Log("设置 BGM 音量 value=" + value + " db=" + db + " result=" + result);

        PlayerPrefs.SetFloat("BGM", value);
    }

    public void SetSFXVolume(float value)
    {
        //value = Mathf.Clamp01(value);

        //float db = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;

        //audioMixer.SetFloat(SFX_VOLUME, db);
        //PlayerPrefs.SetFloat("SFXVolume", value);
        value = Mathf.Clamp01(value);

        if (audioMixer == null)
        {
            Debug.LogError("AudioManager 没有绑定 audioMixer");
            return;
        }

        float db = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;

        bool result = audioMixer.SetFloat("SFX", db);

        Debug.Log("设置 SFX 音量 value=" + value + " db=" + db + " result=" + result);

        PlayerPrefs.SetFloat("SFX", value);
    }
}