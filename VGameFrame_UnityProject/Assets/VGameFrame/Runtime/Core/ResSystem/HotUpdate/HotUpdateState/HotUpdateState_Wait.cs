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
    public class HotUpdateState_Wait : FSMState<HotUpdateStateId>
    {
        public HotUpdateState_Wait(FSMStateMachine<HotUpdateStateId> stateMachine) : base(stateMachine)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            SetCurState(HotUpdateStateId.CopingVersionsFromLocal);
        }
    }

}