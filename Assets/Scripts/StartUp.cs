using Data;
using Events;
using WindowManagement;

[SingletonBehaviour(false, true, false)]
public class StartUp : MonoSingleton<StartUp>
{
    public Config _config;
    private LevelData _levelData;
    public LevelData CurrentLevelData => _levelData;

    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    private async void Init()
    {
        await WindowManager.Instance.ShowWindow(WindowName.MainMenuWindow);

        EventsController.Subscribe<EventModels.Game.LoadLevel>(this, LoadLevel);
    }

    private void LoadLevel(EventModels.Game.LoadLevel e)
    {
        _levelData = new LevelData(e.Width, e.Length, _config);
    }

    private void OnDestroy()
    {
        EventsController.Unsubscribe<EventModels.Game.LoadLevel>(LoadLevel);
    }
}