using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Asynchronous : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // 开始加载
        AsyncOperation op = SceneManager.LoadSceneAsync(2);
        op.allowSceneActivation = false;

        float timer = 0f;

        // 等2秒 + 等加载完成
        while (timer < 2f || op.progress < 0.9f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 允许进入场景
        op.allowSceneActivation = true;
    }
}
