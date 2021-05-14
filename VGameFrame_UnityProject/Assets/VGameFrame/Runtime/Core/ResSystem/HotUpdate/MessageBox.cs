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
	public class MessageBox : CustomYieldInstruction
	{
        public override bool keepWaiting => m_KeepWaiting;

        private bool m_KeepWaiting = true;

        // Start is called before the first frame update
        public MessageBox(string title, string content, string ok = "确定", string no = "取消")
        {
            m_KeepWaiting = false;
        }
    }
	
}