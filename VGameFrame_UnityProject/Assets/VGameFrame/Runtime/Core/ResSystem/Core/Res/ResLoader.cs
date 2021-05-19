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
                        throw new Exception(string.Format("�벻Ҫ���첽������Դ {0} ʱ������ {0} ��ͬ������", res.Name));
                    case ResState.Loaded:
                        return res.Asset as T;
                }
            }

            // ����������Դ
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
            // ��ѯ��ǰ�� ��Դ��¼
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

            // ����������Դ
            res = CreateRes(resType, name);

            res.RegisterOnLoadedEvent(onResLoaded);

            res.LoadAsync();
        }


        private List<Res> m_ResRecord = new List<Res>();

        private Res GetRes(string assetName)
        {
            // ��ѯ��ǰ�� ��Դ��¼
            var res = GetResFromRecord(assetName);

            if (res != null)
            {
                return res;
            }

            // ��ѯȫ����Դ��
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