using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class TestXLua : MonoBehaviour
{
    private void Start()
    {
        LuaEnv luaEnv = new LuaEnv();

        luaEnv.DoString("CS.UnityEngine.Debug.Log('xLua is running')");

        luaEnv.Dispose();
    }
}
