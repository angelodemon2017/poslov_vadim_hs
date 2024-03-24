using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace ResourceManagment.ResourceClasses
{
    public class ResourceSingle
    {
        public float Progress => OperationHandle.IsValid() ? OperationHandle.PercentComplete : 0;
        public AsyncOperationHandle OperationHandle;
        public Task LoadTask;

        public T GetCachedResource<T>() where T : Object
        {
            if (!OperationHandle.IsValid() || !OperationHandle.IsDone)
            {
                return null;
            }

            return OperationHandle.Result as T;
        }

        public Sprite GetSprite()
        {
            if (!OperationHandle.IsValid() || !OperationHandle.IsDone)
            {
                throw new NullReferenceException("Resource is not loaded");
            }

            var tex = OperationHandle.Result as Texture2D;
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f, 1, SpriteMeshType.FullRect);
        }

        public Sprite GetSpriteFromAtlas(string spriteName)
        {
            if (!OperationHandle.IsValid() || !OperationHandle.IsDone)
            {
                Debug.LogError($"Sprite {spriteName} is not loaded");
                return null;
            }

            var atlas = OperationHandle.Result as SpriteAtlas;
            return atlas.GetSprite(spriteName);
        }
    }
}