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
    public abstract class UIPanel : MonoBehaviour, IPanel
    {
        #region IPanel
        public void OnClose()
        {
        }

        public void OnInit()
        {
        }

        public void OnOpen()
        {
        }
        #endregion

    }

}