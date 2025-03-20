using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Dtos
{
    public class ErrorMessageDto
    {
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }

        public string ErrorMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(Detail))
                {
                    return Detail;
                }

                if (AdditionalProperties != null && AdditionalProperties.Count > 0)
                {
                    var firstField = AdditionalProperties.FirstOrDefault();

                    if (firstField.Value.ValueKind == JsonValueKind.Array)
                    {
                        var firstError = firstField.Value.EnumerateArray().FirstOrDefault();
                        if (firstError.ValueKind == JsonValueKind.String)
                        {
                            return firstError.GetString() ?? "Unknown error";
                        }
                    }
                    else if (firstField.Value.ValueKind == JsonValueKind.String)
                    {
                        return firstField.Value.GetString() ?? "Unknown error";
                    }
                }
                return "Unknown error occurred";
            }
        }
    }
}
