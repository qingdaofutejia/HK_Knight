using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEff : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    GameObject leftEff;
    GameObject rightEff;

    GameObject LeftEff;
    GameObject RightEff;

    public float extraOffset = 0f; // 特殊偏移

    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform rect = GetComponent<RectTransform>();
        float width = rect.rect.width * rect.lossyScale.x;

        float offset = width / 2 + 20f + extraOffset;

        LeftEff = Instantiate(leftEff, transform);
        LeftEff.transform.localPosition = new Vector3(-offset, 0, 0);

        RightEff = Instantiate(rightEff, transform);
        RightEff.transform.localPosition = new Vector3(offset, 0, 0);
    }
    //鼠标退出

    public void OnPointerExit(PointerEventData eventData)
    {
        if (LeftEff != null) Destroy(LeftEff);
        if (RightEff != null) Destroy(RightEff);
    }

    // Start is called before the first frame update
    void Start()
    {
        leftEff = Resources.Load<GameObject>("Eff/LeftEff");
        rightEff = Resources.Load<GameObject>("Eff/RightEff");
    }
}
