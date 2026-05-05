using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

public class LuaHotUpdateButton : MonoBehaviour
{
    private LuaEnv luaEnv;
    private LuaTable skillHotUpdateTable;

    [Header("本地服务器上的 Lua 文件地址")]
    public string luaUrl = "http://127.0.0.1:8000/SkillHotUpdate.lua.txt";

    private void Start()
    {
        luaEnv = new LuaEnv();
    }

    public void OnClickHotUpdateSkillImage()
    {
        StartCoroutine(DownloadLuaAndRun());
    }

    private IEnumerator DownloadLuaAndRun()
    {
        Debug.Log("开始下载 Lua 文件：" + luaUrl);

        UnityWebRequest request = UnityWebRequest.Get(luaUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Lua 文件下载失败：" + request.error);
            yield break;
        }

        string luaCode = request.downloadHandler.text;

        if (string.IsNullOrEmpty(luaCode))
        {
            Debug.LogError("下载到的 Lua 内容为空");
            yield break;
        }

        Debug.Log("Lua 文件下载成功，开始执行");

        try
        {
            skillHotUpdateTable?.Dispose();
            skillHotUpdateTable = null;

            object[] results = luaEnv.DoString(luaCode);

            if (results != null && results.Length > 0)
            {
                skillHotUpdateTable = results[0] as LuaTable;
            }

            if (skillHotUpdateTable == null)
            {
                Debug.LogError("Lua 执行成功，但没有返回 SkillHotUpdate 表");
                yield break;
            }

            LuaFunction func = skillHotUpdateTable.Get<LuaFunction>("UpdateSkillImage");

            if (func == null)
            {
                Debug.LogError("Lua 中没有 UpdateSkillImage 方法");
                yield break;
            }

            func.Call();
            func.Dispose();

            Debug.Log("Lua 热更新函数调用完成");
        }
        catch (Exception e)
        {
            Debug.LogError("执行 Lua 失败：" + e);
        }
    }

    public void OnClickResetSkillImage()
    {
        if (skillHotUpdateTable == null)
        {
            SkillWave.HotUpdateSprite = null;
            Debug.Log("已恢复默认技能图片");
            return;
        }

        LuaFunction func = skillHotUpdateTable.Get<LuaFunction>("ResetSkillImage");

        if (func == null)
        {
            SkillWave.HotUpdateSprite = null;
            Debug.Log("Lua 中没有 ResetSkillImage，直接恢复默认技能图片");
            return;
        }

        func.Call();
        func.Dispose();
    }

    private void OnDestroy()
    {
        skillHotUpdateTable?.Dispose();
        luaEnv?.Dispose();
    }
}