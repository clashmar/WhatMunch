namespace WhatMunch_MAUI.Utility.Exceptions
{
    public class LocationException : Exception
    {
        public LocationException() : base("Location unavailable.") { }
        public LocationException(string message) : base(message) { }
    }
}
