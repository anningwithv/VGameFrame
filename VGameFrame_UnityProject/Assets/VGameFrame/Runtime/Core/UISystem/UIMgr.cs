//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VGameFramework
{
    public class UIMgr : TSingleton<UIMgr>, IManager
    {
        private Transform m_UIRoot;

        //private Dictionary<UIID, string> m_PanelPathDic = new Dictionary<UIID, string>();

        private Dictionary<int, UIPanel> m_ActivePanelDic = new Dictionary<int, UIPanel>();
        //private List<UIPanel> m_ActivePanelList = new List<UIPanel>();

        private UIPanelConfig m_UIPanelConfig;

        private ResLoader m_ResLoader = null;

        #region IManager
        public void OnInit()
        {
            m_UIRoot = GameObject.FindObjectOfType<UIRoot>().transform;

            m_UIPanelConfig = new UIPanelConfig();

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

        public UIPanel OpenPanel<T>(T uid) where T : IConvertible
        {
            UIPanel panel = GetPanel(uid);

            if (panel == null)
            {
                panel = CreatePanel(uid);
            }

            return panel;
        }

        public void ClosePanel<T>(T uid) where T : IConvertible
        {
            UIPanel panel = GetPanel(uid);

            if (panel != null)
            {
                m_ActivePanelDic.Remove(uid.ToInt32(null));
            }

            GameObject.Destroy(panel.gameObject);
        }

        private UIPanel GetPanel<T>(T uid) where T : IConvertible
        {
            UIPanel panel = null;
            int id = uid.ToInt32(null);
            if (m_ActivePanelDic.TryGetValue(id, out panel))
            {
                return m_ActivePanelDic[id];
            }

            return panel;
        }

        private UIPanel CreatePanel<T>(T uid) where T : IConvertible
        {
            GameObject go = LoadPanel(uid);

            UIPanel panel = go.GetComponent<UIPanel>();

            return panel;
        }

        private GameObject LoadPanel<T>(T uid) where T : IConvertible
        {
            //UIPanelConfig.PanelConfigInfo panelConfigInfo = m_UIPanelConfig.GetPanelConfigInfo(uid);
            //string path = panelConfigInfo.path;
            string path = "MainMenuPanel.prefab";
            GameObject panelPrefab = m_ResLoader.LoadSync<GameObject>(path);

            GameObject obj = GameObject.Instantiate(panelPrefab);
            obj.transform.SetParent(m_UIRoot);

            return obj;
        }
    }

}