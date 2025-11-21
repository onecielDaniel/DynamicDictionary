using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneCiel.System.Dynamics
{
    /// <summary>
    /// A JsonConverter that enables seamless serialization and deserialization of DynamicDictionary
    /// using System.Text.Json.
    /// </summary>
    public class DynamicDictionaryJsonConverter : JsonConverter<DynamicDictionary>
    {
        /// <summary>
        /// Initializes a new instance of the DynamicDictionaryJsonConverter.
        /// </summary>
        public DynamicDictionaryJsonConverter()
        {
        }

        /// <summary>
        /// Determines whether the specified type can be converted by this converter.
        /// </summary>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DynamicDictionary).IsAssignableFrom(typeToConvert);
        }

        /// <summary>
        /// Reads and converts the JSON to a DynamicDictionary object.
        /// </summary>
        public override DynamicDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }

            // Create instance of the actual type (supports derived classes)
            var dictionary = (DynamicDictionary?)Activator.CreateInstance(typeToConvert);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected PropertyName token");
                }

                string? propertyName = reader.GetString();
                reader.Read();

                // Deserialize as JsonElement and convert
                var element = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                if (dictionary != null && propertyName != null)
                {
                    var convertedValue = ConvertJsonElement(element);
                    dictionary[propertyName] = convertedValue ?? string.Empty;
                }
            }

            throw new JsonException("Expected EndObject token");
        }

        /// <summary>
        /// Writes a DynamicDictionary object as JSON.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, DynamicDictionary? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key);
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Converts a JsonElement to the appropriate .NET object type.
        /// </summary>
        private static object? ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => ConvertJsonObject(element),
                JsonValueKind.Array => ConvertJsonArray(element),
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => ConvertJsonNumber(element),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.ToString()
            };
        }

        /// <summary>
        /// Converts a JSON object element to a DynamicDictionary.
        /// </summary>
        private static DynamicDictionary ConvertJsonObject(JsonElement element)
        {
            var result = new DynamicDictionary();
            foreach (var property in element.EnumerateObject())
            {
                var convertedValue = ConvertJsonElement(property.Value);
                result[property.Name] = convertedValue ?? string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Converts a JSON array element to an object array.
        /// </summary>
        private static object ConvertJsonArray(JsonElement element)
        {
            var items = element.EnumerateArray()
                         .Select(ConvertJsonElement)
                         .ToArray();

            if (items.Length > 0 && items.All(item => item is DynamicDictionary))
            {
                return items.Cast<DynamicDictionary>().ToArray();
            }

            return items;
        }

        /// <summary>
        /// Converts a JSON number element to the appropriate .NET numeric type.
        /// </summary>
        private static object ConvertJsonNumber(JsonElement element)
        {
            if (element.TryGetDecimal(out var decimalValue))
                return decimalValue;
            if (element.TryGetInt64(out var longValue))
                return longValue;
            if (element.TryGetInt32(out var intValue))
                return intValue;
            if (element.TryGetDouble(out var doubleValue))
                return doubleValue;

            return element.GetRawText();
        }
    }
}
