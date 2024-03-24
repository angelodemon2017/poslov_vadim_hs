using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceManagment;

namespace ResourceManagement
{
    public interface IExpansionsLoader
    {
        void Init(bool isEditor);
        List<ResourceGroup> GetPackToUpdate();
        Task AfterPacksPreloaded();
        float LoadToMemoryProgress { get; }
    }
}