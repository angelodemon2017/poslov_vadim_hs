using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceManagment;

namespace ResourceManagement
{
    public class RemotePacksStatus
    {
        public Task Task;
        public Action<ResourceGroup> LoadCompleted;
        public Action PacksLoadCompleted;

        public List<ResourceGroup> Packs => _packsToCheck.Select(x => x.Key).ToList();
            
        private Dictionary<ResourceGroup, RemotePackStatus> _packsToCheck;

        public float TotalSize;
        public float DownloadedSize;
        public string PacksNames;

        public bool HasExceptions => _packsToCheck.Any(x => x.Value.OperationHandle.OperationException != null);

        public RemotePacksStatus(Dictionary<ResourceGroup, RemotePackStatus> packsStatus)
        {
            TotalSize = 0;
            DownloadedSize = 0;
            foreach (var pack in packsStatus)
            {
                PacksNames += pack.Key + " ";
                TotalSize += pack.Value.TotalSize;
                DownloadedSize += pack.Value.DownloadedSize;
                pack.Value.Loaded += InvokePackLoaded;
            }

            if (packsStatus.Count == 0)
            {
                TotalSize = 1;
                DownloadedSize = 1;
            }

            _packsToCheck = packsStatus;
        }

        private void InvokePackLoaded(ResourceGroup expansions)
        {
           LoadCompleted?.Invoke(expansions); 
        }

        public bool IsPacksLoaded => _packsToCheck.All(x => x.Value.IsPackLoaded);
        public float Progress
        {
            get
            {
                if (_packsToCheck.Count == 0)
                {
                    return 1;
                }
                
                float downloadedSize = 0;

                foreach (var packStatus in _packsToCheck)
                {
                    downloadedSize += packStatus.Value.DownloadedSize;
                }

                DownloadedSize = downloadedSize;

                if (TotalSize == 0)
                {
                    return 1;
                }
                    
                return DownloadedSize / TotalSize;
            }
        }
    }
}