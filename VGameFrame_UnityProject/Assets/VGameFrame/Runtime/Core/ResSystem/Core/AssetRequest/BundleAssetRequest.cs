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
	public class BundleAssetRequest : AssetRequest
    {
        protected readonly string assetBundleName;
        protected BundleRequest BundleRequest;
        protected List<BundleRequest> children = new List<BundleRequest>();

        public BundleAssetRequest(string bundle)
        {
            assetBundleName = bundle;
        }

        internal override void Load()
        {
            BundleRequest = ABResMgr.LoadBundle(assetBundleName);
            var names = ABResMgr.GetAllDependencies(assetBundleName);
            foreach (var item in names) children.Add(ABResMgr.LoadBundle(item));
            var assetName = Path.GetFileName(name);
            var ab = BundleRequest.assetBundle;
            if (ab != null) asset = ab.LoadAsset(assetName, assetType);
            if (asset == null) error = "asset == null";
            loadState = LoadState.Loaded;
        }

        internal override void Unload()
        {
            if (BundleRequest != null)
            {
                BundleRequest.Release();
                BundleRequest = null;
            }

            if (children.Count > 0)
            {
                foreach (var item in children) item.Release();
                children.Clear();
            }

            asset = null;
        }
    }
	
}