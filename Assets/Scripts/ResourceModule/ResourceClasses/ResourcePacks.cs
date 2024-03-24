using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceManagment.ResourceClasses
{
    public class ResourcePacks
    {
        public List<ResourcePack> ResourcesList;

        public float Progress => ResourcesList.Count == 0 ? 1 : ResourcesList.Count(x => x.IsLoaded) / (float)ResourcesList.Count;
        public Task LoadingTask;
        public event Action ResourcesLoaded;

        public void TriggerResourcesLoaded()
        {
            ResourcesLoaded?.Invoke();
        }
        
        public bool IsLoaded => ResourcesList.All(x => x.IsLoaded);
    }
}