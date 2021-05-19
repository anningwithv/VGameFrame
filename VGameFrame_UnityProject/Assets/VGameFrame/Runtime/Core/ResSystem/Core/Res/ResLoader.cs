//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VGameFrame
{
	public class ResLoader
	{
        #region API		
        //public T LoadSync<T>(string assetBundleName, string assetName) where T : Object
        //{
        //    return DoLoadSync<T>(assetName, assetBundleName);
        //}

        public T LoadSync<T>(ResType resType, string name) where T : Object
        {
            return DoLoadSync<T>(resType, name);

        }

        public void LoadAsync<T>(ResType resType, string name, Action<T> onLoaded) where T : Object
        {
            DoLoadAsync(resType, name, onLoaded);
        }

        //public void LoadAsync<T>(string assetBundleName, string assetName, Action<T> onLoaded) where T : Object
        //{
        //    DoLoadAsync(assetName, assetBundleName, onLoaded);
        //}

        public void ReleaseAll()
        {
            m_ResRecord.ForEach(loadedAsset => loadedAsset.Release());

            m_ResRecord.Clear();
        }


        #endregion


        #region Private		
        private T DoLoadSync<T>(ResType resType, string name) where T : Object
        {
            var res = GetRes(name);

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
            // 查询当前的 资源记录
            var res = GetRes(name);

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


        private List<Res> m_ResRecord = new List<Res>();

        private Res GetRes(string assetName)
        {
            // 查询当前的 资源记录
            var res = GetResFromRecord(assetName);

            if (res != null)
            {
                return res;
            }

            // 查询全局资源池
            res = GetFromResMgr(assetName);

            if (res != null)
            {
                AddRes2Record(res);

                return res;
            }

            return res;
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
            //return ResMgr.Instance.SharedLoadedReses.Find(loadedAsset => loadedAsset.Path == resPath);
            if(ResMgr.Instance.LoadedAssets.ContainsKey(assetName))
                return ResMgr.Instance.LoadedAssets[assetName];

            return null;
        }

        private void AddRes2Record(Res resFromResMgr)
        {
            m_ResRecord.Add(resFromResMgr);

            resFromResMgr.Retain();
        }

        #endregion
    }

}