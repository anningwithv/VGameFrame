//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VGameFrame
{
    public enum LoadState
    {
        Init,
        LoadAssetBundle,
        LoadAsset,
        Loaded,
        Unload
    }

    public class AssetRequest : RefCounter, IEnumerator
    {
        private LoadState _loadState = LoadState.Init;
        private List<Object> _requires;
        public Type assetType;

        public Action<AssetRequest> completed;
        public string name;

        public AssetRequest()
        {
            asset = null;
            loadState = LoadState.Init;
        }

        public LoadState loadState
        {
            get { return _loadState; }
            protected set
            {
                _loadState = value;
                if (value == LoadState.Loaded)
                {
                    Complete();
                }
            }
        }

        private void Complete()
        {
            if (completed != null)
            {
                completed(this);
                completed = null;
            }
        }

        public virtual bool isDone
        {
            get { return loadState == LoadState.Loaded || loadState == LoadState.Unload; }
        }

        public virtual float progress
        {
            get { return 1; }
        }

        public virtual string error { get; protected set; }

        public string text { get; protected set; }

        public byte[] bytes { get; protected set; }

        public Object asset { get; internal set; }

        private bool checkRequires
        {
            get { return _requires != null; }
        }

        private void UpdateRequires()
        {
            for (var i = 0; i < _requires.Count; i++)
            {
                var item = _requires[i];
                if (item != null)
                    continue;
                Release();
                _requires.RemoveAt(i);
                i--;
            }

            if (_requires.Count == 0)
                _requires = null;
        }

        internal virtual void Load()
        {
            //if (!ABResMgr.runtimeMode /*&& ABResMgr.loadDelegate != null*/)
            //    asset = AssetDatabase.LoadAssetAtPath(name, assetType);
            if (asset == null) error = "error! file not exist:" + name;

            loadState = LoadState.Loaded;
        }

        internal virtual void Unload()
        {
            if (asset == null)
                return;

            if (!ABResMgr.runtimeMode)
                if (!(asset is GameObject))
                    Resources.UnloadAsset(asset);

            asset = null;
            loadState = LoadState.Unload;
        }

        internal virtual bool Update()
        {
            if (checkRequires)
                UpdateRequires();
            if (!isDone)
                return true;
            //if (completed == null)
            //    return false;
            try
            {
                completed?.Invoke(this);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            completed = null;
            return false;
        }

        internal virtual void LoadImmediate()
        {
        }

        #region IEnumerator implementation

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { return null; }
        }

        #endregion
    }

}