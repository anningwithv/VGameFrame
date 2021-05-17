using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGameFrame
{
    public class ABResMgr : TSingleton<ABResMgr>, IElement
    {
        public static readonly string ManifestAsset = "Assets/Manifest.asset";
        public static readonly string Extension = ".unity3d";

        public static bool runtimeMode;
        #region IElement
        public void OnInit()
        {
        }

        public void OnUpdate()
        {
        }
        public void OnDestroy()
        {
        }
        #endregion

        public void LoadAssetSync(string resName)
        {

        }
    }
}