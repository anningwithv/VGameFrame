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
            var v1 = ABVersions.LoadVersion(ABHotUpdater1.Instance.SavePath + ABVersions.versionDetail);
            var basePath = PlatformUtil.GetStreamingAssetsPath() + "/";
            var request = UnityWebRequest.Get(basePath + ABVersions.versionDetail);
            var path = ABHotUpdater1.Instance.SavePath + ABVersions.versionDetail + ".tmp";
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
                    //_step = Step.Coping;
                    SetCurState(HotUpdateStateId.CopingVersionsFromLocal);
                }
                else
                {
                    ABVersions.LoadVersions(path);
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