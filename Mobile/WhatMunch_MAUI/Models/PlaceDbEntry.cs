using SQLite;

namespace WhatMunch_MAUI.Models
{
    public class PlaceDbEntry
    {
        [PrimaryKey]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        [Unique]
        public string PlaceId { get; set; } = string.Empty;

        [Unique]
        public string PlaceJson { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
