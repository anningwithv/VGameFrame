//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VGameFramework;
using System.Linq;

namespace VGameFramework
{
    public class UIPanelConfig : TSingleton<UIPanelConfig>
	{
        private List<PanelConfigInfo> panelPathList = new List<PanelConfigInfo>();

        public void Init()
        {

        }

        public void RegisterPanel(int uid, string panelPath)
        {
            if (IsPanelRegistered(uid) == false)
            {
                panelPathList.Add(new PanelConfigInfo(uid, panelPath));
            }
        }

        public PanelConfigInfo GetPanelConfigInfo(int uid)
        {
            PanelConfigInfo configInfo = panelPathList.FirstOrDefault(i => i.uid == uid);
            return configInfo;
        }

        private bool IsPanelRegistered(int uid)
        {
            bool exist = panelPathList.Any(i => i.uid == uid);
            return exist;
        }
	}

    [System.Serializable]
    public class PanelConfigInfo
    {
        public int uid;
        public string path;

        public PanelConfigInfo(int uid, string path)
        {
            this.uid = uid;
            this.path = path;
        }
    }
}