//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VGameFrame
{
    public class BundleRes : Res
    {
        private AssetBundle m_AssetBundle;

        //private string m_Path;

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

        private ResLoader m_ResLoader = new ResLoader();


        public override bool LoadSync()
        {
            State = ResState.Loading;

            //if (ResMgr.Instance.runtimeMode)
            //{   
            //    string[] dependencyBundleNames = ResMgr.Instance.GetAllDependencies(Name);

            //    foreach (var dependencyBundleName in dependencyBundleNames)
            //    {
            //        m_ResLoader.LoadSync<AssetBundle>(ResType.Bundle, dependencyBundleName);
            //    }
                
            //}

            var url = ResMgr.Instance.GetDataPath(Name) + Name;
            m_AssetBundle = AssetBundle.LoadFromFile(url);

            Asset = m_AssetBundle;

            State = ResState.Loaded;

            return m_AssetBundle != null;
        }

        //private void LoadDependencyBundlesAsync(Action onAllLoaded)
        //{

        //    string[] dependencyBundleNames = ResMgr.Instance.GetAllDependencies(Name);

        //    var loadedCount = 0;

        //    if (dependencyBundleNames.Length == 0)
        //    {
        //        if (onAllLoaded != null)
        //            onAllLoaded.Invoke();
        //    }

        //    foreach (var dependencyBundleName in dependencyBundleNames)
        //    {
        //        m_ResLoader.LoadAsync<AssetBundle>(ResType.Bundle, dependencyBundleName,
        //            dependBundle =>
        //            {
        //                loadedCount++;

        //                if (loadedCount == dependencyBundleNames.Length)
        //                {
        //                    if (onAllLoaded != null)
        //                        onAllLoaded.Invoke();
        //                }
        //            });
        //    }  
        //}

        public override void LoadAsync()
        {
            State = ResState.Loading;

            //LoadDependencyBundlesAsync(() =>
            //{

            //});

            if (!ResMgr.Instance.runtimeMode)
            {
                State = ResState.Loaded;
            }
            else
            {
                var resRequest = AssetBundle.LoadFromFileAsync(Name);

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