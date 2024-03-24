using System.Collections.Generic;

namespace FiniteStateMachine
{
    public interface IStateMachine
    {
        List<State> GetStates { get; }

        void SetState(State state);
    }
}