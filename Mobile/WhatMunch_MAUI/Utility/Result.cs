namespace WhatMunch_MAUI.Utility
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Data = value;
        }

        private Result(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string errorMessage) => new(errorMessage);
    }
}
