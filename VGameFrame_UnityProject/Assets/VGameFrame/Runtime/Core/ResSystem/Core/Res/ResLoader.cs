//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VGameFramework
{
	public class ResLoader
	{
        private List<Res> m_ResRecord = new List<Res>();

        #region API		
        public T LoadSync<T>(ResType resType, string name) where T : Object
        {
            return DoLoadSync<T>(resType, name);

        }

        public void LoadAsync<T>(ResType resType, string name, Action<T> onLoaded) where T : Object
        {
            DoLoadAsync(resType, name, onLoaded);
        }

        public void ReleaseAsset(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var res = GetFromResMgr(name);
            if (res == null)
            {
                return;
            }

            if (m_ResRecord.Remove(res))
            {
                res.SubRef();
            }
        }

        public void ReleaseAll()
        {
            if (m_ResRecord.Count > 0)
            {
                m_ResRecord.ForEach(loadedAsset =>
                {
                    loadedAsset.SubRef();
                });

                m_ResRecord.Clear();

                ResMgr.Instance.SetResMapDirty();
            }
        }

        #endregion


        #region Private		
        private T DoLoadSync<T>(ResType resType, string name) where T : Object
        {
            var res = GetResFromRecord(name);

            if (res == null)
            {
                res = GetFromResMgr(name);

                if (res != null)
                {
                    AddRes2Record(res);
                }
            }

            if (res != null)
            {
                switch (res.State)
                {
                    case ResState.Loading:
                        throw new Exception(string.Format("请不要在异步加载资源 {0} 时，进行 {0} 的同步加载", res.Name));
                    case ResState.Loaded:
                        return res.Asset as T;
                }
            }

            // 真正加载资源
            res = CreateRes(resType, name);

            res.LoadSync();

            if ( res.Asset == null)
            {
                Debug.LogError("Res load faild: " + res.Name);
            }

            return res.Asset as T;
        }

        private void DoLoadAsync<T>(ResType resType, string name, Action<T> onLoaded) where T : Object
        {
            var res = GetResFromRecord(name);

            if (res == null)
            {
                res = GetFromResMgr(name);

                if (res != null)
                {
                    AddRes2Record(res);
                }
            }

            Action<Res> onResLoaded = null;
            onResLoaded = loadedRes =>
            {
                onLoaded(loadedRes.Asset as T);
                res.UnRegisterOnLoadedEvent(onResLoaded);
            };

            if (res != null)
            {
                if (res.State == ResState.Loading)
                {
                    res.RegisterOnLoadedEvent(onResLoaded);
                }
                else if (res.State == ResState.Loaded)
                {
                    onLoaded(res.Asset as T);
                }

                return;
            }

            // 真正加载资源
            res = CreateRes(resType, name);

            res.RegisterOnLoadedEvent(onResLoaded);

            res.LoadAsync();
        }

        private Res CreateRes(ResType resType, string name)
        {
            var res = ResFactory.Create(resType, name);

            ResMgr.Instance.LoadedAssets.Add(name, res);

            AddRes2Record(res);

            return res;
        }

        private Res GetResFromRecord(string assetName)
        {
            return m_ResRecord.Find(loadedAsset => loadedAsset.Name == assetName);
        }

        private Res GetFromResMgr(string assetName)
        {
            if (ResMgr.Instance.LoadedAssets.ContainsKey(assetName))
            {
                Res res = ResMgr.Instance.LoadedAssets[assetName];

                return res;
            }

            return null;
        }

        private void AddRes2Record(Res res)
        {
            m_ResRecord.Add(res);

            res.Retain();
        }

        #endregion
    }

}