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
	public class InjectExample : MonoBehaviour
	{
        [Inject] public A AObj;

        // Use this for initialization
        void Start()
        {
            var container = new IOCContainer();
            container.RegisterInstance(new A());
            container.Inject(this);

            AObj.HelloWorld();
        }

    }
    public class A
    {
        public void HelloWorld()
        {
            Debug.Log("This is A obj");
        }
    }
}