//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VGameFramework
{
    [CreateAssetMenu(menuName = "VGameFramework/Create ProjectPathConfig ")]
    public class ProjectPathConfig : ScriptableObject
	{

        #region 配置工具相关
        [SerializeField]
        private string m_BuildCSharpWinShell = "table/output_code_csharp.bat";
        [SerializeField]
        public string m_BuildTxtDataWinShell = "table/output_txt.bat";
        [SerializeField]
        public string m_BuildLrgDataWinShell = "table/output_xc.bat";
        [SerializeField]
        public string m_BuildCSharpLinuxShell = "table/output_code_csharp.sh";
        [SerializeField]
        public string m_BuildTxtDataLinuxShell = "table/output_txt.sh";
        [SerializeField]
        public string m_BuildTxtDataUnixShell = "table/output_txt_unix.sh";
        [SerializeField]
        public string m_BuildLrgDataLinuxShell = "table/output_xc.sh";

        [SerializeField]
        private string m_ProjectToolsFolder = @"../../../Tools/";
        #endregion


        #region 初始化过程
        private static ProjectPathConfig s_Instance;

        private static ProjectPathConfig LoadInstance()
        {
            s_Instance = Resources.Load("ProjectPathConfig") as ProjectPathConfig;

            return s_Instance;
        }

        #endregion

        public static ProjectPathConfig Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = LoadInstance();
                }

                return s_Instance;
            }
        }

        #region 配置工具相关

        public static string buildCSharpWinShell
        {
            get { return Instance.m_BuildCSharpWinShell; }
        }

        public static string buildTxtDataWinShell
        {
            get { return Instance.m_BuildTxtDataWinShell; }
        }

        public static string buildLrgDataWinShell
        {
            get { return Instance.m_BuildLrgDataWinShell; }
        }

        public static string buildCSharpLinuxShell
        {
            get { return Instance.m_BuildCSharpLinuxShell; }
        }

        public static string buildTxtDataLinuxShell
        {
            get { return Instance.m_BuildTxtDataLinuxShell; }
        }

        public static string buildTxtDataUnixShell
        {
            get { return Instance.m_BuildTxtDataUnixShell; }
        }

        public static string buildLrgDataLinuxShell
        {
            get { return Instance.m_BuildLrgDataLinuxShell; }
        }

        public static string projectToolsFolder
        {
            get { return Application.dataPath + Instance.m_ProjectToolsFolder; }
        }

        #endregion
    }

}