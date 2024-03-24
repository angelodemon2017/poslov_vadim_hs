using Enums;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.MonoBehaviourUtils;

namespace Utils
{
    public static class ScenesChanger
    {
        public static AsyncOperation CurrentChangeOperation { get; private set; }
        public static bool IsLoading { get; private set; }
        public static event Action SceneLoadedEvent;

        private static Scenes _loadingSceneName;
        private static bool _changingStarted;

        public static void GotoScene(Scenes sceneName)
        {
            if (IsLoading && _loadingSceneName == sceneName)
                return;

            Debug.Log($"Go to {sceneName}");

            IsLoading = true;
            _loadingSceneName = sceneName;
            _changingStarted = false;

            Coroutines.Instance.StartCoroutine(ChangeSceneCoroutine());
        }

        private static IEnumerator ChangeSceneCoroutine()
        {
            OnChangeStarted();

            yield return new WaitUntil(() => _changingStarted);

            SceneManager.sceneLoaded += OnSceneLoaded;
            CurrentChangeOperation = SceneManager.LoadSceneAsync(_loadingSceneName.ToString());
        }

        private static void OnChangeStarted()
        {
            _changingStarted = true;
        }

        private static void OnChangeFinished()
        {
            _loadingSceneName = default;
            _changingStarted = false;
            
            CurrentChangeOperation = null;
            IsLoading = false;
            
            SceneLoadedEvent?.Invoke();
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            OnChangeFinished();
        }
    }
}