namespace WhatMunch_MAUI.Utility.Exceptions
{
    public class ConnectivityException : Exception
    {
        public ConnectivityException() : base("No internet connection available.") { }
        public ConnectivityException(string message) : base(message) { }
    }
}
