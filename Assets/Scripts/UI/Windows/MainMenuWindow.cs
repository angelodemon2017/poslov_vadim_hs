using Enums;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using WindowManagement;

public class MainMenuWindow : BaseWindowController
{
    [SerializeField] private Button _playGameButton;

    private void Awake()
    {
        _playGameButton.onClick.AddListener(OnPlayGame);
    }

    private async void OnPlayGame()
    {
        await WindowManager.Instance.ShowWindow(WindowName.GamePlayMenuWindow);
        ScenesChanger.GotoScene(Scenes.GamePlay);
    }

    private void OnDestroy()
    {
        _playGameButton.onClick.RemoveAllListeners();
    }
}