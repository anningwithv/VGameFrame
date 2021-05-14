//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace VGameFrame
{
	public class ABHotUpdater1 : TMonoSingleton<ABHotUpdater1>
	{
        private string m_Platform;
        private string m_SavePath;
        private ABDownloader m_ABDownloader;

        private FSMStateMachine<HotUpdateStateId> m_StateMachine = null;

        public string SavePath { get => m_SavePath;}
        public string Platform { get => m_Platform;}
        public ABDownloader ABDownloader { get => m_ABDownloader; }

        private void Start()
        {
            m_SavePath = string.Format("{0}/DLC/", Application.persistentDataPath);
            m_Platform = PlatformUtil.GetPlatformForAssetBundles(Application.platform);
            m_ABDownloader = gameObject.GetComponent<ABDownloader>();

            m_StateMachine = new FSMStateMachine<HotUpdateStateId>();

            m_StateMachine.RegisterState(HotUpdateStateId.Wait, new HotUpdateState_Wait(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.CopyVersionsFromLocal, new HotUpdateState_CopyVersionFromLocal(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.DownloadVersionsFromServer, new HotUpdateState_DownloadVersionsFromServer(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.PreparedDownloadAB, new HotUpdateState_PrepareDownload(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.DownloadAB, new HotUpdateState_DownloadVersionsFromServer(m_StateMachine));

            m_StateMachine.SetCurState(HotUpdateStateId.Wait);
        }

        private void Update()
        {
            m_StateMachine.OnUpdate();
        }

        
    }

}