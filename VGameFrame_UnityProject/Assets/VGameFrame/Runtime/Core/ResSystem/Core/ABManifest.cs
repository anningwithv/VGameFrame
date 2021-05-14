using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGameFrame
{
    [Serializable]
    public class AssetRef
    {
        public string name;
        public int bundle;
        public int dir;
    }

    [Serializable]
    public class BundleRef
    {
        public string name;
        public int id;
        public int[] deps;
        public long len;
        public string hash;
    }

    public class ABManifest : ScriptableObject
    {
        public string[] activeVariants = new string[0];
        public string[] dirs = new string[0];
        public AssetRef[] assets = new AssetRef[0];
        public BundleRef[] bundles = new BundleRef[0];
    }
}