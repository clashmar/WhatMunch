using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WhatMunch_MAUI.Utility
{
    public sealed class FavouritesChangedMessage(string value) : ValueChangedMessage<string>(value)
    {
    }

    public sealed class FavouriteDeletedMessage(string value) : ValueChangedMessage<string>(value)
    {
    }

    public sealed class AllFavouritesDeletedMessage(string value) : ValueChangedMessage<string>(value)
    {
    }
}
