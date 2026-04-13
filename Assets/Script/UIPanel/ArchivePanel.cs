using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchivePanel : MonoBehaviour
{
    Image BgImg;
    Text numberTxt;
    Transform HPlist;
    Text timerTxt;
    GameObject HpPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        BgImg = transform.Find("BG").GetComponent<Image>();
        numberTxt = transform.Find("number").GetComponent<Text>();
        HPlist = transform.Find("Avatar");
        timerTxt = transform.Find("Timer").GetComponent<Text>();
        HpPrefabs = Resources.Load<GameObject>("Hp");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
