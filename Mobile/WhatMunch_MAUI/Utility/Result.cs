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

        private Result()
        {
            IsSuccess = false;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string errorMessage) => new(errorMessage);
        public static Result<T> Failure() => new();
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        private Result()
        {
            IsSuccess = true;
        }

        private Result(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new();
        public static Result Failure(string errorMessage) => new(errorMessage);
    }
}
