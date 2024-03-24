using GamePrefabs;
using UnityEngine;

namespace Data
{
    public class BasePointModel : IBaseModel
    {
        public Vector3 position;

        public string NameAsset => Prefabs.BasePoint.ToString();

        public BasePointModel(float borderWidth, float borderLength)
        {
            position = new Vector3(Random.Range(-(borderWidth/2f), borderWidth / 2f), 0f,
                Random.Range(-(borderLength / 2f), borderLength / 2f));
        }
    }
}