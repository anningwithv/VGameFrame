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
	public class ABHotUpdater : MonoBehaviour
	{
        enum Step
        {
            Wait,
            Copy,
            Coping,
            Versions,
            Prepared,
            Download,
        }

        [SerializeField] private bool development;

        private ABDownloader _downloader;
        //private NetworkMonitor _monitor;
        private string _platform;
        private string _savePath;
        private List<VersionFile> _versions = new List<VersionFile>();

        private Step _step;

        private IEnumerator _checking;

        EngineConfig engineConfig;

        private void Start()
        {
            engineConfig = ScriptableObject.CreateInstance<EngineConfig>();

            _downloader = gameObject.GetComponent<ABDownloader>();
            _downloader.onUpdate = OnUpdate;
            _downloader.onFinished = OnComplete;

            //_monitor = gameObject.GetComponent<NetworkMonitor>();
            //_monitor.listener = this;

            _savePath = string.Format("{0}/DLC/", Application.persistentDataPath);
            _platform = GetPlatformForAssetBundles(Application.platform);

            _step = Step.Wait;

            //Assets.updatePath = _savePath;

            StartUpdate();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (/*_reachabilityChanged ||*/ _step == Step.Wait)
            {
                return;
            }

            if (hasFocus)
            {
                //MessageBox.CloseAll();
                if (_step == Step.Download)
                {
                    _downloader.Restart();
                }
                else
                {
                    StartUpdate();
                }
            }
            else
            {
                if (_step == Step.Download)
                {
                    _downloader.Stop();
                }
            }
        }


        #region Public
        public void StartUpdate()
        {
            Debug.Log("StartUpdate.Development:" + development);
//#if UNITY_EDITOR
//            if (development)
//            {
//                ABResMgr.runtimeMode = false;
//                //StartCoroutine(LoadGameScene());
//                return;
//            }
//#endif
            //OnStart();

            if (_checking != null)
            {
                StopCoroutine(_checking);
            }

            _checking = Checking();

            StartCoroutine(_checking);
        }
        #endregion

        #region Private
        private void OnUpdate(long progress, long size, float speed)
        {
            //OnMessage(string.Format("下载中...{0}/{1}, 速度：{2}",
            //    Downloader.GetDisplaySize(progress),
            //    Downloader.GetDisplaySize(size),
            //    Downloader.GetDisplaySpeed(speed)));

            //OnProgress(progress * 1f / size);
            Debug.Log("Download progress: " + progress * 1f / size);
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
            var version = ABVersions.LoadVersion(_savePath + ABVersions.versionDetail);
            //if (version > 0)
            //{
            //    OnVersion(version.ToString());
            //}

            //StartCoroutine(LoadGameScene());
        }

        private IEnumerator Checking()
        {
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }

            if (_step == Step.Wait)
            {
                //yield return RequestVFS();
                _step = Step.Copy;
            }

            if (_step == Step.Copy)
            {
                yield return RequestCopy();
            }

            if (_step == Step.Coping)
            {
                var path = _savePath + ABVersions.versionDetail + ".tmp";
                var versions = ABVersions.LoadVersions(path);
                var basePath = GetStreamingAssetsPath() + "/";
                yield return UpdateCopy(versions, basePath);
                _step = Step.Versions;
            }

            if (_step == Step.Versions)
            {
                yield return RequestVersions();
            }

            if (_step == Step.Prepared)
            {
                OnMessage("正在检查版本信息...");
                var totalSize = _downloader.size;
                if (totalSize > 0)
                {
                    var tips = string.Format("发现内容更新，总计需要下载 {0} 内容", ABDownloader.GetDisplaySize(totalSize));
                    //var mb = MessageBox.Show("提示", tips, "下载", "退出");
                    //yield return mb;
                    //if (mb.isOk)
                    if(true)
                    {
                        _downloader.StartDownload();
                        _step = Step.Download;
                    }
                    else
                    {
                        Quit();
                    }
                }
                else
                {
                    OnComplete();
                }
            }
        }

        private IEnumerator RequestVersions()
        {
            OnMessage("正在获取版本信息...");
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

            var request = UnityWebRequest.Get(GetDownloadURL(ABVersions.versionDetail));
            request.downloadHandler = new DownloadHandlerFile(_savePath + ABVersions.versionDetail);
            yield return request.SendWebRequest();
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
                yield break;
            }
            try
            {
                _versions = ABVersions.LoadVersions(_savePath + ABVersions.versionDetail, true);
                if (_versions.Count > 0)
                {
                    PrepareDownloads();
                    _step = Step.Prepared;
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
            return string.Format("{0}{1}/{2}", engineConfig.url, _platform, filename);
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform target)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (target)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "iOS"; // OSX
                default:
                    return null;
            }
        }

        private static string GetStreamingAssetsPath()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return Application.streamingAssetsPath;
            }

            if (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.WindowsEditor)
            {
                return "file:///" + Application.streamingAssetsPath;
            }

            return "file://" + Application.streamingAssetsPath;
        }

        private IEnumerator RequestCopy()
        {
            var v1 = ABVersions.LoadVersion(_savePath + ABVersions.versionDetail);
            var basePath = GetStreamingAssetsPath() + "/";
            var request = UnityWebRequest.Get(basePath + ABVersions.versionDetail);
            var path = _savePath + ABVersions.versionDetail + ".tmp";
            request.downloadHandler = new DownloadHandlerFile(path);
            yield return request.SendWebRequest();
            if (string.IsNullOrEmpty(request.error))
            {
                var v2 = ABVersions.LoadVersion(path);
                if (v2 > v1)
                {
                    //var mb = MessageBox.Show("提示", "是否将资源解压到本地？", "解压", "跳过");
                    //yield return mb;
                    //_step = mb.isOk ? Step.Coping : Step.Versions;
                    _step = Step.Coping;
                }
                else
                {
                    ABVersions.LoadVersions(path);
                    _step = Step.Versions;
                }
            }
            else
            {
                _step = Step.Versions;
            }
            request.Dispose();
        }

        private IEnumerator UpdateCopy(IList<VersionFile> versions, string basePath)
        {
            var version = versions[0];
            if (version.name.Equals(ABVersions.bundleDetail))
            {
                var request = UnityWebRequest.Get(basePath + version.name);
                request.downloadHandler = new DownloadHandlerFile(_savePath + version.name);
                var req = request.SendWebRequest();
                while (!req.isDone)
                {
                    OnMessage("正在复制文件");
                    OnProgress(req.progress);
                    yield return null;
                }

                request.Dispose();
            }
            else
            {
                for (var index = 0; index < versions.Count; index++)
                {
                    var item = versions[index];
                    var request = UnityWebRequest.Get(basePath + item.name);
                    request.downloadHandler = new DownloadHandlerFile(_savePath + item.name);
                    yield return request.SendWebRequest();
                    request.Dispose();
                    OnMessage(string.Format("正在复制文件：{0}/{1}", index, versions.Count));
                    OnProgress(index * 1f / versions.Count);
                }
            }
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

            for (var i = 1; i < _versions.Count; i++)
            {
                var item = _versions[i];
                if (ABVersions.IsNew(string.Format("{0}{1}", _savePath, item.name), item.len, item.hash))
                {
                    AddDownload(item);
                }
            }
        }

        private void AddDownload(VersionFile item)
        {
            _downloader.AddDownload(GetDownloadURL(item.name), item.name, _savePath + item.name, item.hash, item.len);
        }

        private void OnMessage(string msg)
        {
            //if (listener != null)
            //{
            //    listener.OnMessage(msg);
            //}
        }

        private void OnProgress(float progress)
        {
            //if (listener != null)
            //{
            //    listener.OnProgress(progress);
            //}
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion
    }

}