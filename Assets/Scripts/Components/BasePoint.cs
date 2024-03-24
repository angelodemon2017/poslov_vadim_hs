using Data;
using UnityEngine;

public class BasePoint : MonoBehaviour, IHaveModel
{
    private BasePointModel _basePoint;

    public void Init<T>(T baseModel) where T : IBaseModel
    {
        if (baseModel is BasePointModel basePoint)
        {
            _basePoint = basePoint;
            UpdateData();
        }
    }

    private void UpdateData()
    {
        transform.position = _basePoint.position;
    }
}