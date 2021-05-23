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
	public class MessageBox : CustomYieldInstruction
	{
        public override bool keepWaiting => m_KeepWaiting;

        private bool m_KeepWaiting = true;

        // Start is called before the first frame update
        public MessageBox(string title, string content, string ok = "ȷ��", string no = "ȡ��")
        {
            m_KeepWaiting = false;
        }
    }
	
}