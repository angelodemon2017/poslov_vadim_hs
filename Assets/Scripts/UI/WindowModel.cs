using System;

namespace WindowManagement
{
    public class WindowModel
    {
        public Action OnShowCallback;
        public Action OnCloseCallback;

        public WindowModel(Action showCallback, Action closeCallback)
        {
            OnShowCallback = showCallback;
            OnCloseCallback = closeCallback;
        }
    }
}