using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace FiniteStateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/PatrolState", order = 2, fileName = "PatrolState")]
    public class PatrolState : State
    {
        [SerializeField] State _stateWithoutTargets;
        [SerializeField] float _speedMove;
        [SerializeField] float _triggerDistance;
        private NavMeshAgent _navMeshAgent;
        private List<Vector3> _points = new();
        private int _targetPoint = 0;

        internal override string GetNameState => "Патрулирование";

        public override void Init(Transform transform)
        {
            base.Init(transform);

            if (!_transform.TryGetComponent(out _navMeshAgent))
            {
                _navMeshAgent = _transform.gameObject.AddComponent<NavMeshAgent>();
            }
            _navMeshAgent.speed = _speedMove;

            _points = StartUp.Instance.CurrentLevelData.PatrolPoints.Select(p => p.Position).ToList();
            if (_points.Count == 0)
            {
                if (_stateWithoutTargets != null)
                {
                    var fsm = _transform.GetComponent<FSMController>();
                    fsm.SetState(_stateWithoutTargets);
                }
                else
                {
                    Debug.LogError("Patrol state didn't find any points and havn't next state");
                }
            }
            else
            {
                NextTarget();
            }
        }

        public override void Run()
        {
            if (_points.Count == 0)
                return;

            if (Vector3.Distance(_transform.position, _points[_targetPoint]) < _triggerDistance)
            {
                NextTarget();
            }
        }

        private void NextTarget()
        {
            _targetPoint++;
            if (_targetPoint >= _points.Count)
            {
                _targetPoint = 0;
            }
            _navMeshAgent.SetDestination(_points[_targetPoint]);
        }

        public override void Finish()
        {
            base.Finish();
            _navMeshAgent.SetDestination(_transform.position);
        }
    }
}