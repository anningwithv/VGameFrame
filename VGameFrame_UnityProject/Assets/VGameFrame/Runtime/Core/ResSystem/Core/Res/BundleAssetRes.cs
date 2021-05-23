//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace VGameFrame
{
    public class BundleAssetRes : Res
    {
        private AssetBundle m_AssetBundle;
        private string m_OwnerBundleName;
        private List<string> m_AllDependencyBundles = new List<string>();

        public BundleAssetRes(string assetName, string ownerBundleName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("invalid path");
            }

            Name = assetName;
            m_OwnerBundleName = ownerBundleName;

            State = ResState.Waiting;

            ResType = ResType.BundleAsset;

            m_AllDependencyBundles.Clear();
            m_AllDependencyBundles.Add(ownerBundleName);
        }

        private ResLoader m_ResLoader = new ResLoader();

        public override bool LoadSync()
        {
            State = ResState.Loading;

            if (ResMgr.Instance.runtimeMode)
            {
                //Load owner bundle
                m_AssetBundle = m_ResLoader.LoadSync<AssetBundle>(ResType.Bundle, m_OwnerBundleName);

                string[] dependencyBundleNames = ResMgr.Instance.GetAllDependencies(m_OwnerBundleName);

                m_AllDependencyBundles.AddRange(dependencyBundleNames);

                //Load dependency bundles of owner bundle
                foreach (var dependencyBundleName in dependencyBundleNames)
                {
                    m_ResLoader.LoadSync<AssetBundle>(ResType.Bundle, dependencyBundleName);
                }
                
            }

            Asset = m_AssetBundle.LoadAsset<UnityEngine.Object>(Name);

            State = ResState.Loaded;

            return Asset != null;
        }

        private void LoadDependencyBundlesAsync(Action onAllLoaded)
        {
            string[] dependencyBundleNames = ResMgr.Instance.GetAllDependencies(m_OwnerBundleName);

            m_AllDependencyBundles.AddRange(dependencyBundleNames);

            var loadedCount = 0;

            if (dependencyBundleNames.Length == 0)
            {
                if (onAllLoaded != null)
                    onAllLoaded.Invoke();
            }

            foreach (var dependencyBundleName in dependencyBundleNames)
            {
                m_ResLoader.LoadAsync<AssetBundle>(ResType.Bundle, dependencyBundleName,
                    dependBundle =>
                    {
                        loadedCount++;

                        if (loadedCount == dependencyBundleNames.Length)
                        {
                            if (onAllLoaded != null)
                                onAllLoaded.Invoke();
                        }
                    });
            }
        }

        public override void LoadAsync()
        {
            State = ResState.Loading;

            LoadDependencyBundlesAsync(() =>
            {
                if (!ResMgr.Instance.runtimeMode)
                {
                    State = ResState.Loaded;
                }
                else
                {
                    m_ResLoader.LoadAsync<AssetBundle>(ResType.Bundle, m_OwnerBundleName,
                        dependBundle =>
                        {
                            m_AssetBundle = dependBundle;
                            Asset = m_AssetBundle.LoadAsset<UnityEngine.Object>(Name);

                            State = ResState.Loaded;
                        });
                }
            });
        }

        protected override void OnZeroRef()
        {
            base.OnZeroRef();

            if (m_AllDependencyBundles.Count > 0)
            {
                m_AllDependencyBundles.ForEach(i => 
                {
                    Res res = ResMgr.Instance.GetRes(i);
                    if (res != null)
                    {
                        res.SubRef();
                    }
                });
            }
        }

        protected override void OnReleaseRes()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;

                Asset = null;

                m_ResLoader.ReleaseAll();
                m_ResLoader = null;
            }

            //ResMgr.Instance.LoadedAssets.Remove(Name);
        }
    }

}