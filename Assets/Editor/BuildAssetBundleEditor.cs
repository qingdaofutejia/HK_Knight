using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundleEditor
{
    [MenuItem("Tools/Build AssetBundles/Windows")]
    public static void BuildWindowsAssetBundles()
    {
        string outputPath = Application.dataPath + "/../AssetBundles/Windows";

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        BuildPipeline.BuildAssetBundles(
            outputPath,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows64
        );

        Debug.Log("AB 包打包完成：" + outputPath);
    }
}