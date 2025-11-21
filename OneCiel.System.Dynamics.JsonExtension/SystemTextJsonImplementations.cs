using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneCiel.System.Dynamics
{
    /// <summary>
    /// Unified JSON serializer/deserializer using System.Text.Json.
    /// Implements IDynamicsJsonSerializer with configurable JsonSerializerOptions for both serialization and deserialization.
    /// </summary>
    public sealed class SystemTextJsonDynamicsSerializer : IDynamicsJsonSerializer
    {
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _deserializeOptions;

        /// <summary>
        /// Initializes a new instance with optional JsonSerializerOptions.
        /// </summary>
        /// <param name="serializeOptions">Optional JsonSerializerOptions for serialization. If null, uses default options.</param>
        /// <param name="deserializeOptions">Optional JsonSerializerOptions for deserialization. If null, uses default options with DynamicDictionaryJsonConverter.</param>
        public SystemTextJsonDynamicsSerializer(JsonSerializerOptions? serializeOptions = null, JsonSerializerOptions? deserializeOptions = null)
        {
            _serializeOptions = serializeOptions ?? GetDefaultSerializeOptions();
            _deserializeOptions = deserializeOptions != null 
                ? EnsureConverterInOptions(deserializeOptions) 
                : GetDefaultDeserializeOptions();
        }

        /// <summary>
        /// Serializes an object to JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when serialization fails.</exception>
        public string Serialize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            try
            {
                return JsonSerializer.Serialize(obj, _serializeOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Error occurred while serializing object to JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected error occurred during JSON serialization.", ex);
            }
        }

        /// <summary>
        /// Deserializes a JSON string to a DynamicDictionary.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A DynamicDictionary containing the deserialized data.</returns>
        /// <exception cref="ArgumentException">Thrown when json is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when deserialization fails.</exception>
        public DynamicDictionary Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));

            try
            {
                return JsonSerializer.Deserialize<DynamicDictionary>(json, _deserializeOptions)
                    ?? throw new InvalidOperationException("Failed to deserialize JSON string.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Error occurred while deserializing JSON string.", ex);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException("Unexpected error occurred during JSON deserialization.", ex);
            }
        }

        /// <summary>
        /// Deserializes a JSON array string to an array of DynamicDictionary objects.
        /// </summary>
        /// <param name="json">The JSON array string to deserialize.</param>
        /// <returns>An array of DynamicDictionary objects.</returns>
        /// <exception cref="ArgumentException">Thrown when json is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when deserialization fails.</exception>
        public DynamicDictionary[] DeserializeArray(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON array string cannot be null or empty.", nameof(json));

            try
            {
                return JsonSerializer.Deserialize<DynamicDictionary[]>(json, _deserializeOptions)
                    ?? throw new InvalidOperationException("Failed to deserialize JSON array.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Error occurred while deserializing JSON array.", ex);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException("Unexpected error occurred during JSON array deserialization.", ex);
            }
        }

        /// <summary>
        /// Gets the default JsonSerializerOptions for serialization.
        /// </summary>
        /// <returns>JsonSerializerOptions with default serialization settings (WriteIndented, CamelCase, IgnoreNull).</returns>
        public static JsonSerializerOptions GetDefaultSerializeOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Gets the default JsonSerializerOptions for deserialization with DynamicDictionaryJsonConverter.
        /// </summary>
        /// <returns>JsonSerializerOptions with default deserialization settings (CaseInsensitive, AllowCommas, SkipComments).</returns>
        public static JsonSerializerOptions GetDefaultDeserializeOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            options.Converters.Add(new DynamicDictionaryJsonConverter());
            return options;
        }

        /// <summary>
        /// Ensures that DynamicDictionaryJsonConverter is present in the options.
        /// Creates a new options instance if the converter needs to be added.
        /// </summary>
        private static JsonSerializerOptions EnsureConverterInOptions(JsonSerializerOptions options)
        {
            // Check if converter already exists
            foreach (var converter in options.Converters)
            {
                if (converter is DynamicDictionaryJsonConverter)
                    return options;
            }

            // Create new options with the converter
            var newOptions = new JsonSerializerOptions(options);
            newOptions.Converters.Add(new DynamicDictionaryJsonConverter());
            return newOptions;
        }
    }
}
