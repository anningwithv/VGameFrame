//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VGameFramework
{
	public class FrameworkMgr : TMonoSingleton<FrameworkMgr>
	{
        private void Awake()
        {
            Init();   
        }

        private void Init()
        {
            ResMgr.Instance.OnInit();
            UIMgr.Instance.OnInit();
            XLuaMgr.Instance.OnInit();
        }

        private void Update()
        {
            ResMgr.Instance.OnUpdate();
        }
    }
	
}