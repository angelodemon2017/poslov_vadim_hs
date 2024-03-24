using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceManagment;

namespace ResourceManagement.RemotePacks.CustomLoaders
{
    public class EventsExpansionsController : BaseAutoLoader, IExpansionsLoader
    {
        public EventsExpansionsController(IRemotePacksManager remotePacksManager) : base(remotePacksManager)
        {
        }
        
        public void Init(bool isEditor)
        {
            BaseInit(isEditor);
        }
        
        protected override void DownloadRequiredPacks()
        {
            List<ResourceGroup> requiredPacks = new List<ResourceGroup>();

            base.DownloadRequiredPacks(requiredPacks);
        }

        public List<ResourceGroup> GetPackToUpdate()
        {
            List<ResourceGroup> result = new List<ResourceGroup>();

            return result;
        }

        protected override void ExpansionLoaded(ResourceGroup resourceGroup)
        {

        }

        public async Task AfterPacksPreloaded()
        {

        }

        public float LoadToMemoryProgress => 1;
    }
}