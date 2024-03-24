using ResourceManagment;
using System.Threading.Tasks;
using UnityEngine;

namespace WindowManagement
{
    [SingletonBehaviour(false, true, false)]
    public class WindowManager : MonoSingleton<WindowManager>
    {
        [SerializeField] private Transform _windowContainer;
        private BaseWindowController _currentWindow;

        public async Task ShowWindow(WindowName windowName, WindowModel model = null)
        {
            if (windowName == WindowName.None)
                return;

            var oldWindow = _currentWindow;
            var windowPrefab = await ResourceManager.Instance.LoadAsset<GameObject>(windowName.ToString());

            var windowGO = Instantiate(windowPrefab, _windowContainer);

            var windowController = windowGO.GetComponent<BaseWindowController>();

            _currentWindow = windowController;
            await _currentWindow.SetModel(model);

            await _currentWindow.Show();

            if (oldWindow != null)
            {
                oldWindow.Close();
                Destroy(oldWindow.gameObject);
            }
        }
    }
}