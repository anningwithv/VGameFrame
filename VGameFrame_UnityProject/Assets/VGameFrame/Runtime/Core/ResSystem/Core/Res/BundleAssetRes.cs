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
        //public AssetBundle AssetBundle
        //{
        //    get { return Asset as AssetBundle; }
        //    set { Asset = value; }
        //}
        private AssetBundle m_AssetBundle;
        private string m_OwnerBundleName;

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
        }

        private ResLoader m_ResLoader = new ResLoader();

        public override bool LoadSync()
        {
            State = ResState.Loading;

            if (ResMgr.Instance.runtimeMode)
            {
                m_AssetBundle = m_ResLoader.LoadSync<AssetBundle>(ResType.Bundle, m_OwnerBundleName);

                string[] dependencyBundleNames = ResMgr.Instance.GetAllDependencies(m_OwnerBundleName);

                foreach (var dependencyBundleName in dependencyBundleNames)
                {
                    m_ResLoader.LoadSync<AssetBundle>(ResType.Bundle, dependencyBundleName);
                }
                
            }

            //var url = ResMgr.Instance.GetDataPath(m_OwnerBundleName) + m_OwnerBundleName;
            //AssetBundle = AssetBundle.LoadFromFile(url);
            Asset = m_AssetBundle.LoadAsset<UnityEngine.Object>(Name);

            State = ResState.Loaded;

            return Asset != null;
        }

        private void LoadDependencyBundlesAsync(Action onAllLoaded)
        {
            //if (ResMgr.Instance.GetAssetBundleName(Name, out assetBundleName))
            {
                string[] dependencyBundleNames = ResMgr.Instance.GetAllDependencies(m_OwnerBundleName);
                List<String> dependencyBundleNamesList = dependencyBundleNames.ToList();
                dependencyBundleNamesList.Add(m_OwnerBundleName);

                var loadedCount = 0;

                if (dependencyBundleNamesList.Count == 0)
                {
                    if (onAllLoaded != null)
                        onAllLoaded.Invoke();
                }

                foreach (var dependencyBundleName in dependencyBundleNamesList)
                {
                    m_ResLoader.LoadAsync<AssetBundle>(ResType.Bundle, dependencyBundleName,
                        dependBundle =>
                        {
                            loadedCount++;

                            if (loadedCount == dependencyBundleNamesList.Count)
                            {
                                if (onAllLoaded != null)
                                    onAllLoaded.Invoke();
                            }
                        });
                }
            } 
            //else
            //{
            //    Debug.LogError("Error: Bundle not found, path is: " + Name);
            //}
            //var dependencyBundleNames = ResData.Instance.GetDirectDependencies(Path);

            
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
                    var resRequest = AssetBundle.LoadFromFileAsync(Name);

                    resRequest.completed += operation =>
                    {
                        m_AssetBundle = resRequest.assetBundle;
                        Asset = m_AssetBundle.LoadAsset<UnityEngine.Object>(Name);

                        State = ResState.Loaded;
                    };
                }
            });
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

            ResMgr.Instance.LoadedAssets.Remove(Name);
        }
    }

}