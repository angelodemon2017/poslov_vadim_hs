using Enums;
using Events;
using FiniteStateMachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using WindowManagement;

public class GamePlayMenuWindow : BaseWindowController
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private HealtBarComponent _healtBar;
    [SerializeField] private ButtonSelectCommand _buttonSelectCommandPrefab;
    [SerializeField] private Transform _parentSelectedCommand;

    private GamePlayMenuModel _model = new();

    private void Awake()
    {
        _closeButton.onClick.AddListener(OnCloseWindow);

        _healtBar.Init();

        EventsController.Subscribe<EventModels.Game.FocusFSM>(this, InitFSM);
    }

    private void InitFSM(EventModels.Game.FocusFSM e)
    {
        _model.SelectFSM(e.StateMachine);
        InitPanelSelectCommand(e.StateMachine.GetStates);
    }

    private void InitPanelSelectCommand(List<State> states)
    {
        foreach (var s in states)
        {
            var bcp = Instantiate(_buttonSelectCommandPrefab, _parentSelectedCommand);
            bcp.Init(s, _model.SetState);
        }
    }

    private async void OnCloseWindow()
    {
        await WindowManager.Instance.ShowWindow(WindowName.MainMenuWindow);
        ScenesChanger.GotoScene(Scenes.StartScene);
    }

    private void OnDestroy()
    {
        EventsController.Unsubscribe<EventModels.Game.FocusFSM>(InitFSM);
        _closeButton.onClick.RemoveAllListeners();
    }
}