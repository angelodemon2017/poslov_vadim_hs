using System;
using System.Threading.Tasks;
using ResourceManagment;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResourceManagement
{
    public class RemotePackStatus
    {
        public ResourceGroup CurrentPack;
        public bool IsPackLoaded => Mathf.Abs(TotalSize - DownloadedSize) < 0.001;
            
        public float TotalSize;
        
        private const float _bytesToMb = 1024 * 1024;

        public float DownloadedSize
        {
            get
            {
                if (!OperationHandle.IsValid())
                {
                    return 0;
                }

                return OperationHandle.GetDownloadStatus().DownloadedBytes / _bytesToMb;
            }
        }
        
        public AsyncOperationHandle OperationHandle;
        public Task DownloadTask;
        public Action<ResourceGroup> Loaded;
        public bool LoadStarted;
        public bool InProgress;
    }
}