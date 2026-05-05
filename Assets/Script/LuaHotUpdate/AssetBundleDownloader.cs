using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

[LuaCallCSharp]
public class AssetBundleDownloader : MonoBehaviour
{
    public static AssetBundleDownloader Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static void DownloadAssetBundle(
        string url,
        Action<AssetBundle> onSuccess,
        Action<string> onFailed
    )
    {
        if (Instance == null)
        {
            onFailed?.Invoke("场景中没有 AssetBundleDownloader 组件");
            return;
        }

        Instance.StartCoroutine(
            Instance.DownloadCoroutine(url, onSuccess, onFailed)
        );
    }

    private IEnumerator DownloadCoroutine(
        string url,
        Action<AssetBundle> onSuccess,
        Action<string> onFailed
    )
    {
        Debug.Log("开始下载 AB 包：" + url);

        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onFailed?.Invoke(request.error);
            yield break;
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        if (bundle == null)
        {
            onFailed?.Invoke("AssetBundle 加载失败");
            yield break;
        }

        onSuccess?.Invoke(bundle);
    }
}