using System.Collections.Generic;
using System.Linq;
using ResourceManagment;

namespace ResourceManagement.RemotePacks.CustomLoaders
{
    public abstract class BaseAutoLoader
    {
        protected readonly IRemotePacksManager RemotePacksManager;
        
        public BaseAutoLoader(IRemotePacksManager remotePacksManager)
        {
            RemotePacksManager = remotePacksManager;
        }

        protected RemotePacksStatus DownloadRequiredPacks(List<ResourceGroup> requiredPacks)
        {
            var notLoadedPacks = requiredPacks.Where(x => !RemotePacksManager.IsPackDownloaded(x)).ToList();

            if (notLoadedPacks.Count > 0)
            {
                var status = RemotePacksManager.DownloadsPacks(notLoadedPacks);
                status.LoadCompleted += ExpansionLoaded;
                return status;
            }
            
            return null;
        }

        protected void BaseInit(bool isEditor)
        {
            if (!isEditor)
            {
                // TODO
                // SystemController.ViewManager.SceneShowing += Handler_WorldLoading;
                // ServerController.InternetStateChanged += Handler_InternetChange;
            }
        }
        
        private void Handler_InternetChange(bool internetActive)
        {
            // TODO
            // if (!internetActive) return;
            // if (SystemController.ViewManager.CurrentSceneIsMeta)
            // {
            //     DownloadRequiredPacks();
            // }
        }

        protected abstract void DownloadRequiredPacks();
        protected abstract void ExpansionLoaded(ResourceGroup resourceGroup);
    }
}