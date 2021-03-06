//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Networking;
using System;

namespace VGameFramework
{
    public class HotUpdateState_DownloadVersionsFromServer : FSMState<HotUpdateStateId>
    {
        private List<VersionFile> m_Versions = new List<VersionFile>();

        public HotUpdateState_DownloadVersionsFromServer(FSMStateMachine<HotUpdateStateId> stateMachine) : base(stateMachine)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            Observable.FromCoroutine(DownloadVersionsFromServer).Subscribe(_ => { }).AddTo(ABHotUpdater.Instance.gameObject);
        }

        private IEnumerator DownloadVersionsFromServer()
        {
            //OnMessage("正在获取版本信息...");
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    //var mb = MessageBox.Show("提示", "请检查网络连接状态", "重试", "退出");
            //    //yield return mb;
            //    //if (mb.isOk)
            //    if (true)
            //    {
            //        StartUpdate();
            //    }
            //    else
            //    {
            //        Quit();
            //    }
            //    yield break;
            //}
            Debug.Log("DownloadVersionsFromServer: Create request");
            string url = GetDownloadURL(ABVersions.versionDetail);
            Debug.Log("DownloadVersionsFromServer, url is: " + url);
            var request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerFile(ABHotUpdater.Instance.SavePath + ABVersions.versionDetail);
            yield return request.SendWebRequest();
            Debug.Log("DownloadVersionsFromServer: Download done");
            var error = request.error;
            request.Dispose();
            if (!string.IsNullOrEmpty(error))
            {
                //var mb = MessageBox.Show("提示", string.Format("获取服务器版本失败：{0}", error), "重试", "退出");
                //yield return mb;
                //if (mb.isOk)
                //if (true)
                //{
                //    StartUpdate();
                //}
                //else
                //{
                //    Quit();
                //}
                Debug.LogError("Download versions Error: " + error.ToString());

                ABHotUpdater.Instance.OnComplete();
                yield break;
            }
            try
            {
                m_Versions = ABVersions.LoadVersions(ABHotUpdater.Instance.SavePath + ABVersions.versionDetail, true);
                if (m_Versions.Count > 0)
                {
                    PrepareDownloads();

                    SetCurState(HotUpdateStateId.PreparedDownloadAB);
                }
                else
                {
                    OnComplete();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                //MessageBox.Show("提示", "版本文件加载失败", "重试", "退出").onComplete +=
                //    delegate (MessageBox.EventId id)
                //    {
                //        if (id == MessageBox.EventId.Ok)
                //        {
                //            StartUpdate();
                //        }
                //        else
                //        {
                //            Quit();
                //        }
                //    };
            }
        }

        private string GetDownloadURL(string filename)
        {
            string url = string.Format("{0}{1}/{2}", EngineConfig.Instance.url + "DLC/", ABHotUpdater.Instance.Platform, filename);
            return url;
        }

        private void PrepareDownloads()
        {
            //if (enableVFS)
            //{
            //    var path = string.Format("{0}{1}", _savePath, Versions.Dataname);
            //    if (!File.Exists(path))
            //    {
            //        AddDownload(_versions[0]);
            //        return;
            //    }

            //    Versions.LoadDisk(path);
            //}

            for (var i = 1; i < m_Versions.Count; i++)
            {
                var item = m_Versions[i];
                if (ABVersions.IsNew(string.Format("{0}{1}", ABHotUpdater.Instance.SavePath, item.name), item.len, item.hash))
                {
                    AddDownload(item);
                }
            }
        }

        private void AddDownload(VersionFile item)
        {
            ABHotUpdater.Instance.ABDownloader.AddDownload(GetDownloadURL(item.name), item.name, ABHotUpdater.Instance.SavePath + item.name, item.hash, item.len);
        }

        private void OnComplete()
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
            //var version = ABVersions.LoadVersion(ABHotUpdater.Instance.SavePath + ABVersions.versionDetail);
            //if (version > 0)
            //{
            //    OnVersion(version.ToString());
            //}

            //StartCoroutine(LoadGameScene());
        }


    }
}