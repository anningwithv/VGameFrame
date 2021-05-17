//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VGameFrame
{
	public class BundleRequestAsync : BundleRequest
    {
        private AssetBundleCreateRequest _request;

        public override float progress
        {
            get
            {
                if (isDone) return 1;
                if (loadState == LoadState.Init) return 0;
                if (_request == null) return 1;
                return _request.progress;
            }
        }

        internal override bool Update()
        {
            if (!base.Update()) return false;

            if (loadState == LoadState.LoadAsset)
                if (_request.isDone)
                {
                    assetBundle = _request.assetBundle;
                    if (assetBundle == null) error = string.Format("unable to load assetBundle:{0}", name);
                    loadState = LoadState.Loaded;
                    return false;
                }

            return true;
        }

        internal override void Load()
        {
            if (_request == null)
            {
                _request = AssetBundle.LoadFromFileAsync(name);
                if (_request == null)
                {
                    error = name + " LoadFromFile failed.";
                    return;
                }

                loadState = LoadState.LoadAsset;
            }
        }

        internal override void Unload()
        {
            _request = null;
            loadState = LoadState.Unload;
            base.Unload();
        }

        internal override void LoadImmediate()
        {
            Load();
            assetBundle = _request.assetBundle;
            if (assetBundle != null) Debug.LogWarning("LoadImmediate:" + assetBundle.name);
            loadState = LoadState.Loaded;
        }
    }
	
}