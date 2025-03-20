namespace WhatMunch_MAUI.Utility
{
    public class HttpResult<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }

        private HttpResult(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        private HttpResult(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public static HttpResult<T> Success(T value) => new(value);
        public static HttpResult<T> Failure(string errorMessage) => new(errorMessage);
    }
}
