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
	public class ResFactory
	{
        public static Res Create(ResType resType, string name)
        {
            Res res = null;

            //if (assetBundleName != null)
            //{
            //    res = new AssetRes(assetName, assetBundleName);
            //}
            //else if (assetName.StartsWith("resources://"))
            //{
            //    res = new ResourcesRes(assetName);
            //}
            //else
            switch (resType)
            {
                case ResType.BundleAsset:
                    string bundleName;
                    if (ResMgr.Instance.GetAssetBundleName(name, out bundleName))
                        res = new BundleAssetRes(name, bundleName);
                    break;
                case ResType.Bundle:
                    res = new BundleRes(name);
                    break;
            }

            return res;
        }
    }
	
}