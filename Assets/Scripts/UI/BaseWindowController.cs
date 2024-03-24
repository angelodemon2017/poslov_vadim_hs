using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace WindowManagement
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseWindowController : MonoBehaviour, IWindow
    {
        [SerializeField] protected CanvasGroup _canvasGroup;

        protected WindowModel Model;

        public CanvasGroup CanvasGroup => _canvasGroup;

        public bool IsClosed { get; private set; } = false;

        private object locker = new();
        public bool Interactable = true;

        private void OnValidate()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_canvasGroup == null)
            {
                Debug.LogError("Canvas Group is needed");
            }
        }

        public virtual async Task SetModel(WindowModel model)
        {
            Model = model;
            await AfterModelChanged();
        }

        protected virtual Task AfterModelChanged()
        {
            return Task.CompletedTask;
        }

        public virtual Task<Sequence> Show()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                gameObject.SetActive(true);
                try
                {
                    Model?.OnShowCallback?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Model?.OnShowCallback?.Invoke()", e));
                }
            });

            return Task.FromResult(sequence);
        }

        public virtual void Close()
        {
            Close(false);
        }

        public virtual void Close(bool silent = false)
        {
            SetInteractable(false);

            lock (locker)
            {
                if (IsClosed)
                    return;

                if (gameObject == null)
                {
                    Debug.LogError("gameObject == null");
                    InvokeCloseCallback(silent);
                    return;
                }

                gameObject.SetActive(false);

                InvokeCloseCallback(silent);
                IsClosed = true;
            }
        }

        private void InvokeCloseCallback(bool silent)
        {
            if (!silent)
            {
                try
                {
                    Model?.OnCloseCallback?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Model?.OnCloseCallback?.Invoke()", e));
                }
            }
        }

        public void SetInteractable(bool val)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = val;
                _canvasGroup.interactable = val;
            }

            Interactable = val;
        }
    }
}