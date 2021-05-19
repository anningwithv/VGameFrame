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
	public class ABHotUpdater : TMonoSingleton<ABHotUpdater>
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
            m_SavePath = string.Format("{0}/", Application.persistentDataPath);
            m_Platform = PlatformUtil.GetPlatformForAssetBundles(Application.platform);
            m_ABDownloader = gameObject.GetComponent<ABDownloader>();
            m_ABDownloader.onUpdate = OnUpdate;
            m_ABDownloader.onFinished = OnComplete;

            m_StateMachine = new FSMStateMachine<HotUpdateStateId>();

            m_StateMachine.RegisterState(HotUpdateStateId.Wait, new HotUpdateState_Wait(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.StartCopyVersionsFromLocal, new HotUpdateState_CopyVersionFromLocal(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.CopingVersionsFromStreamingAssets, new HotUpdateState_CopingVersionsFromStreamingAssets(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.DownloadVersionsFromServer, new HotUpdateState_DownloadVersionsFromServer(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.PreparedDownloadAB, new HotUpdateState_PreparedDownloadAB(m_StateMachine));
            m_StateMachine.RegisterState(HotUpdateStateId.DownloadingAB, new HotUpdateState_DownloadingAB(m_StateMachine));

            m_StateMachine.SetCurState(HotUpdateStateId.Wait);
        }

        private void Update()
        {
            m_StateMachine.OnUpdate();
        }

        private void OnUpdate(long progress, long size, float speed)
        {
            //OnMessage(string.Format("下载中...{0}/{1}, 速度：{2}",
            //    Downloader.GetDisplaySize(progress),
            //    Downloader.GetDisplaySize(size),
            //    Downloader.GetDisplaySpeed(speed)));

            //OnProgress(progress * 1f / size);
            Debug.Log("Download progress: " + progress * 1f / size);
        }

        public void OnComplete()
        {
            //if (enableVFS)
            //{
            //    var dataPath = _savePath + Versions.Dataname;
            //    var downloads = _downloader.downloads;
            //    if (downloads.Count > 0 && File.Exists(dataPath))
            //    {
            //        OnMessage("更新本地版本信息");
            //        var files = new List<VFile>(downloads.Count);
            //        foreach (var download in downloads)
            //        {
            //            files.Add(new VFile
            //            {
            //                name = download.name,
            //                hash = download.hash,
            //                len = download.len,
            //            });
            //        }

            //        var file = files[0];
            //        if (!file.name.Equals(Versions.Dataname))
            //        {
            //            Versions.UpdateDisk(dataPath, files);
            //        }
            //    }

            //    Versions.LoadDisk(dataPath);
            //}

            //OnProgress(1);
            //OnMessage("更新完成");
            Debug.Log("Download finish");
            //var version = ABVersions.LoadVersion(m_SavePath + ABVersions.versionDetail);
            //if (version > 0)
            //{
            //    OnVersion(version.ToString());
            //}

            //StartCoroutine(LoadGameScene());

            //StartCoroutine(LoadManifest());
            ResMgr.Instance.LoadManifest();
            ResLoader resLoader = new ResLoader();
            GameObject mainMenuPrefab = resLoader.LoadSync<GameObject>(ResType.BundleAsset, "MainMenuPanel.prefab");
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject go = GameObject.Instantiate(mainMenuPrefab);
                go.transform.SetParent(canvas.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = new Vector3(1, 1, 1);
            }
            //resLoader.LoadSync<GameObject>(ResType.BundleAsset, "Triangle.png");
        }
   
    }

}