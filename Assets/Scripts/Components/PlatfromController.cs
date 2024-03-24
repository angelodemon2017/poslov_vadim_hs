using Data;
using Events;
using ResourceManagment;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlatfromController : MonoBehaviour
{
    private LevelData _levelData;
    private const float MINIMAL_FIELD = 2f;

    [Header("Available field generation")]
    [SerializeField] private float Weight = 0f;
    [SerializeField] private float Length = 0f;
    public Action<LevelData> loadedLevelData;

    private void Awake()
    {
        EventsController.Subscribe<EventModels.Game.LoadLevelData>(this, SpawnLevelData);

        EventsController.Fire(new EventModels.Game.LoadLevel(Weight, Length));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(Weight, 2, Length));
    }

    private void OnValidate()
    {
        if (Weight < MINIMAL_FIELD)
        {
            Weight = MINIMAL_FIELD;
        }
        if (Length < MINIMAL_FIELD)
        {
            Length = MINIMAL_FIELD;
        }
    }

    private async void SpawnLevelData(EventModels.Game.LoadLevelData e)
    {
        _levelData = e.LevelData;

        await SpawnObject<BasePoint>(_levelData.BasePoint);

        SpawnPatrolPoints();

        await SpawnObject<PersonController>(_levelData.Person);
    }

    private async void SpawnPatrolPoints()
    {
        foreach (var pp in _levelData.PatrolPoints)
        {
            await SpawnObject<PatrolPoint>(pp);
        }
    }

    private async Task<T> SpawnObject<T>(IBaseModel baseModel) where T : IHaveModel
    {
        var basePoint = await ResourceManager.Instance.LoadAsset<GameObject>(baseModel.NameAsset);

        var basePointObject = Instantiate(basePoint);
        var personContrlComn = basePointObject.GetComponent<T>();
        personContrlComn.Init(baseModel);
        return personContrlComn;
    }

    private void OnDestroy()
    {
        EventsController.Unsubscribe<EventModels.Game.LoadLevelData>(SpawnLevelData);
    }
}