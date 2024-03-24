using UnityEngine;
using UnityEngine.AI;

namespace FiniteStateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/ComeBackState", order = 2, fileName = "ComeBackState")]
    public class ComeBackState : State
    {
        [SerializeField] State _endState;
        [SerializeField] float _speedMove;
        [SerializeField] float _triggerDistance;
        private bool _isFinish = false;
        private Vector3 _target;
        private FSMController _fsmController;
        private NavMeshAgent _navMeshAgent;

        internal override string GetNameState => "На базу";

        public override void Init(Transform transform)
        {
            base.Init(transform);

            _fsmController = _transform.GetComponent<FSMController>();

            if (!_transform.TryGetComponent(out _navMeshAgent))
            {
                _navMeshAgent = _transform.gameObject.AddComponent<NavMeshAgent>();
            }
            _navMeshAgent.speed = _speedMove;

            _target = StartUp.Instance.CurrentLevelData.BasePoint.position;
            _navMeshAgent.SetDestination(_target);
        }

        public override void Run()
        {
            if (_isFinish)
                return;

            if (Vector3.Distance(_transform.position, _target) < _triggerDistance)
            {
                _isFinish = true;
                if (_endState != null)
                {
                    _fsmController.SetState(_endState);
                }
            }
        }

        public override void Finish()
        {
            base.Finish();
            _navMeshAgent.SetDestination(_transform.position);
        }
    }
}