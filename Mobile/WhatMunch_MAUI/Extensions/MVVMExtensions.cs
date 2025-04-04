namespace WhatMunch_MAUI.Extensions
{
    public static class MVVMExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            ObservableCollection<T> collection = [];

            foreach(var item in enumerable)
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}
