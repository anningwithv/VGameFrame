//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace VGameFrame
{
	public class BundleAssetRequestAsync : BundleAssetRequest
    {
        private AssetBundleRequest m_Request;

        public BundleAssetRequestAsync(string bundle)
            : base(bundle)
        {
        }

        public override float progress
        {
            get
            {
                if (isDone) return 1;

                if (loadState == LoadState.Init) return 0;

                if (m_Request != null) return m_Request.progress * 0.7f + 0.3f;

                if (BundleRequest == null) return 1;

                var value = BundleRequest.progress;
                var max = children.Count;
                if (max <= 0)
                    return value * 0.3f;

                for (var i = 0; i < max; i++)
                {
                    var item = children[i];
                    value += item.progress;
                }

                return value / (max + 1) * 0.3f;
            }
        }

        private bool OnError(BundleRequest bundleRequest)
        {
            error = bundleRequest.error;
            if (!string.IsNullOrEmpty(error))
            {
                loadState = LoadState.Loaded;
                return true;
            }

            return false;
        }

        internal override bool Update()
        {
            if (!base.Update()) return false;

            if (loadState == LoadState.Init) return true;

            if (m_Request == null)
            {
                if (!BundleRequest.isDone) return true;
                if (OnError(BundleRequest)) return false;

                for (var i = 0; i < children.Count; i++)
                {
                    var item = children[i];
                    if (!item.isDone) return true;
                    if (OnError(item)) return false;
                }

                var assetName = Path.GetFileName(name);
                m_Request = BundleRequest.assetBundle.LoadAssetAsync(assetName, assetType);
                if (m_Request == null)
                {
                    error = "request == null";
                    loadState = LoadState.Loaded;
                    return false;
                }

                return true;
            }

            if (m_Request.isDone)
            {
                asset = m_Request.asset;
                loadState = LoadState.Loaded;
                if (asset == null)
                    error = "asset == null";

                return false;
            }

            return true;
        }

        internal override void Load()
        {
            BundleRequest = ABResMgr.LoadBundleAsync(assetBundleName);
            var bundles = ABResMgr.GetAllDependencies(assetBundleName);
            foreach (var item in bundles)
                children.Add(ABResMgr.LoadBundleAsync(item));

            loadState = LoadState.LoadAssetBundle;
        }

        internal override void Unload()
        {
            m_Request = null;
            loadState = LoadState.Unload;
            base.Unload();
        }

        internal override void LoadImmediate()
        {
            BundleRequest.LoadImmediate();
            foreach (var item in children) item.LoadImmediate();
            if (BundleRequest.assetBundle != null)
            {
                var assetName = Path.GetFileName(name);
                asset = BundleRequest.assetBundle.LoadAsset(assetName, assetType);
            }

            loadState = LoadState.Loaded;
            if (asset == null) error = "asset == null";
        }
    }
	
}