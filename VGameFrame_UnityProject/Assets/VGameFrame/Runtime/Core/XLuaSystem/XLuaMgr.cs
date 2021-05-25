//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

namespace VGameFramework
{
	public class XLuaMgr : TSingleton<XLuaMgr>, IManager
    {
        private LuaEnv m_LuaEnv = null;

        #region IManager

        public void OnInit()
        {
            m_LuaEnv = new LuaEnv();

            //m_LuaEnv.DoString("print('hello world')");

            m_LuaEnv.AddLoader(FileLoader);
            //m_LuaEnv.DoString("XLuaTest");
            string testFileName = @"TextXLuaHotFix.lua.txt";

            ABHotUpdater.Instance.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(LoadLuaFileCorotine(testFileName));
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        #endregion


        IEnumerator LoadLuaFileCorotine(string fileName)
        {
            UnityWebRequest request = UnityWebRequest.Get(EngineConfig.Instance.url + @"Code\" + fileName);
            yield return request.SendWebRequest();
            string str = request.downloadHandler.text;
            File.WriteAllText(Application.persistentDataPath + @"\" + fileName, str);
            Debug.Log("Lua file download finished");

            m_LuaEnv.DoString("require'TextXLuaHotFix.lua.txt'");
        }

        private byte[] FileLoader(ref string fileName)
        {
            string filePath = Application.persistentDataPath + @"\" + fileName;
            return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath));
        }
    }

}