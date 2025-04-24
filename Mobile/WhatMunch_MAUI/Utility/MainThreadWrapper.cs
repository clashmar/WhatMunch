namespace WhatMunch_MAUI.Utility
{
    public interface IMainThread
    {
        bool IsMainThread { get; }
        void BeginInvokeOnMainThread(Action action);
    }

    public class MainThreadWrapper : IMainThread
    {
        public bool IsMainThread
            => MainThread.IsMainThread;
        public void BeginInvokeOnMainThread(Action action)
            => MainThread.BeginInvokeOnMainThread(action);
    }
}
