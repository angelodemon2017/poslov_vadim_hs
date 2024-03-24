using System.Threading.Tasks;
using DG.Tweening;

namespace WindowManagement
{
    public interface IWindow
    {
        Task SetModel(WindowModel model);
        Task<Sequence> Show();
        void Close(bool silent = false);
    }
}