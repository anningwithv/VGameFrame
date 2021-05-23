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
    public class UIMgr : TSingleton<UIMgr>, IManager
    {
        private Transform m_UIRoot;

        private Dictionary<UIID, string> m_PanelPathDic = new Dictionary<UIID, string>();

        private Dictionary<UIID, UIPanel> m_ActivePanelDic = new Dictionary<UIID, UIPanel>();
        private List<UIPanel> m_ActivePanelList = new List<UIPanel>();

        #region IManager
        public void OnInit()
        {
            m_UIRoot = GameObject.FindObjectOfType<UIRoot>().transform;
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
        }
        #endregion

        public void OpenPanel<T>(UIID panelId)
        {
            UIPanel panel = null;
            if (m_ActivePanelDic.TryGetValue(panelId, out panel))
            {

            }
            else
            {
            }
        }


    }

}