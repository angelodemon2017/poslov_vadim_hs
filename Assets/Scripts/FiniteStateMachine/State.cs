using UnityEngine;

namespace FiniteStateMachine
{
    public abstract class State : ScriptableObject
    {
        internal abstract string GetNameState { get; }
        protected Transform _transform;

        public virtual void Init(Transform transform)
        {
            _transform = transform;
        }

        public abstract void Run();

        public virtual void Finish()
        {

        }
    }
}