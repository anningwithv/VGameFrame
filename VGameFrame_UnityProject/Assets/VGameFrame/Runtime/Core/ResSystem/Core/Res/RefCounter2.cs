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
    public class RefCounter2 : IRefCounter
    {
        public int RefCount { get; private set; }

        public void Retain(object refOwner = null)
        {
            RefCount++;
        }

        public void Release(object refOwner = null)
        {
            RefCount--;

            if (RefCount == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {

        }
    }

}