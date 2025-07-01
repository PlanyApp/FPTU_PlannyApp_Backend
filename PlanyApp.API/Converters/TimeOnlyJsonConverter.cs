using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlanyApp.API.Converters
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly?>
    {
        public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var timeString = reader.GetString();
                if (TimeOnly.TryParse(timeString, out var time))
                {
                    return time;
                }
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                int hour = 0;
                int minute = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return new TimeOnly(hour, minute);
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = reader.GetString();
                        reader.Read();

                        switch (propertyName.ToLowerInvariant())
                        {
                            case "hour":
                                hour = reader.GetInt32();
                                break;
                            case "minute":
                                minute = reader.GetInt32();
                                break;
                        }
                    }
                }
            }

            throw new JsonException("Invalid JSON for TimeOnly?");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
} 