namespace WhatMunch_MAUI.MockData
{
    public static class MockPlace
    {
        public const string ID = "ChIJxUpFWk9vxkcRwNu9kxkQoM8";
        public static string GetMockPlaceJson()
        {
            return @"
            {
            ""places"": [
                {
                    ""id"": ""_PLACE_ID_"",
                    ""primaryType"": ""cafe"",
                    ""types"": [
                        ""restaurant"",
                        ""point_of_interest"",
                        ""vegan_restaurant"",
                        ""vegetarian_restaurant"",
                        ""food"",
                        ""establishment""
                    ],
                    ""rating"": 4.9,
                    ""priceLevel"": ""PRICE_LEVEL_MODERATE"",
                    ""websiteUri"": ""https://www.pampalini.nl/"",
                    ""regularOpeningHours"": {
                        ""openNow"": true,
                        ""weekdayDescriptions"": [
                            ""Monday: Closed"",
                            ""Tuesday: Closed"",
                            ""Wednesday: 10:00 AM – 4:30 PM"",
                            ""Thursday: 10:00 AM – 4:30 PM"",
                            ""Friday: 10:00 AM – 4:30 PM"",
                            ""Saturday: 10:00 AM – 4:30 PM"",
                            ""Sunday: 10:00 AM – 4:30 PM""
                        ]
                    },
                    ""userRatingCount"": 1765,
                    ""displayName"": {
                        ""text"": ""Pampalini Lunchroom & Coffee - Utrecht 2014"",
                        ""languageCode"": ""en""
                    },
                    ""primaryTypeDisplayName"": {
                        ""text"": ""Restaurant""
                    },
                    ""shortFormattedAddress"": ""Wittevrouwenstraat 14, Utrecht"",
                    ""goodForChildren"": true,
                    ""allowsDogs"": true,
                    ""location"": {
                        ""latitude"": 52.092992300000006,
                        ""longitude"": 5.1221492
                        },
                    ""reviews"": [
                        {
                            ""relativePublishTimeDescription"": ""3 months ago"",
                            ""rating"": 4,
                            ""text"": {
                            ""text"": ""We had a wonderful experience visiting Super Bros on our trip to the Netherlands. The owner welcomed us right in and got to know us while prepping our food. We enjoyed the atmosphere of the shop paired with the simplicity of the menu. We ordered the chicken panini with the lentil soup and they were both delicious 🥹 We’ll be sure to come back!"",
                            ""languageCode"": ""en""
                            }
                        },
                        {
                            ""relativePublishTimeDescription"": ""2 months ago"",
                            ""rating"": 1,
                            ""text"": {
                            ""text"": ""I was looking for something light to eat and I stumbled upon this spot based on the good reviews. The chicken panini did not disappoint and it was the perfect light meal I needed. I also had a good conversation with one of the co-founders and the hospitality and service was phenomenal. I will definitely return as a customer if I’m ever in the area."",
                            ""languageCode"": ""en""
                            }
                        },
                        {
                            ""relativePublishTimeDescription"": ""2 weeks ago"",
                            ""rating"": 0,
                            ""text"": {
                            ""text"": ""Had the Shoarma wrap and it tasted amazing. Owner and staff were incredibly welcoming and outgoing making the whole experience memorable. Will definitely come back if I'm in the area"",
                            ""languageCode"": ""en""
                            }
                        },
                        {
                            ""relativePublishTimeDescription"": ""2 months ago"",
                            ""rating"": 2,
                            ""text"": {
                            ""text"": ""I had a great time! Everything was so tasty and healthy. And I loved the beautiful story behind the business 🌱 really something that should be in every Dutch city!"",
                            ""languageCode"": ""en""
                            }
                        },
                        {
                            ""relativePublishTimeDescription"": ""a month ago"",
                            ""rating"": 3,
                            ""text"": {
                            ""text"": ""This place is a real changemaker in the healthy quick delivered food. Come by if ur searching a super tasty soup or try the extremely tasteful sandwiches and wraps. The actual quality of all the individual foods are very above expectations. To your wallet “this is a bang for ur buck”. Also very nice owner. Super polite, nice and great service.\n\nExpect above restaurant food, quick and easy and for a good price!"",
                            ""languageCode"": ""en""
                            }
                        }
                    ]
                }
            ],
            ""nextPageToken"": ""AeeoHcI7Xnd8tU32jESwMvgnhAo6QAJfBz6liaHIUALeeGfQ-8NM763uoABKPHSXrxo6MwR6GPkQI3BuamzPLyfNC1ssp5P6JBXwRmUADDsokhrcRQ""
            }".Replace("_PLACE_ID_",ID);
        }
    }
}
