//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFramework. 
//  Author:      V 
//-------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace VGameFramework
{
	public class XLuaMgr : TSingleton<XLuaMgr>, IManager
    {
        private LuaEnv m_LuaEnv = null;

        #region IManager

        public void OnInit()
        {
            m_LuaEnv = new LuaEnv();
            m_LuaEnv.DoString("XLuaTest");
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        #endregion

    }

}