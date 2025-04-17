using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace UnityFramework.FSM
{

    public class StateMachine
    {
        protected object owner;
        private Dictionary<int, State> states = new Dictionary<int, State>();
        private State currentState = null;
        private int defaultID = 0;        

        public int CurrentID => currentState == null ? defaultID : currentState.ID;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">무조건 클래스 타입</param>
        /// <exception cref="System.InvalidOperationException">클래스 타입이 아닐경우 예외 발생함</exception>
        public StateMachine(object owner)
        {
            if (!owner.GetType().IsClass)
                throw new System.InvalidOperationException("클래스 타입이 아닙니다.");
            this.owner = owner;

        }

        public object GetOwner() { return owner; }
        public T GetOwner<T>() where T : class { return owner as T; }

        /// <summary>
        /// 상태 추가
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="state"></param>
        public void AddState(int id, State state)
        {
            if (states.TryAdd(id, state))
                state.SetOwnerMachine(this);
        }


        /// <summary>
        /// 상태 삭제
        /// </summary>
        /// <param name="id"></param>
        public void RemoveState(int id)
        {
            if (states.Remove(id,out State state))
                state.SetOwnerMachine(null);   
        }

        /// <summary>
        /// 기본 상태 세팅
        /// </summary>
        /// <param name="id">기본상태 ID</param>
        public void SetDefaultState(int id)
        {
            defaultID = id;
            Reset();
        }

        /// <summary>
        /// 기본 상태로 전환
        /// </summary>
        public void Reset()
        {
            currentState?.Exit();
            if (!states.TryGetValue(defaultID, out currentState))
                return;
            currentState.Enter();
        }

        /// <summary>
        /// 상태 변환
        /// </summary>
        /// <param name="id"></param>
        public void ChangeState(int id)
        {
            if (CurrentID == id)
                return;

            if (!states.TryGetValue(id, out State nextState))
                return;

            if (!nextState.ConditionChangeID(id))
                return;

            currentState?.Exit();
            currentState = nextState;
            currentState.Enter();
        }

        public void Update()
        {
            currentState.Update();
        }

    }
}

