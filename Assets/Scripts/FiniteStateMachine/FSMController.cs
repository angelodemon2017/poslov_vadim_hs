using Events;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    public class FSMController : MonoBehaviour, IStateMachine
    {
        [SerializeField] private List<State> _states = new();
        [SerializeField] private State _deathState;
        private State _currentState;
        private State _selectedStateLink;

        public List<State> GetStates => _states;

        private void Awake()
        {
            SetState(_states[0]);
        }

        public void SelectFSM()
        {
            EventsController.Fire(new EventModels.Game.FocusFSM(this));
        }

        private void Update()
        {
            _currentState?.Run();
        }

        public void KillFSM()
        {
            SetState(_deathState);
        }

        public void SetState(State newState)
        {
            if (_selectedStateLink == newState ||
                _selectedStateLink == _deathState && _deathState != null)
            {
                return;
            }

            _currentState?.Finish();
            _selectedStateLink = newState;
            _currentState = Instantiate(newState);
            _currentState.Init(transform);
        }
    }
}