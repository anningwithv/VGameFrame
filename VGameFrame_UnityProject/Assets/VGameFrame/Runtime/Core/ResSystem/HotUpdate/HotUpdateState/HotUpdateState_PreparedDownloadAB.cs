//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VGameFrame
{
    public class HotUpdateState_PreparedDownloadAB : FSMState<HotUpdateStateId>
    {
        public HotUpdateState_PreparedDownloadAB(FSMStateMachine<HotUpdateStateId> stateMachine) : base(stateMachine)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            var totalSize = ABHotUpdater1.Instance.ABDownloader.size;
            if (totalSize > 0)
            {
                var tips = string.Format("�������ݸ��£��ܼ���Ҫ���� {0} ����", ABDownloader.GetDisplaySize(totalSize));
                //var mb = MessageBox.Show("��ʾ", tips, "����", "�˳�");
                //yield return mb;
                //if (mb.isOk)
                if (true)
                {
                    ABHotUpdater1.Instance.ABDownloader.StartDownload();
                    SetCurState(HotUpdateStateId.DownloadingAB);
                }
                else
                {
                    //Quit();
                }
            }
            else
            {
                //OnComplete();
            }
        }
    }

}