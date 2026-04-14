using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMana:MonoBehaviour
{
    public static UIMana Instance;

    public UIState currentState;

    private void Awake()
    {
        Instance = this;
    }
}
public enum UIState
{
    None,
    Start,
    Choice,
    Setting
}
