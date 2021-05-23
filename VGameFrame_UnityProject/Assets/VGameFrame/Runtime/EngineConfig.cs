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
    [CreateAssetMenu(menuName = "VGameFramework/Create EngineConfig ")]
    public class EngineConfig : ScriptableObject
	{
        public string url = "http://192.168.3.87/DLC/";
	}
	
}