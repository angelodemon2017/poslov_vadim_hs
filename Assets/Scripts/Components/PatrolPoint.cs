using Data;
using UnityEngine;

public class PatrolPoint : MonoBehaviour, IHaveModel
{
    private PatrolPointModel _patrolPoint;

    public void Init<T>(T baseModel) where T : IBaseModel
    {
        if (baseModel is PatrolPointModel patrolPoint)
        {
            _patrolPoint = patrolPoint;
            UpdateData();
        }
    }

    private void UpdateData()
    {
        transform.position = _patrolPoint.Position;
    }
}