using System.Text;
using UnityEngine;
using PushEmOut;

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
public class SingletonBehaviour : System.Attribute
{
    public bool SpawnIfMissing { get; private set; }
    public bool DontDestroy { get; private set; }
    public bool SearchInactive { get; private set; }

    public static SingletonBehaviour Default
    {
        get { return new SingletonBehaviour(false, true, false); }
    }

    public SingletonBehaviour(bool spawnIfMissing, bool dontDestroy, bool searchInactive)
    {
        SpawnIfMissing = spawnIfMissing;
        DontDestroy = dontDestroy;
        SearchInactive = searchInactive;
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>, new()
{
    private static StringBuilder _Builder = new StringBuilder();
    private static string _TypeString = typeof(T).ToString();
    private static T _ActualInstance;

    public static bool HasInstance => _ActualInstance != null;

    public static T Instance
    {
        get
        {
            if (_ActualInstance != null)
            {
                return _ActualInstance;
            }

            var behaviour =
                System.Attribute.GetCustomAttribute(typeof(T), typeof(SingletonBehaviour)) as SingletonBehaviour;
            if (behaviour == null)
            {
                behaviour = SingletonBehaviour.Default;
                LogWarning(
                    "Behaviour is not defined! Consider using [SingularBehaviour] attribute; using Default for now");
            }

            var foundInstances = Extensions.FindInstances<T>(behaviour.SearchInactive);
            if (foundInstances.Count == 0)
            {
                if (behaviour.SpawnIfMissing)
                    _ActualInstance = SpawnInstance();
                else
                    LogWarning("No instances found, strategy is 'Don't spawn if missing'. Things will break");
            }
            else if (foundInstances.Count == 1)
            {
                _ActualInstance = foundInstances[0];
            }
            else
            {
                LogWarning("More than one. Will try to not fail by using the first one");
                _ActualInstance = foundInstances[0];
            }

            if (behaviour.DontDestroy && (_ActualInstance != null))
                DontDestroyOnLoad(_ActualInstance.gameObject);

            return _ActualInstance;
        }
    }

    protected virtual void Awake()
    {
        var tmp = Instance;
    }

    #region Internals

    protected static void Log(string message)
    {
        var formatted = FormatMessage(message);
        Debug.Log(formatted, _ActualInstance);
    }

    protected static void LogWarning(string message)
    {
        var formatted = FormatMessage(message);
        Debug.LogWarning(formatted, _ActualInstance);
    }

    protected static void LogError(string message)
    {
        var formatted = FormatMessage(message);
        Debug.LogError(formatted, _ActualInstance);
    }

    private static T SpawnInstance()
    {
        if (!Application.isPlaying)
            return null;
        
        var holder = new GameObject(_TypeString);
        return holder.AddComponent<T>();
    }

    private static string FormatMessage(string message)
    {
        _Builder.Length = 0;

        _Builder.Append("[");
        _Builder.Append(_TypeString);
        _Builder.Append("] ");
        _Builder.Append(message);

        return _Builder.ToString();
    }

    #endregion
}