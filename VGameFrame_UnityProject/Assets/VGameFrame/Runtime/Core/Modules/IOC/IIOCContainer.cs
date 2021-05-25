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


namespace VGameFramework
{
	public interface IIOCContainer : IDisposable
	{
        void RegisterInstance<TBase>(TBase instance);
        void UnRegisterInstance<TBase>();
        void Inject(object obj);
        T Resolve<T>(string name = null, bool requireInstance = false, params object[] args) where T : class;

        void Clear();
    }
	
}