using Data;
using FiniteStateMachine;
using UnityEngine;

public class PersonController : MonoBehaviour, IHaveModel
{
    private bool IsDeath => _personModel.IsDeath;

    private int _gettingDamage;
    private PersonModel _personModel;
    private FSMController _fsmController;

    public void Init<T>(T baseModel) where T : IBaseModel
    {
        if (baseModel is PersonModel personModel)
        {
            _personModel = personModel;
            _gettingDamage = StartUp.Instance._config.DamageValue;
            UpdateData();
            if (TryGetComponent(out _fsmController))
            {
                _fsmController.SelectFSM();
            }
        }
    }

    private void UpdateData()
    {
        transform.position = _personModel.Position;
    }

    private void OnMouseDown()
    {
        if (IsDeath)
            return;

        TakeDamage();
    }

    private void TakeDamage()
    {
        _personModel.GetDamage(_gettingDamage);

        if (IsDeath)
        {
            if (_fsmController != null)
            {
                _fsmController.KillFSM();
            }
        }
    }
}