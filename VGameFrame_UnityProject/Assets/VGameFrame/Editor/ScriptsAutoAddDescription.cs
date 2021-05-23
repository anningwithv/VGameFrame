using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VGameFramework
{
    public class ScriptsAutoAddDescription : UnityEditor.AssetModificationProcessor
    {
        static string namespaceName = "VGameFramework";

        /// <summary>
        /// 将要创建资源时会调用这个函数
        /// </summary>
        static void OnWillCreateAsset(string path)
        {
            //导入资源的路径，不知道具体是什么的时候建议输出查看
            Debug.Log(path);
            string str = path.Replace(".meta", "");
            string[] splitArgs = str.Split('.');
            if (splitArgs[splitArgs.Length - 1].Equals("cs"))
            {
                //Debug.Log("导入的是脚本");
                string[] newSplitArgs = str.Split('/');
                bool isEditor = false;
                foreach (var item in newSplitArgs)
                {
                    if (item.Equals("Editor"))
                    {
                        isEditor = true;
                        break; ;
                    }
                }
                if (isEditor) return;
                ParseAndChangeScript(str.Substring(6, str.Length - 6));
            }
        }

        private static void ParseAndChangeScript(string path)
        {
            string str = File.ReadAllText(Application.dataPath + path);
            if (string.IsNullOrEmpty(str))
            {
                Debug.Log("读取出错了，Application.dataPath=" + Application.dataPath + "  path=" + path);
                return;
            }

            string newStr = "";

            newStr += "//-------------------------------------------------------\n";
            newStr += "//  Desc:        Framework For Game Develop with Unity3d \n";
            newStr += "//  Copyright:   Copyright (C) 2021. All rights reserved. \n";
            newStr += "//  Website:     https://github.com/anningwithv/VGameFramework. \n";
            newStr += "//  Author:      V \n";
            newStr += "//-------------------------------------------------------\n";
            newStr += "\n";

            //增加命名空间
            if (!str.Contains("namespace"))
            {
                if (!string.IsNullOrEmpty(namespaceName))
                {
                    int length = str.IndexOf("public");
                    newStr += str.Substring(0, length);
                    string extraStr = "";
                    string[] extraStrs = str.Substring(length, str.Length - length).Replace("\r\n", "\n").Split('\n');
                    foreach (var item in extraStrs)
                    {
                        extraStr += "\t" + item + "\r\n";
                    }


                    newStr += "\r\nnamespace " + namespaceName + "\r\n{\r\n" + extraStr + "}";
                    //newStr = newStr.Replace("\n", "\r\n");
                    //newStr = newStr.Replace('\r', ' ');
                }
                else
                {
                    newStr = str;
                }
                File.WriteAllText(Application.dataPath + path, newStr);
            }
        }

    }
}