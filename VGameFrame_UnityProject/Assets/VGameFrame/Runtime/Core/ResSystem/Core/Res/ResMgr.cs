//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace VGameFrame
{
	public class ResMgr : TMonoSingleton<ResMgr>, IElement
    {
        public readonly string ManifestAsset = "Assets/Manifest.asset";
        public readonly string Extension = ".unity3d";

        public bool runtimeMode = true;

        public string basePath { get; set; }
        public string updatePath { get; set; }
        public Dictionary<string, Res> LoadedAssets { get => m_LoadedAssets;}


        private List<string> _activeVariants = new List<string>();
        private Dictionary<string, string> _assetNameToBundles = new Dictionary<string, string>();
        private Dictionary<string, string[]> _bundleToDependencies = new Dictionary<string, string[]>();

        private Dictionary<string, Res> m_LoadedAssets = new Dictionary<string, Res>();

        private ABManifest m_ABManifest = null;

        //public List<Res> SharedLoadedReses = new List<Res>();
        #region IElement
        public void OnInit()
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar;
            }

            if (string.IsNullOrEmpty(updatePath))
            {
                updatePath = Application.persistentDataPath + Path.DirectorySeparatorChar;
            }
        }

        public void OnUpdate()
        {

        }

        public void OnDestroy()
        {
        }

        #endregion

        #region Public
        public void LoadManifest()
        {
            //ManifestRequest request = new ManifestRequest { name = ManifestAsset };
            //AddAssetRequest(request);
            string assetName = "ABManifest.asset";
            if (runtimeMode)
            {
                var assetBundleName = assetName.Replace(".asset", ".unity3d")/*.ToLower()*/;

                assetBundleName = RemapVariantName(assetBundleName);
                var path = GetDataPath(assetBundleName) + assetBundleName;
                AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
                if (assetBundle != null)
                {
                    ABManifest abManifest = assetBundle.LoadAsset<ABManifest>(assetName);
                    if (abManifest != null)
                    {
                        OnLoadManifest(abManifest);
                    }
                }

            }

        }

        public void OnLoadManifest(ABManifest manifest)
        {
            m_ABManifest = manifest;

            _activeVariants.AddRange(manifest.activeVariants);

            var assets = manifest.assets;
            var dirs = manifest.dirs;
            var bundles = manifest.bundles;

            foreach (var item in bundles)
                _bundleToDependencies[item.name] = Array.ConvertAll(item.deps, id => bundles[id].name);

            foreach (var item in assets)
            {
                //var path = string.Format("{0}/{1}", dirs[item.dir], item.name).ToLower();
                if (item.bundle >= 0 && item.bundle < bundles.Length)
                {
                    _assetNameToBundles[item.name] = bundles[item.bundle].name;
                }
                else
                {
                    Debug.LogError(string.Format("{0} bundle {1} not exist.", item.name, item.bundle));
                }
            }
        }

        public bool GetAssetBundleName(string assetName, out string assetBundleName)
        {
            //string assetPath =  GetAssetPath(assetName);
            return _assetNameToBundles.TryGetValue(assetName, out assetBundleName);
        }

        public string[] GetAllDependencies(string bundleName)
        {
            string[] deps;
            if (_bundleToDependencies.TryGetValue(bundleName, out deps))
                return deps;

            return new string[0];
        }
        #endregion

        #region Private

        public string GetDataPath(string bundleName)
        {
            if (string.IsNullOrEmpty(updatePath))
                return basePath;

            if (File.Exists(updatePath + bundleName))
                return updatePath;

            return basePath;
        }

        //private string GetAssetPath(string assetName)
        //{
        //    foreach (var item in m_ABManifest.assets)
        //    {
        //        if (item.name == assetName)
        //        {
        //            var path = string.Format("{0}/{1}", m_ABManifest.dirs[item.dir], item.name).ToLower();
        //            return path;
        //        }
        //    }

        //    return string.Empty;
        //}

        private string RemapVariantName(string assetBundleName)
        {
            var bundlesWithVariant = _activeVariants;
            // Get base bundle path
            var baseName = assetBundleName.Split('.')[0];

            var bestFit = int.MaxValue;
            var bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (var i = 0; i < bundlesWithVariant.Count; i++)
            {
                var curSplit = bundlesWithVariant[i].Split('.');
                var curBaseName = curSplit[0];
                var curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;

                var found = bundlesWithVariant.IndexOf(curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found >= bestFit)
                    continue;
                bestFit = found;
                bestFitIndex = i;
            }

            if (bestFit == int.MaxValue - 1)
                Debug.LogWarning(
                    "Ambiguous asset bundle variant chosen because there was no matching active variant: " +
                    bundlesWithVariant[bestFitIndex]);

            return bestFitIndex != -1 ? bundlesWithVariant[bestFitIndex] : assetBundleName;
        }

        private List<string> _searchPaths = new List<string>();

        public string GetExistPath(string path)
        {
            path = path.ToLower();

#if UNITY_EDITOR
            if (!runtimeMode)
            {
                if (File.Exists(path))
                    return path;

                foreach (var item in _searchPaths)
                {
                    var existPath = string.Format("{0}/{1}", item, path);
                    if (File.Exists(existPath))
                        return existPath;
                }

                Debug.LogError("找不到资源路径" + path);
                return path;
            }
#endif
            if (_assetNameToBundles.ContainsKey(path))
                return path;

            foreach (var item in _searchPaths)
            {
                var existPath = string.Format("{0}/{1}", item, path);
                if (_assetNameToBundles.ContainsKey(existPath))
                    return existPath;
            }

            Debug.LogError("资源没有收集打包" + path);
            return path;
        }
        #endregion

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (Input.GetKey(KeyCode.F1))
            {
                GUILayout.BeginVertical("box");

                LoadedAssets.Values.ToList().ForEach(loadedRes =>
                {
                    GUILayout.Label(string.Format("Name:{0} RefCount:{1} State:{2}", loadedRes.Name,
                        loadedRes.RefCount, loadedRes.State));
                });

                GUILayout.EndVertical();
            }
        }
#endif
    }

}