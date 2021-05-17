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
	public class FSMStateMachine<T> where T : IConvertible
    {
        private Dictionary<T, IFSMState<T>> m_StateDic = new Dictionary<T, IFSMState<T>>();

        private T m_CurStateId;
        private IFSMState<T> m_CurState;

        public FSMStateMachine()
        {

        }

        public void OnUpdate()
        {
            if (m_CurState != null)
            {
                m_CurState.OnExecute();
            }
        }

        public void RegisterState(T stateID, IFSMState<T> state)
        {
            if (state == null)
            {
                Debug.LogError("StateMachine.RegisterState state is Null!");
                return;
            }

            if (m_StateDic.ContainsKey(stateID))
            {
                Debug.LogError("StateMachine.RegisterState mStateDic hava this key key = " + state.GetStateId());
                return;
            }

            m_StateDic.Add(stateID, state);
        }

        public void RemoveState(T key)
        {
            if (!m_StateDic.ContainsKey(key))
            {
                return;
            }

            if (m_CurState != null && m_CurState.GetStateId().ToInt32(null) == key.ToInt32(null))
            {
                return;
            }

            m_StateDic.Remove(key);
        }

        public IFSMState<T> GetState(T stateId)
        {
            IFSMState<T> state = null;
            m_StateDic.TryGetValue(stateId, out state);

            return state;
        }

        public void SetCurState(T stateId)
        {
            IFSMState<T> state = GetState(stateId);
            if (state != null)
            {
                Debug.Log("Set state to: " + stateId.ToString());

                if (m_CurState != null)
                {
                    m_CurState.OnExit();
                }

                m_CurState = state;
                m_CurStateId = stateId;

                m_CurState.OnEnter();
            }
            else
            {
                Debug.LogError("State not found: " + stateId.ToString());
            }
        }
    }

}