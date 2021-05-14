//-------------------------------------------------------
//  Desc:        Framework For Game Develop with Unity3d 
//  Copyright:   Copyright (C) 2021. All rights reserved. 
//  Website:     https://github.com/anningwithv/VGameFrame. 
//  Author:      V 
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VGameFrame
{
    public class FSMState<T> : IFSMState<T> where T : IConvertible
    {
        private T m_StateId;
        private FSMStateMachine<T> m_StateMachine;

        public FSMState(FSMStateMachine<T> stateMachine)
        {
            m_StateMachine = stateMachine;
        }

        public T GetStateId()
        {
            return m_StateId;
        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExecute()
        {

        }

        public virtual void OnExit()
        {

        }

        protected void SetCurState(T stateId)
        {
            m_StateMachine.SetCurState(stateId);
        }
    }

}