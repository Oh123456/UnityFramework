using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFramework.FSM
{
    public abstract class State
    {
        protected int id;
        protected string name;
        protected StateMachine ownerMachine;
        protected HashSet<int> changeAble = new HashSet<int>();

        public int ID => id;

        public State(int id)
        {
            this.id = id;
            SetChangeAble(changeAble);
        }

        public void SetOwnerMachine(StateMachine stateMachine)
        {
            ownerMachine = stateMachine;    
        }

		protected abstract void SetChangeAble(HashSet<int> changeAble);
		public bool ConditionChangeID(int id) => changeAble.Contains(id);
		
        public virtual void Enter() { }
		public virtual void Update() { }
		public virtual void Exit() { }
	}

}
