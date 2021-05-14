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
    [CreateAssetMenu(menuName = "VGameFrame/Create EngineConfig ")]
    public class EngineConfig : ScriptableObject
	{
        public string url = "http://172.16.1.170/DLC/";
	}
	
}