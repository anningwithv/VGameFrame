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
	public class GameMgr : TMonoSingleton<GameMgr>
	{
        private void Awake()
        {
            Init();   
        }

        private void Init()
        {
            ResMgr.Instance.OnInit();
        }

        private void Update()
        {
            ResMgr.Instance.OnUpdate();
        }
    }
	
}