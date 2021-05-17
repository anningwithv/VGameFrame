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
	public class ManifestRequest : AssetRequest
    {
        private string assetName = "ABManifest.asset";
        private BundleRequest request;

        public int version { get; private set; }

        public override float progress
        {
            get
            {
                if (isDone) return 1;

                if (loadState == LoadState.Init) return 0;

                if (request == null) return 1;

                return request.progress;
            }
        }

        internal override void Load()
        {
            //assetName = Path.GetFileName(name);
            if (ABResMgr.runtimeMode)
            {
                var assetBundleName = assetName.Replace(".asset", ".unity3d")/*.ToLower()*/;
                request = ABResMgr.LoadBundleAsync(assetBundleName);
                loadState = LoadState.LoadAssetBundle;
            }
            else
            {
                loadState = LoadState.Loaded;
            }
        }

        internal override bool Update()
        {
            if (!base.Update()) return false;

            if (loadState == LoadState.Init) return true;

            if (request == null)
            {
                loadState = LoadState.Loaded;
                error = "request == null";
                return false;
            }

            if (request.isDone)
            {
                if (request.assetBundle == null)
                {
                    error = "assetBundle == null";
                }
                else
                {
                    var manifest = request.assetBundle.LoadAsset<ABManifest>(assetName);
                    if (manifest == null)
                        error = "manifest == null";
                    else
                        ABResMgr.OnLoadManifest(manifest);
                }

                loadState = LoadState.Loaded;
                return false;
            }

            return true;
        }

        internal override void Unload()
        {
            if (request != null)
            {
                request.Release();
                request = null;
            }

            loadState = LoadState.Unload;
        }
    }
	
}