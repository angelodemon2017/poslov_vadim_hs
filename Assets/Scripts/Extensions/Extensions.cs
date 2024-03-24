using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace PushEmOut
{
    public static class Extensions
    {

        #region ----------------------------------------- Components -------------------------------------------------------
        public static List<T> FindInstances<T>(bool searchInactive) where T : Component
        {
            var result = new List<T>();

            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
                result.AddRange(root.GetComponentsInChildren<T>(searchInactive));

            return result;
        }

        public static T FindInstance<T>(bool searchInactive) where T : Component
        {
            var instances = FindInstances<T>(searchInactive);
            if (instances.Count == 0)
                return null;

            return instances[0];
        }

        public static T GetAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetAddComponent<T>();
        }

        public static T GetAddComponent<T>(this GameObject go) where T : Component
        {
            T result = default(T);
            var found = go.TryGetComponent<T>(out result);
            if (!found)
                result = go.AddComponent<T>();
            return result;
        }

        public static T GetTopComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetTopComponent<T>();
        }

        public static T GetTopComponent<T>(this GameObject component) where T : Component
        {
            var transform = component.transform;
            T result = null;
            while (transform != null)
            {
                if (transform.TryGetComponent<T>(out var found))
                    result = found;

                transform = transform.parent;
            }
            return result;
        }
        #endregion


        #region ----------------------------------------- GameObjects ------------------------------------------------------
        public static IEnumerable<GameObject> ExtractGameObjects(this IEnumerable<Object> objects)
        {
            foreach (var o in objects)
            {
                var extracted = o.ExtractGameObject();
                if (extracted != null) { yield return extracted; }
            }
        }

        public static GameObject ExtractGameObject(this Object o)
        {
            var asComponent = o as Component;
            if (asComponent != null) { return asComponent.gameObject; }

            var asGameObject = o as GameObject;
            if (asGameObject != null) { return asGameObject; }
            return null;
        }
        #endregion


        #region ----------------------------------------- Strings ----------------------------------------------------------
        public static void TrimEnd(this System.Text.StringBuilder builder, int count)
        {
            var removeStart = builder.Length - count;
            builder.Remove(removeStart, count);
        }
        #endregion
    }
}
