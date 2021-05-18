using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VGameFrame
{
    public class ABResMgr : TSingleton<ABResMgr>, IElement
    {
        public static readonly string ManifestAsset = "Assets/Manifest.asset";
        public static readonly string Extension = ".unity3d";

        public static bool runtimeMode = true;

        public static string basePath { get; set; }
        public static string updatePath { get; set; }

        //private static readonly int MAX_BUNDLES_PERFRAME = 0;
        private static Dictionary<string, BundleRequest> _bundles = new Dictionary<string, BundleRequest>();
        private static List<BundleRequest> _loadingBundles = new List<BundleRequest>();
        private static List<BundleRequest> _unusedBundles = new List<BundleRequest>();
        //private static List<BundleRequest> _toloadBundles = new List<BundleRequest>();
        private static List<string> _activeVariants = new List<string>();
        private static Dictionary<string, string> _assetToBundles = new Dictionary<string, string>();
        private static Dictionary<string, string[]> _bundleToDependencies = new Dictionary<string, string[]>();

        private static Dictionary<string, AssetRequest> m_LoadedAssets = new Dictionary<string, AssetRequest>();
        private static List<AssetRequest> _loadingAssets = new List<AssetRequest>();
        private static List<AssetRequest> _unusedAssets = new List<AssetRequest>();
        //private static List<SceneAssetRequest> _scenes = new List<SceneAssetRequest>();

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
            UpdateAssets();
            UpdateBundles();
        }

        public void OnDestroy()
        {
        }

        #endregion

        public void LoadAssetSync(string resName)
        {

        }

        public static ManifestRequest LoadManifest()
        {
            ManifestRequest request = new ManifestRequest { name = ManifestAsset };
            AddAssetRequest(request);

            return request;
        }

        internal static void OnLoadManifest(ABManifest manifest)
        {
            _activeVariants.AddRange(manifest.activeVariants);

            var assets = manifest.assets;
            var dirs = manifest.dirs;
            var bundles = manifest.bundles;

            foreach (var item in bundles)
                _bundleToDependencies[item.name] = Array.ConvertAll(item.deps, id => bundles[id].name);

            foreach (var item in assets)
            {
                var path = string.Format("{0}/{1}", dirs[item.dir], item.name).ToLower();
                if (item.bundle >= 0 && item.bundle < bundles.Length)
                {
                    _assetToBundles[path] = bundles[item.bundle].name;
                }
                else
                {
                    Debug.LogError(string.Format("{0} bundle {1} not exist.", path, item.bundle));
                }
            }
        }

        public static AssetRequest LoadAssetAsync(string path, Type type)
        {
            return LoadAsset(path, type, true);
        }

        public static AssetRequest LoadAsset(string path, Type type)
        {
            return LoadAsset(path, type, false);
        }

        public static void UnloadAsset(AssetRequest asset)
        {
            asset.Release();
        }

        internal static BundleRequest LoadBundle(string assetBundleName)
        {
            return LoadBundle(assetBundleName, false);
        }

        internal static BundleRequest LoadBundleAsync(string assetBundleName)
        {
            return LoadBundle(assetBundleName, true);
        }


        internal static BundleRequest LoadBundle(string assetBundleName, bool asyncMode)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                Debug.LogError("assetBundleName == null");
                return null;
            }

            assetBundleName = RemapVariantName(assetBundleName);
            var url = GetDataPath(assetBundleName) + assetBundleName;

            BundleRequest bundle;

            if (_bundles.TryGetValue(url, out bundle))
            {
                bundle.Retain();
                _loadingBundles.Add(bundle);
                return bundle;
            }

            //if (url.StartsWith("http://", StringComparison.Ordinal) ||
            //    url.StartsWith("https://", StringComparison.Ordinal) ||
            //    url.StartsWith("file://", StringComparison.Ordinal) ||
            //    url.StartsWith("ftp://", StringComparison.Ordinal))
            //    bundle = new WebBundleRequest();
            //else
                bundle = asyncMode ? new BundleRequestAsync() : new BundleRequest();

            bundle.name = url;
            _bundles.Add(url, bundle);

            //if (MAX_BUNDLES_PERFRAME > 0 && (bundle is BundleRequestAsync || bundle is WebBundleRequest))
            //{
            //    _toloadBundles.Add(bundle);
            //}
            //else
            {
                bundle.Load();
                _loadingBundles.Add(bundle);
                Debug.Log("LoadBundle: " + url);
            }

            bundle.Retain();
            return bundle;
        }
        internal static void UnloadBundle(BundleRequest bundle)
        {
            bundle.Release();
        }

        internal static string[] GetAllDependencies(string bundle)
        {
            string[] deps;
            if (_bundleToDependencies.TryGetValue(bundle, out deps))
                return deps;

            return new string[0];
        }

        private static void AddAssetRequest(AssetRequest request)
        {
            m_LoadedAssets.Add(request.name, request);
            _loadingAssets.Add(request);
            request.Load();
        }

        private static void UpdateAssets()
        {
            for (var i = 0; i < _loadingAssets.Count; ++i)
            {
                var request = _loadingAssets[i];
                if (request.Update())
                    continue;
                _loadingAssets.RemoveAt(i);
                --i;
            }

            foreach (var item in m_LoadedAssets)
            {
                if (item.Value.isDone && item.Value.IsUnused())
                {
                    _unusedAssets.Add(item.Value);
                }
            }

            if (_unusedAssets.Count > 0)
            {
                for (var i = 0; i < _unusedAssets.Count; ++i)
                {
                    var request = _unusedAssets[i];
                    Debug.Log(string.Format("UnloadAsset:{0}", request.name));
                    m_LoadedAssets.Remove(request.name);
                    request.Unload();
                }
                _unusedAssets.Clear();
            }

            //for (var i = 0; i < _scenes.Count; ++i)
            //{
            //    var request = _scenes[i];
            //    if (request.Update() || !request.IsUnused())
            //        continue;
            //    _scenes.RemoveAt(i);
            //    Log(string.Format("UnloadScene:{0}", request.name));
            //    request.Unload();
            //    --i;
            //}
        }

        private static void UpdateBundles()
        {
            //var max = MAX_BUNDLES_PERFRAME;
            //if (_toloadBundles.Count > 0 && max > 0 && _loadingBundles.Count < max)
            //    for (var i = 0; i < Math.Min(max - _loadingBundles.Count, _toloadBundles.Count); ++i)
            //    {
            //        var item = _toloadBundles[i];
            //        if (item.loadState == LoadState.Init)
            //        {
            //            item.Load();
            //            _loadingBundles.Add(item);
            //            _toloadBundles.RemoveAt(i);
            //            --i;
            //        }
            //    }

            for (var i = 0; i < _loadingBundles.Count; i++)
            {
                var item = _loadingBundles[i];
                if (item.Update())
                    continue;
                _loadingBundles.RemoveAt(i);
                --i;
            }

            foreach (var item in _bundles)
            {
                if (item.Value.isDone && item.Value.IsUnused())
                {
                    _unusedBundles.Add(item.Value);
                }
            }

            if (_unusedBundles.Count <= 0) return;
            {
                for (var i = 0; i < _unusedBundles.Count; i++)
                {
                    var item = _unusedBundles[i];
                    if (item.isDone)
                    {
                        item.Unload();
                        _bundles.Remove(item.name);
                        Debug.Log("UnloadBundle: " + item.name);
                    }
                }
                _unusedBundles.Clear();
            }
        }

        private static AssetRequest LoadAsset(string path, Type type, bool async)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("invalid path");
                return null;
            }

            path = GetExistPath(path);

            AssetRequest request;
            if (m_LoadedAssets.TryGetValue(path, out request))
            {
                request.Retain();
                _loadingAssets.Add(request);
                return request;
            }

            string assetBundleName;
            if (GetAssetBundleName(path, out assetBundleName))
            {
                request = async
                    ? new BundleAssetRequestAsync(assetBundleName)
                    : new BundleAssetRequest(assetBundleName);
            }
            else
            {
                //if (path.StartsWith("http://", StringComparison.Ordinal) ||
                //    path.StartsWith("https://", StringComparison.Ordinal) ||
                //    path.StartsWith("file://", StringComparison.Ordinal) ||
                //    path.StartsWith("ftp://", StringComparison.Ordinal) ||
                //    path.StartsWith("jar:file://", StringComparison.Ordinal))
                //    request = new WebAssetRequest();
                //else
                    request = new AssetRequest();
            }

            request.name = path;
            request.assetType = type;
            AddAssetRequest(request);
            request.Retain();
            Debug.Log(string.Format("LoadAsset:{0}", path));
            return request;
        }

        private static bool GetAssetBundleName(string path, out string assetBundleName)
        {
            return _assetToBundles.TryGetValue(path, out assetBundleName);
        }


        private static string GetDataPath(string bundleName)
        {
            if (string.IsNullOrEmpty(updatePath))
                return basePath;

            if (File.Exists(updatePath + bundleName))
                return updatePath;

            return basePath;
        }

        private static string RemapVariantName(string assetBundleName)
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

        private static List<string> _searchPaths = new List<string>();

        private static string GetExistPath(string path)
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
            if (_assetToBundles.ContainsKey(path))
                return path;

            foreach (var item in _searchPaths)
            {
                var existPath = string.Format("{0}/{1}", item, path);
                if (_assetToBundles.ContainsKey(existPath))
                    return existPath;
            }

            Debug.LogError("资源没有收集打包" + path);
            return path;
        }

    }
}