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


namespace VGameFrame
{
    public enum ResState
    {
        Waiting,
        Loading,
        Loaded,
    }

    public enum ResType
    {
        ResourceAsset,
        Bundle,
        BundleAsset,
    }

    public abstract class Res : RefCounter2
	{
        public ResState State
        {
            get { return m_State; }
            protected set
            {
                m_State = value;

                if (m_State == ResState.Loaded)
                {
                    if (m_OnLoadedEvent != null)
                    {
                        m_OnLoadedEvent.Invoke(this);
                    }
                }
            }
        }

        private ResState m_State;

        public UnityEngine.Object Asset { get; protected set; }
        public string Name { get; protected set; }

        public abstract bool LoadSync();

        public abstract void LoadAsync();

        protected abstract void OnReleaseRes();

        protected override void OnZeroRef()
        {
            OnReleaseRes();
        }

        private event Action<Res> m_OnLoadedEvent;

        public void RegisterOnLoadedEvent(Action<Res> onLoaded)
        {
            m_OnLoadedEvent += onLoaded;
        }

        public void UnRegisterOnLoadedEvent(Action<Res> onLoaded)
        {
            m_OnLoadedEvent -= onLoaded;
        }
    }
	
}