using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceManagement;
using ResourceManagment.ResourceClasses;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace ResourceManagment
{
    [SingletonBehaviour(false, true, false)]
    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        private const float RemoveTimeoutSec = 60;

        private IRemotePacksManager _remotePacksManager;
        private IResourceLocator _locator;
        private readonly Dictionary<ResourceGroup, ResourcePack> _resources = new();
        private readonly Dictionary<string, ResourceSingle> _singleLocalCachedResources = new();
        private Dictionary<ResourceGroup, ReleaseResourceItem> _releasingQueue = new();

        public Action<ResourceGroup> LoadedCallback { get; set; }

        private IRemotePacksManager RemotePacksManager => _remotePacksManager;

        protected override void Awake()
        {
            base.Awake();

            _remotePacksManager = new RemotePacksManager(this);
        }

        private void OnOnLateUpdate()
        {
            float currentTime = Time.time;
            ReleaseResourceItem groupToReleaseResource = null;
            foreach (var group in _releasingQueue)
            {
                if (group.Value.StartRemoveTime + RemoveTimeoutSec < currentTime)
                {
                    groupToReleaseResource = group.Value;
                    break;
                }
            }

            if (groupToReleaseResource != null)
            {
                if (_releasingQueue.ContainsKey(groupToReleaseResource.ResourceGroup))
                {
                    _releasingQueue.Remove(groupToReleaseResource.ResourceGroup);
                }

                ReleaseGroupImmediate(groupToReleaseResource.ResourceGroup);

                if (groupToReleaseResource.RemoveFromCache)
                {
                    // TODO
                    // ExpansionController.DeletePacksCache(new List<ResourceGroup>()
                    // {
                    // 	groupToRelease.ResourceGroup
                    // });
                }
            }
        }

        public async Task Initialize()
        {
            var res = Addressables.InitializeAsync();
            _locator = await res.Task;
            foreach (var resourceGroup in Enum.GetValues(typeof(ResourceGroup)).Cast<ResourceGroup>())
            {
                if (_locator.Locate(resourceGroup.ToString(), typeof(Object), out var locations))
                {
                    var resource = new ResourcePack()
                    {
                        ResourceGroup = resourceGroup
                    };

                    _resources.Add(resourceGroup, resource);
                }
            }
        }

        public ResourcePacks LoadGroup(ResourceGroup group)
        {
            return LoadGroups(new List<ResourceGroup>()
            {
                group
            });
        }

        public ResourcePacks LoadGroups(List<ResourceGroup> groups)
        {
            foreach (var group in groups)
            {
                if (_releasingQueue.ContainsKey(group))
                {
                    _releasingQueue.Remove(group);
                }
            }

            var resources = new List<ResourcePack>();
            foreach (var @group in groups)
            {
                if (_resources.TryGetValue(@group, out ResourcePack ResourcePack))
                {
                    ResourcePack.LoadRequests++;
                }
            }

            var filteredGroups = groups.Where(x => x != ResourceGroup.None && !IsResourceGroupLoaded(x));
            foreach (var @group in filteredGroups)
            {
                if (!_resources.ContainsKey(@group))
                {
                    Debug.Log($"Group not exist {group}");
                    continue;
                }

                var resource = _resources[@group];
                if (resource.LoadInProgress)
                {
                    resources.Add(resource);
                    continue;
                }

                if (!resource.OperationHandle.IsValid() ||
                    resource.OperationHandle.OperationException != null)
                {
                    resource.LoadTask = LoadResource(resource);
                }

                resources.Add(resource);
            }

            var res = new ResourcePacks()
            {
                ResourcesList = resources,
            };

            res.LoadingTask = WaitForResources(resources, res.TriggerResourcesLoaded);
            return res;
        }

        public void ReleaseGroup(ResourceGroup group, bool removeFromCache, bool checkReferences)
        {
            if (_resources.ContainsKey(group) && --_resources[group].LoadRequests > 0 && checkReferences)
            {
                Debug.LogWarning("Can't release group, someone referred to this resource");
                return;
            }

            if (!_releasingQueue.ContainsKey(group))
            {
                _releasingQueue.Add(group, new ReleaseResourceItem()
                {
                    ResourceGroup = group,
                    RemoveFromCache = removeFromCache,
                    StartRemoveTime = Time.time
                });
            }
        }

        public void ReleaseGroups(List<ResourceGroup> groups, bool removeFromCache, bool checkReferences)
        {
            foreach (var group in groups)
            {
                ReleaseGroup(group, removeFromCache, checkReferences);
            }
        }

        public void ReleaseGroupsImmediate(List<ResourceGroup> groups)
        {
            foreach (var group in groups)
            {
                ReleaseGroupImmediate(group);
            }
        }

        public void ReleaseGroupImmediate(ResourceGroup group)
        {
            if (!_resources.ContainsKey(group))
            {
                Debug.Log($"Group {group} not exist");
                return;
            }

            var resource = _resources[group];
            if (!resource.OperationHandle.IsValid())
            {
                Debug.Log($"Group {group} release failed");
                return;
            }

            Addressables.Release(resource.OperationHandle);
            Debug.Log($"Group {group} has been released");
            _resources[group] = new ResourcePack()
            {
                ResourceGroup = group
            };
        }

        public ResourceSingle GetSingleResource(string name)
        {
            if (_singleLocalCachedResources.ContainsKey(name))
            {
                return _singleLocalCachedResources[name];
            }

            var resource = new ResourceSingle();
            resource.LoadTask = LoadResource(name, resource);
            _singleLocalCachedResources.Add(name, resource);
            return resource;
        }

        private async Task LoadResource(string name, ResourceSingle resource)
        {
            resource.OperationHandle = Addressables.LoadAssetAsync<Object>(name);

            while (!resource.OperationHandle.IsDone)
            {
                await Task.Yield();
            }
        }

        public void ReleaseSingleResource(string name)
        {
            if (_singleLocalCachedResources.ContainsKey(name))
            {
                var resource = _singleLocalCachedResources[name];
                Addressables.Release(resource.OperationHandle);
                _singleLocalCachedResources.Remove(name);
            }
        }

        private async Task WaitForResources(List<ResourcePack> ResourcePack, Action onComplete)
        {
            var tasks = ResourcePack.Select(x => x.LoadTask).ToList();

            await Task.WhenAll(tasks);
            // TODO
            // await Task.WaitUntil(() => ResourcePack.All(x => x.OperationHandle.IsDone));

            onComplete?.Invoke();
        }

        private async Task LoadResource(ResourcePack ResourcePack)
        {
            while (_resources.Values.Count(x => x.LoadInProgress) > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
            }

            ResourcePack.OperationHandle = LoadAssets(ResourcePack.ResourceGroup.ToString(), ResourcePack.AfterLoaded);
            ResourcePack.LoadInProgress = true;
            float startLoadTime = Time.time;
            Debug.Log($"Start caching group {ResourcePack.ResourceGroup}");

            await ResourcePack.OperationHandle.Task;
            while (!ResourcePack.OperationHandle.IsDone)
            {
                await Task.Yield();
            }

            ResourcePack.LoadInProgress = false;

            if (ResourcePack.OperationHandle.IsValid() &&
                ResourcePack.OperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log(
                    $"Complete caching pack {ResourcePack.ResourceGroup}, load time: {Time.time - startLoadTime}");
                ResourcePack.InvokeLoaded();
                LoadedCallback?.Invoke(ResourcePack.ResourceGroup);
            }
            else
            {
                Debug.Log($"Fail loading pack {ResourcePack.ResourceGroup}");
            }
        }

        private AsyncOperationHandle LoadAssets(string key, Action<Object> onObjLoaded)
        {
            _locator.Locate(key, typeof(Object), out var locations);
            locations = locations.Distinct().ToList();
            return Addressables.LoadAssetsAsync(locations, onObjLoaded);
        }

        private bool GroupLoadedCheck(ResourceGroup resourceGroup)
        {
            if (!_resources.ContainsKey(resourceGroup))
            {
                Debug.LogError($"Group {resourceGroup} not exist in dictionary");
                return false;
            }

            if (!_resources[resourceGroup].IsLoaded)
            {
                Debug.LogError($"Group {resourceGroup} not loaded");
                return false;
            }

            return true;
        }

        // public GameObject InstantiateGameObject(string gameObjectIdent, ResourceGroup resourceGroup, Transform parent)
        // {
        //     if (!GroupLoadedCheck(resourceGroup))
        //     {
        //         Debug.LogError($"Failed group check, gameobject name: {gameObjectIdent}");
        //     }
        //
        //     GameObject prefabByName = _resources[resourceGroup].GetPrefabByName(gameObjectIdent);
        //     try
        //     {
        //         return Object.Instantiate(prefabByName, parent);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log("не удалось создать объект " + gameObjectIdent);
        //         throw e;
        //     }
        // }

        public T GetGUIPrefabByName<T>(string name, ResourceGroup resourceGroup) where T : Component
        {
            if (!GroupLoadedCheck(resourceGroup))
            {
                Debug.LogError($"Failed group check, gameobject name: {name}");
            }

            var prefab = _resources[resourceGroup].GetPrefabByName(name);
            var neededComponent = prefab.GetComponent<T>();

            if (neededComponent == null)
            {
                string txt = $"нет такого компонента/объекта  {name} {typeof(T)}";
                Debug.LogWarning(txt);
                throw new NullReferenceException(txt);
            }

            return neededComponent;
        }

        public T InstantiatePrefabByName<T>(string name, ResourceGroup resourceGroup, Transform parent = null)
            where T : Component
        {
            if (!GroupLoadedCheck(resourceGroup))
            {
                Debug.LogWarning("Failed group check");
                return null;
            }

            var neededObj = _resources[resourceGroup].GetPrefabByName(name);

            var gameObject = MonoBehaviour.Instantiate(neededObj, parent);
            var neededComponent = gameObject.GetComponent<T>();

            if (neededComponent == null)
            {
                string txt = $"нет такого компонента/объекта  {name} {typeof(T)}";
                Debug.LogWarning(txt);
                return null;
            }

            return neededComponent;
        }

        public T GetPrefabByName<T>(string name, ResourceGroup resourceGroup) where T : Component
        {
            if (!GroupLoadedCheck(resourceGroup))
            {
                Debug.LogError($"Failed group check, gameobject name: {name}");
                return null;
            }

            var prefab = _resources[resourceGroup].GetPrefabByName(name);
            if (prefab == null)
            {
                Debug.LogError($"Failed load prefab, gameobject name: {name}");
                return null;
            }

            return prefab.GetComponent<T>();
        }

        public T GetAssetByName<T>(string name, ResourceGroup resourceGroup) where T : Object
        {
            if (!GroupLoadedCheck(resourceGroup))
            {
                Debug.LogWarning("Failed group check");
                return null;
            }

            if (!_resources[resourceGroup].ObjectExist(name))
            {
                var errorMsg = $"No asset found found, group: {resourceGroup}, name: {name}";
                Debug.LogWarning(errorMsg);
                return null;
            }

            return _resources[resourceGroup].GetAssetByName<T>(name);
        }

        public Sprite GetSpriteByName(string name, ResourceGroup resourceGroup)
        {
            if (!GroupLoadedCheck(resourceGroup))
            {
                throw new NullReferenceException("Failed group check");
            }

            return _resources[resourceGroup].GetSprite(name);
        }

        public Sprite GetSpriteByNameInAtlas(string name, ResourceGroup resourceGroup)
        {
            if (!GroupLoadedCheck(resourceGroup))
            {
                throw new NullReferenceException("Failed group check");
            }

            return _resources[resourceGroup].GetSpriteFromAtlas(name);
        }

        public bool IsResourceGroupLoaded(ResourceGroup group)
        {
            return _resources[group].IsLoaded;
        }

        public bool IsResourceGroupLoaded(List<ResourceGroup> groups)
        {
            return groups.All(IsResourceGroupLoaded);
        }

        public async Task<T> LoadAsset<T>(string name) where T : Object
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<T>(name);
            while (asyncOperationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                await Task.Yield();
            }

            return asyncOperationHandle.Result;
        }

        public string GetDataFile(string fileName)
        {
            string value = null;
            fileName = fileName.Replace(".json", "");

            if (GroupLoadedCheck(ResourceGroup.Configs))
            {
                var textAsset = _resources[ResourceGroup.Configs].GetAssetByName<TextAsset>(fileName);
                if (textAsset == null)
                {
                    return null;
                }

                value = textAsset.text;
            }

            return value;
        }

        public ResourceGroup StringToResourceGroup(string str)
        {
            return Enum.TryParse(str, out ResourceGroup rg) ? rg : ResourceGroup.None;
        }
    }
}