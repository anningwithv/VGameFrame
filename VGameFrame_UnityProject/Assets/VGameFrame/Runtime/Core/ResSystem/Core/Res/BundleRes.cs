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
    public class BundleRes : Res
    {
        private ResLoader m_ResLoader = new ResLoader();

        private AssetBundle m_AssetBundle;

        public BundleRes(string bundleName)
        {
            if (string.IsNullOrEmpty(bundleName))
            {
                Debug.LogError("invalid path");
            }

            Name = bundleName;

            State = ResState.Waiting;

            ResType = ResType.Bundle;
        }

        public override bool LoadSync()
        {
            State = ResState.Loading;

            var url = ResMgr.Instance.GetDataPath(Name) + Name;
            m_AssetBundle = AssetBundle.LoadFromFile(url);

            Asset = m_AssetBundle;

            State = ResState.Loaded;

            return m_AssetBundle != null;
        }

        public override void LoadAsync()
        {
            State = ResState.Loading;

            if (!ResMgr.Instance.runtimeMode)
            {
                State = ResState.Loaded;
            }
            else
            {
                var url = ResMgr.Instance.GetDataPath(Name) + Name;
                var resRequest = AssetBundle.LoadFromFileAsync(url);

                resRequest.completed += operation =>
                {
                    m_AssetBundle = resRequest.assetBundle;
                    Asset = m_AssetBundle;

                    State = ResState.Loaded;
                };
            }
        }

        protected override void OnReleaseRes()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;

                m_ResLoader.ReleaseAll();
                m_ResLoader = null;
            }

            ResMgr.Instance.LoadedAssets.Remove(Name);
        }
    }

}