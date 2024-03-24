using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceManagment;

namespace ResourceManagement
{
    public interface IRemotePacksManager
    {
        /// <summary>
        /// Callback, when some resource loaded from web
        /// </summary>
        Action<ResourceGroup> LoadedCallback { get; set; }

        Task Initialize();

        /// <summary>
        /// Get all currently downloading packs status
        /// </summary>
        /// <returns>Status of loading</returns>
        RemotePacksStatus GetAllDownloadInProgress();

        /// <summary>
        /// Start packs (or get status, if pack already downloading) loading
        /// </summary>
        /// <param name="expansionsList">Name of groups</param>
        /// <returns>Status of loading</returns>
        RemotePacksStatus DownloadsPacks(List<ResourceGroup> expansionsList);

        /// <summary>
        /// Check is pack loaded from web
        /// </summary>
        /// <param name="expansion">Name of group</param>
        /// <returns></returns>
        bool IsPackDownloaded(ResourceGroup expansion);

        /// <summary>
        /// Check is packs loaded from web
        /// </summary>
        /// <param name="expansion">Name of group</param>
        /// <returns></returns>
        bool IsPacksDownloaded(List<ResourceGroup> expansionsList);

        /// <summary>
        /// Check is pack stored in default build
        /// </summary>
        /// <param name="expansion">Name of group</param>
        /// <returns></returns>
        bool IsPackLocal(ResourceGroup expansions);

        /// <summary>
        /// Delete pack from device (if pack in memory, it will be unloaded from memory), works immediate
        /// </summary>
        /// <param name="expansionsList">Name of groups</param>
        /// <returns>Job task</returns>
        Task DeletePacksCache(List<ResourceGroup> expansionsList);

        /// <summary>
        /// Get custom expansions manager
        /// </summary>
        /// <returns>Job task</returns>
        // List<IExpansionsLoader> GetAllExpansionLoaders();

        /// <summary>
        /// Get packs size
        /// </summary>
        /// <returns>Packs size</returns>
        float GetPacksSize();
    }
}