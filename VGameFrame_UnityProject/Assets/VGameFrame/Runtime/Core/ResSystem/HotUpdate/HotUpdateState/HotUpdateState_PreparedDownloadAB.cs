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
    public class HotUpdateState_PreparedDownloadAB : FSMState<HotUpdateStateId>
    {
        public HotUpdateState_PreparedDownloadAB(FSMStateMachine<HotUpdateStateId> stateMachine) : base(stateMachine)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            var totalSize = ABHotUpdater.Instance.ABDownloader.size;
            if (totalSize > 0)
            {
                var tips = string.Format("发现内容更新，总计需要下载 {0} 内容", ABDownloader.GetDisplaySize(totalSize));
                //var mb = MessageBox.Show("提示", tips, "下载", "退出");
                //yield return mb;
                //if (mb.isOk)
                if (true)
                {
                    ABHotUpdater.Instance.ABDownloader.StartDownload();
                    SetCurState(HotUpdateStateId.DownloadingAB);
                }
                else
                {
                    
                    //Quit();
                }
            }
            else
            {
                ABHotUpdater.Instance.OnComplete();
            }
        }
    }

}