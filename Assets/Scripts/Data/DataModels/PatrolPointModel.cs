using GamePrefabs;
using UnityEngine;

namespace Data
{
    public class PatrolPointModel : IBaseModel
    {
        public Vector3 Position;

        public string NameAsset => Prefabs.PatrolPoint.ToString();

        public PatrolPointModel(Vector3 position)
        {
            Position = position;
        }
    }
}