using Data;
using FiniteStateMachine;

namespace Events
{
    public static class EventModels
    {
        public static class Game
        {
            public struct LoadLevel : IEvent
            {
                public float Width;
                public float Length;

                public LoadLevel(float width, float length)
                {
                    Width = width;
                    Length = length;
                }
            }

            public struct LoadLevelData : IEvent
            {
                public LevelData LevelData;

                public LoadLevelData(LevelData levelData)
                {
                    LevelData = levelData;
                }
            }

            public struct FocusFSM : IEvent
            {
                public IStateMachine StateMachine;

                public FocusFSM(IStateMachine stateMachine)
                {
                    StateMachine = stateMachine;
                }
            }

            public struct HPStatus : IEvent
            {
                public int HPValue;
                public int HPMax;

                public HPStatus(int hpValue, int hpMax)
                {
                    HPValue = hpValue;
                    HPMax = hpMax;
                }
            }
        }
    }
}