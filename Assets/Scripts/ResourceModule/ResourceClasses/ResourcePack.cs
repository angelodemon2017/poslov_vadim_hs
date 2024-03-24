using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace ResourceManagment.ResourceClasses
{
    public class ResourcePack
    {
        private Dictionary<string, Object> _objects = new Dictionary<string, Object>();
        private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        private List<SpriteAtlas> _atlases = new List<SpriteAtlas>();

        public AsyncOperationHandle OperationHandle;
        public ResourceGroup ResourceGroup;
        public Task LoadTask;
        public int LoadRequests;
        public bool LoadInProgress { get; set; }

        public float Progress => OperationHandle.IsValid() ? OperationHandle.PercentComplete : 0;

        public bool IsLoaded => OperationHandle.IsValid() && OperationHandle.IsDone &&
                                OperationHandle.Status == AsyncOperationStatus.Succeeded;

        public Action<ResourceGroup> Loaded;
        public void InvokeLoaded() => Loaded?.Invoke(ResourceGroup);

        public GameObject GetPrefabByName(string name)
        {
            if (!ObjectExist(name))
            {
                var errorMsg = $"No prefab found, group: {ResourceGroup}, name: {name}";
                Debug.LogWarning(errorMsg);
                return null;
            }

            var gameObject = _objects[name] as GameObject;

            if (gameObject == null)
            {
                var errorMsg = $"{name} is not GameObject";
                Debug.LogWarning(errorMsg);
                return null;
            }

            return gameObject;
        }

        public bool ObjectExist(string objectName)
        {
            return _objects.ContainsKey(objectName);
        }

        public T GetAssetByName<T>(string name) where T : Object
        {
            if (!ObjectExist(name)) return null;
            return _objects[name] as T;
        }

        public Sprite GetSprite(string name)
        {
            if (_sprites.ContainsKey(name))
            {
                return _sprites[name];
            }

            return null;
        }

        public Sprite GetSpriteFromAtlas(string name)
        {
            foreach (var atlas in _atlases)
            {
                var sprite = atlas.GetSprite(name);
                if (sprite != null)
                {
                    return sprite;
                }
            }

            Debug.LogError($"sprite {name} not found");
            throw new NullReferenceException($"sprite {name} not found");
        }

        public void AfterLoaded(Object obj)
        {
            switch (obj)
            {
                case SpriteAtlas spriteAtlas:
                    _atlases.Add(spriteAtlas);
                    break;
                case Texture2D tex:
                    if (!_sprites.ContainsKey(tex.name))
                    {
                        var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f), 1.0f, 1, SpriteMeshType.FullRect);
                        _sprites.Add(tex.name, sprite);
                    }

                    break;
                default:
                    _objects[obj.name] = obj;
                    break;
            }
        }
    }
}