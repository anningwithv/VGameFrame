//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Networking;

namespace VGameFrame
{
    public class HotUpdateState_CopingVersionsFromStreamingAssets : FSMState<HotUpdateStateId>
    {
        public HotUpdateState_CopingVersionsFromStreamingAssets(FSMStateMachine<HotUpdateStateId> stateMachine) : base(stateMachine)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            Observable.FromCoroutine(UpdateCopy).Subscribe(_ => { SetCurState(HotUpdateStateId.DownloadVersionsFromServer); }).AddTo(ABHotUpdater1.Instance.gameObject);
        }

        private IEnumerator UpdateCopy()
        {
            var path = ABHotUpdater1.Instance.SavePath + ABVersions.versionDetail + ".tmp";
            var versions = ABVersions.LoadVersions(path);
            var basePath = PlatformUtil.GetStreamingAssetsPath() + "/";

            var version = versions[0];
            if (version.name.Equals(ABVersions.bundleDetail))
            {
                var request = UnityWebRequest.Get(basePath + version.name);
                request.downloadHandler = new DownloadHandlerFile(ABHotUpdater1.Instance.SavePath + version.name);
                var req = request.SendWebRequest();
                while (!req.isDone)
                {
                    //OnMessage("正在复制文件");
                    //OnProgress(req.progress);
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
                    request.downloadHandler = new DownloadHandlerFile(ABHotUpdater1.Instance.SavePath + item.name);
                    yield return request.SendWebRequest();
                    request.Dispose();
                    //OnMessage(string.Format("正在复制文件：{0}/{1}", index, versions.Count));
                    //OnProgress(index * 1f / versions.Count);
                }
            }
        }

    }

}