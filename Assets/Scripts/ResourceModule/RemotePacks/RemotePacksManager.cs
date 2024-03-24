using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ResourceManagment;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;
using Object = UnityEngine.Object;

namespace ResourceManagement
{
    public class RemotePacksManager : IRemotePacksManager
    {
        private const int MinSize = 100;
        
        // Лучше перейти на интерфейсы
        private ResourceManager _resourceManager;
        
        // [Preserve] 
        // public static string EXPANSIONS_PATH
        // {
        //     get
        //     {
        //         VersionUtility.SetStringVersion(Application.version);
        //         var url = VersionUtility.IsReleaseVersion() ? MAIN_URL : TEST_URL;
        //         return $"{url}/{GetPlatform()}";
        //     }
        // }
        
        private static RuntimePlatform GetPlatform()
        {
#if UNITY_ANDROID
            return RuntimePlatform.Android;
#elif UNITY_IOS 
            return RuntimePlatform.IPhonePlayer;
#elif UNITY_WEBGL
            return RuntimePlatform.WebGLPlayer;
#else
            return Application.platform == RuntimePlatform.WindowsEditor
                ? RuntimePlatform.WindowsPlayer
                : Application.platform;
#endif
        }

        // private static string MAIN_URL =
        //     $"{Prefix}://static.BLABLA.packs";
        //
        // private static string TEST_URL =
        //     $"{Prefix}://static.BLABLA.expansions";

        private Dictionary<ResourceGroup, RemotePackStatus> _packsStatus = new();
        private HashSet<ResourceGroup> _localPacks = new();
        private HashSet<string> _existingPacks = new();
        
        private IResourceLocator _locator;
        
        private List<IExpansionsLoader> _expansionsLoaders = new List<IExpansionsLoader>()
        {
            // new LevelsExpansionsController(),
            // new LocationsExpansionsController(),
            // new EventsExpansionsController(),
            // new IslandExpansionsController()
        };

        private const float _bytesToMb = 1024 * 1024;

        public Action<ResourceGroup> LoadedCallback { get; set; }

