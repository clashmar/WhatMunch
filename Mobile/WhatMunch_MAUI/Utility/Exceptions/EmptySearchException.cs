namespace WhatMunch_MAUI.Utility.Exceptions
{
    public class EmptySearchException : Exception
    {
        public EmptySearchException() : base("Search returned no results.") { }
        public EmptySearchException(string message) : base(message) { }
    }
}
