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
    public class HotUpdateState_CopyVersionFromLocal : FSMState<HotUpdateStateId>
    {
        public HotUpdateState_CopyVersionFromLocal(FSMStateMachine<HotUpdateStateId> stateMachine) : base(stateMachine)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            Observable.FromCoroutine(RequestCopy).Subscribe(_ => { }).AddTo(ABHotUpdater1.Instance.gameObject);
        }

        private IEnumerator RequestCopy()
        {
            //Load version from saved path
            var versionInSavedPath = ABVersions.LoadVersion(ABHotUpdater1.Instance.SavePath + ABVersions.versionDetail);
            var streamingAssetsPath = PlatformUtil.GetStreamingAssetsPath()+ "/DLC/" + ABHotUpdater1.Instance.Platform + "/";
            //Download version from streaming assets path to saved path .tmp
            var request = UnityWebRequest.Get(streamingAssetsPath + ABVersions.versionDetail);
            var tmpSavedPath = ABHotUpdater1.Instance.SavePath + ABVersions.versionDetail + ".tmp";
            request.downloadHandler = new DownloadHandlerFile(tmpSavedPath);

            yield return request.SendWebRequest();

            if (string.IsNullOrEmpty(request.error))
            {
                var versionInStreamingAssets = ABVersions.LoadVersion(tmpSavedPath);
                if (versionInStreamingAssets > versionInSavedPath)
                {
                    //var mb = MessageBox.Show("提示", "是否将资源解压到本地？", "解压", "跳过");
                    //yield return mb;
                    //_step = mb.isOk ? Step.Coping : Step.Versions;
                    //_step = Step.Coping;
                    SetCurState(HotUpdateStateId.CopingVersionsFromStreamingAssets);
                }
                else
                {
                    ABVersions.LoadVersions(tmpSavedPath);
                    SetCurState(HotUpdateStateId.DownloadVersionsFromServer);
                }
            }
            else
            {
                SetCurState(HotUpdateStateId.DownloadVersionsFromServer);
            }

            request.Dispose();

        }
    }

}