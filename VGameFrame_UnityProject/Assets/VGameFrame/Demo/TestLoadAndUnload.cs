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
	public class TestLoadAndUnload : MonoBehaviour
	{
        ResLoader resLoader = null;
        // Start is called before the first frame update
        void Start()
	    {
            ResMgr.Instance.LoadManifest();
            resLoader = new ResLoader();
            resLoader.LoadAsync<GameObject>(ResType.BundleAsset, "MainMenuPanel.prefab", mainMenuPrefab => {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    GameObject go = GameObject.Instantiate(mainMenuPrefab);
                    go.transform.SetParent(canvas.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localScale = new Vector3(1, 1, 1);
                }
            });
        }
	
	    // Update is called once per frame
	    void Update()
	    {
            if (Input.GetKeyDown(KeyCode.T))
            {
                resLoader.ReleaseAsset("MainMenuPanel.prefab");
            }
	    }
	}
	
}