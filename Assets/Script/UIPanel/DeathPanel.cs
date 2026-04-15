using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPanel : MonoBehaviour
{
    public static DeathPanel Instance;

    public RectTransform black; // 黑幕

    public float duration = 0.5f;

    private void Awake()
    {
        Instance = this;
        black.localScale = Vector3.zero;
    }

    public void Play(System.Action onBlackFull)
    {
        StartCoroutine(DeathCoroutine(onBlackFull));
    }

    IEnumerator DeathCoroutine(System.Action onBlackFull)
    {
        //等待播放死亡动画
        yield return new WaitForSeconds(1.2f);
        // 收缩（四周→中间）
        yield return Scale(Vector3.zero, Vector3.one);

        // 完全黑了
        onBlackFull?.Invoke();

        yield return new WaitForSeconds(0.5f);

        //展开（中间→四周）
        yield return Scale(Vector3.one, Vector3.zero);
    }

    IEnumerator Scale(Vector3 from, Vector3 to)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            black.localScale = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }
        black.localScale = to;
    }
}
