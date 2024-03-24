using Sequences;
using UnityEngine;

namespace FiniteStateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/IdleState", order = 2, fileName = "IdleState")]
    public class IdleState : State
    {
        [SerializeField] private SequenceType _sequenceType;
        private AnimationController _animationController;

        internal override string GetNameState => "Бездействие";

        public override void Init(Transform transform)
        {
            base.Init(transform);

            _animationController = _transform.GetComponent<AnimationController>();

            _animationController.StartSequences(_sequenceType);
        }

        public override void Run()
        {

        }

        public override void Finish()
        {
            base.Finish();
            _animationController.EndIdle();
        }
    }
}