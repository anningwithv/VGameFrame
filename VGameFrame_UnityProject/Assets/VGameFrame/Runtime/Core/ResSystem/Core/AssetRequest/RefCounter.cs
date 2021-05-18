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
	public class RefCounter
	{
        public int refCount;

        private List<Object> m_Requires;

        public bool IsUnused()
        {
            if (m_Requires != null)
            {
                for (var i = 0; i < m_Requires.Count; i++)
                {
                    var item = m_Requires[i];
                    if (item != null)
                        continue;
                    Release();
                    m_Requires.RemoveAt(i);
                    i--;
                }
                if (m_Requires.Count == 0)
                    m_Requires = null;
            }
            return refCount <= 0;
        }

        public virtual void Retain()
        {
            refCount++;
        }

        public virtual void Release()
        {
            refCount--;
        }

        public void Require(Object obj)
        {
            if (m_Requires == null)
                m_Requires = new List<Object>();

            m_Requires.Add(obj);
            Retain();
        }

        public void Dequire(Object obj)
        {
            if (m_Requires == null)
                return;

            if (m_Requires.Remove(obj))
                Release();
        }
        private bool checkRequires
        {
            get { return m_Requires != null; }
        }

    }
	
}