using FiniteStateMachine;
using System;

namespace WindowManagement
{
    public class GamePlayMenuModel : WindowModel
    {
        private IStateMachine _stateMachine;
        public IStateMachine StateMachine => _stateMachine;

        public GamePlayMenuModel(Action showCallback = null, Action closeCallback = null)
            : base(showCallback, closeCallback)
        {

        }

        public void SelectFSM(IStateMachine fsm)
        {
            _stateMachine = fsm;
        }

        public void SetState(State state)
        {
            StateMachine.SetState(state);
        }
    }
}