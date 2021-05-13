using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    private static string bundleVersion = "1";
    private static string package = "test";

    [MenuItem("Version2AB/测试/测试Version写入")]
    public static void TVersion()
    {
        SaveVersion(bundleVersion, package);
    }

    static void SaveVersion(string version, string package)
    {
        string content = "Version|" + version + ";PackageName|" + package + ";";
        string savePath = Application.dataPath + "/Resources/Version.txt";
        string oneLine = "";
        string all = "";
        using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                all = sr.ReadToEnd();
                oneLine = all.Split('\r')[0];
            }
        }
        using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
        {
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                if (string.IsNullOrEmpty(all))
                {
                    all = content;
                }
                else
                {
                    all = all.Replace(oneLine, content);
                }
                sw.Write(all);
            }
        }
    }

}
