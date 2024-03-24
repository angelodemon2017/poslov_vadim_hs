using Events;
using GamePrefabs;
using UnityEngine;

namespace Data
{
    public class PersonModel : IBaseModel
    {
        public int MaxHP;
        public Vector3 Position;

        private int _currentHP;

        public int CurrentHP
        {
            get { return _currentHP; }
            set 
            {
                _currentHP = value;
                if (_currentHP < 0)
                {
                    _currentHP = 0;
                }

                EventsController.Fire(new EventModels.Game.HPStatus(_currentHP, MaxHP));
            }
        }
        public bool IsDeath => _currentHP <= 0;

        public string NameAsset => Prefabs.Person.ToString();

        public PersonModel(int HP, Vector3 posit)
        {
            MaxHP = HP;
            CurrentHP = HP;
            Position = posit;
        }

        public void GetDamage(int countDamage)
        {
            if (CurrentHP > 0)
            {
                CurrentHP -= countDamage;
            }
        }
    }
}