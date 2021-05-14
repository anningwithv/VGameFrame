using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGameFrame
{
    public class MonoSingleton : MonoBehaviour
    {
        private static bool m_IsApplicationQuit = false;

        public static bool isApplicationQuit
        {
            get { return m_IsApplicationQuit; }
            set { m_IsApplicationQuit = value; }
        }

        public static K CreateMonoSingleton<K>() where K : MonoBehaviour, ISingleton
        {
            if (m_IsApplicationQuit)
            {
                return null;
            }

            K instance = null;

            if (instance == null && !m_IsApplicationQuit)
            {
                instance = GameObject.FindObjectOfType(typeof(K)) as K;
                if (instance == null)
                {
                    GameObject obj = new GameObject("Singleton of " + typeof(K).Name);
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<K>();
                }

                instance.OnSingletonInit();
            }

            return instance;
        }       
    }
}