        public RemotePacksManager(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        
        public async Task Initialize()
        {
            _locator = Addressables.ResourceLocators.First();

            var expansionsList = Enum.GetValues(typeof(ResourceGroup)).Cast<ResourceGroup>().ToList();
            foreach (ResourceGroup expansion in expansionsList)
            {
                string exp = expansion.ToString();
                if (!int.TryParse(exp, out _))
                {
                    if (_locator.Keys.Contains(exp))
                    {
                        _existingPacks.Add(exp);
                    }
                }

                if (PackExist(expansion.ToString()))
                {
                    bool isLocal = true;
                    if (_locator.Locate(expansion.ToString(), typeof(Object), out var locations))
                    {
                        List<IResourceLocation> allLocations = new List<IResourceLocation>();
                        foreach (var loc in locations)
                        {
                            if (loc.HasDependencies)
                                allLocations.AddRange(loc.Dependencies);
                        }

                        foreach (IResourceLocation location in allLocations.Distinct())
                        {
                            bool isDependencyRemote =
                                ResourceManagerConfig.IsPathRemote(
                                    Addressables.ResourceManager.TransformInternalId(location));
                            if (isDependencyRemote)
                            {
                                isLocal = false;
                                break;
                            }
                        }
                    }

                    if (isLocal)
                    {
                        _localPacks.Add(expansion);
                        continue;
                    }

                    float size = await Addressables.GetDownloadSizeAsync(expansion.ToString()).Task / _bytesToMb;

                    if (size == 0)
                    {
                        Debug.Log($"Expansion {exp} is in cache");
                    }

                    _packsStatus.Add(expansion, new RemotePackStatus()
                    {
                        CurrentPack = expansion,
                        TotalSize = size
                    });
                }
                else
                {
                    //Debug.LogWarning($"Pack {expansion.ToString()} do not exist in packs"); 
                }
            }
        }

        public bool IsPackLocal(ResourceGroup expansions)
        {
            if (!PackExist(expansions.ToString())) return false;
            return _localPacks.Contains(expansions);
        }

        private bool PackExist(string packName)
        {
            return _existingPacks.Contains(packName);
        }

        public RemotePacksStatus GetAllDownloadInProgress()
        {
            Dictionary<ResourceGroup, RemotePackStatus> statuses = new Dictionary<ResourceGroup, RemotePackStatus>();
            foreach (var status in _packsStatus)
            {
                if (status.Value.LoadStarted && !status.Value.IsPackLoaded)
                {
                    statuses.Add(status.Key, status.Value);
                }
            }

            var res = new RemotePacksStatus(statuses);
            res.Task = WaitForPacks(statuses.Select(x => x.Value).ToList(), res.PacksLoadCompleted);
            return res;
        }

        public RemotePacksStatus DownloadsPacks(List<ResourceGroup> expansionsList)
        {
            Dictionary<ResourceGroup, RemotePackStatus> statuses = new Dictionary<ResourceGroup, RemotePackStatus>();
            foreach (var expansion in expansionsList)
            {
                if (!PackExist(expansion.ToString()) || IsPackLocal(expansion))
                {
                    continue;
                }

                if (!_packsStatus[expansion].OperationHandle.IsValid() ||
                    _packsStatus[expansion].OperationHandle.OperationException != null ||
                    _packsStatus[expansion].OperationHandle.Task.Status == TaskStatus.WaitingForActivation)
                {
                    _packsStatus[expansion].DownloadTask = DownloadPack(expansion, _packsStatus[expansion]);
                }

                if (!statuses.ContainsKey(expansion))
                {
                    statuses.Add(expansion, _packsStatus[expansion]);
                }
            }

            if (statuses.Count == 0)
            {
                return new RemotePacksStatus(statuses);
            }


            var res = new RemotePacksStatus(statuses);
            res.Task = WaitForPacks(statuses.Select(x => x.Value).ToList(), res.PacksLoadCompleted);
            return res;
        }

        private async Task DownloadPack(ResourceGroup expansion, RemotePackStatus packStatus)
        {
            packStatus.LoadStarted = true;
            while (_packsStatus.Values.Count(x => x.InProgress) > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
            }

            if (packStatus.OperationHandle.IsValid() && packStatus.OperationHandle.IsDone)
            {
                return;
            }

            if (!EnoughMemory()/* && !_notEnoughWindowShown*/)
            {
                /*_notEnoughWindowShown = true;*/
                // TODO Show Not enough memory window
            }

            // TODO Check Internet
            bool internetActive = true;
            Debug.Log($"Start loading pack {expansion}");
            packStatus.InProgress = true;

            if (internetActive)
            {
                while (!packStatus.OperationHandle.IsDone)
                {
                    await Task.Yield();
                }
            }

            packStatus.InProgress = false;

            if (internetActive && packStatus.OperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Complete loading pack {expansion}");
                packStatus.Loaded?.Invoke(packStatus.CurrentPack);
                LoadedCallback?.Invoke(packStatus.CurrentPack);
            }
            else
            {
                Debug.Log($"Fail loading pack {expansion}");
                // TODO Check Internet

                await DownloadPack(expansion, packStatus);
            }
        }
        private bool EnoughMemory()
        {
            return true;
            // int _diskMemory = AndroidMemoryChecker.GetFreeDiskSpace();
            // return _diskMemory > 100;
        }

        public bool IsPackDownloaded(ResourceGroup expansion)
        {
            if (!PackExist(expansion.ToString())) return false;
            if (IsPackLocal(expansion)) return true;
            return _packsStatus[expansion].IsPackLoaded;
        }

        public bool IsPacksDownloaded(List<ResourceGroup> expansionsList)
        {
            return expansionsList.Where(x => !IsPackLocal(x)).All(x =>
            {
                bool packExist = PackExist(x.ToString());
                bool packIsLocal = _localPacks.Contains(x);
                bool packLoaded = _packsStatus.ContainsKey(x) && _packsStatus[x].IsPackLoaded;
                return packExist && (packIsLocal || packLoaded);
            });
        }

        private async Task WaitForPacks(List<RemotePackStatus> packStatusList, Action onComplete)
        {
            await Task.WhenAll(packStatusList.Select(x => x.DownloadTask));
            await Task.Yield();
            onComplete?.Invoke();
        }

        public float GetPacksSize()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            float size = 0;
            List<string> pathes = new List<string>();
            Caching.GetAllCachePaths(pathes);
            foreach (var path in pathes)
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }
                var allfiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (var filePath in allfiles)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    size += fileInfo.Length / 1024f / 1024f;
                }
            }
           
            return size;
#else
            return 100;
#endif
        }

        public async Task DeletePacksCache(List<ResourceGroup> expansionsList)
        {
            if (expansionsList == null)
            {
                Debug.LogWarning("Expansions list is null");
                return;
            }

            var filteredExpansionsList = expansionsList.Where(x => PackExist(x.ToString()) && !IsPackLocal(x)).ToList();
            if (filteredExpansionsList.Count == 0)
            {
                Debug.LogWarning("Empty expansions list");
                return;
            }

            var operationHandles = new Dictionary<ResourceGroup, AsyncOperationHandle<bool>>();
            foreach (var expansion in filteredExpansionsList)
            {
                if (_packsStatus[expansion].OperationHandle.IsValid())
                {
                    Addressables.Release(_packsStatus[expansion].OperationHandle);
                }

                if (_resourceManager.IsResourceGroupLoaded(expansion))
                {
                    _resourceManager.ReleaseGroupImmediate(expansion);
                }

                operationHandles.Add(expansion, Addressables.ClearDependencyCacheAsync(expansion.ToString(), false));
            }

            await Task.WhenAll(operationHandles.Select(x => x.Value.Task));

            foreach (var operationHandle in operationHandles)
            {
                if (!operationHandle.Value.IsValid())
                {
                    continue;
                }

                if (operationHandle.Value.Result && !_packsStatus[operationHandle.Key].IsPackLoaded)
                {
                    Debug.Log($"{operationHandle.Key} cache cleared");
                }
                else
                {
                    Debug.LogWarning($"{operationHandle.Key} cache not cleared ");
                }

                Addressables.Release(operationHandle.Value);
            }
        }

        public List<IExpansionsLoader> GetAllExpansionLoaders() => _expansionsLoaders;
    }
}