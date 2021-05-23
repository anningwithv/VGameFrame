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

        //private Dictionary<UIID, string> m_PanelPathDic = new Dictionary<UIID, string>();

        private Dictionary<UIID, UIPanel> m_ActivePanelDic = new Dictionary<UIID, UIPanel>();
        private List<UIPanel> m_ActivePanelList = new List<UIPanel>();

        private UIPanelConfig m_UIPanelConfig;

        private ResLoader m_ResLoader = null;

        #region IManager
        public void OnInit()
        {
            m_UIRoot = GameObject.FindObjectOfType<UIRoot>().transform;

            m_UIPanelConfig = ScriptableObject.CreateInstance<UIPanelConfig>();

            m_ResLoader = ResLoader.Allocate("UIPanelLoader");
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
            m_ResLoader.RecycleToCache();
        }

        #endregion

        public UIPanel OpenPanel(UIID uid)
        {
            UIPanel panel = GetPanel(uid);

            return panel;
        }

        private UIPanel GetPanel(UIID uid)
        {
            UIPanel panel = null;
            if (m_ActivePanelDic.TryGetValue(uid, out panel))
            {
                return m_ActivePanelDic[uid];
            }
            else
            {
                panel = CreatePanel(uid);

                return panel;
            }
        }

        private UIPanel CreatePanel(UIID uid)
        {
            GameObject go = LoadPanel(uid);

            UIPanel panel = go.GetComponent<UIPanel>();

            return panel;
        }

        private GameObject LoadPanel(UIID uid)
        {
            //UIPanelConfig.PanelConfigInfo panelConfigInfo = m_UIPanelConfig.GetPanelConfigInfo(uid);
            //string path = panelConfigInfo.path;
            string path = "MainMenuPanel.prefab";
            GameObject panelPrefab = m_ResLoader.LoadSync<GameObject>(path);

            GameObject obj = GameObject.Instantiate(panelPrefab);

            return obj;
        }
    }

}