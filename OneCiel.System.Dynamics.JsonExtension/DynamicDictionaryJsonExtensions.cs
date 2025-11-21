using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneCiel.System.Dynamics
{
    /// <summary>
    /// JSON extension methods for DynamicDictionary using standardized interfaces.
    /// All JSON deserialization must use DynamicDictionary.Create(json, IDynamicsJsonSerializer) pattern.
    /// This class provides helper methods and serialization support.
    /// </summary>
    public static class DynamicDictionaryJsonExtensions
    {
        private static IDynamicsJsonSerializer _defaultSerializer = 
            new SystemTextJsonDynamicsSerializer();

        /// <summary>
        /// Static constructor that automatically sets the default serializer for DynamicDictionary.
        /// This ensures that DynamicDictionary.Create(json) works without explicit serializer setup.
        /// </summary>
        static DynamicDictionaryJsonExtensions()
        {
            // Automatically set the default serializer for DynamicDictionary
            DynamicDictionary.SetDefaultSerializer(_defaultSerializer);
        }

        /// <summary>
        /// Sets the default JSON serializer for all DynamicDictionary instances.
        /// </summary>
        public static void SetDynamicsJsonSerializer(IDynamicsJsonSerializer serializer)
        {
            _defaultSerializer = serializer ?? 
                throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// Creates a default unified serializer/deserializer with standard options.
        /// This can be used for both serialization and deserialization operations.
        /// </summary>
        /// <returns>A new SystemTextJsonDynamicsSerializer instance with default options.</returns>
        /// <example>
        /// <code>
        /// var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
        /// dynamic data = serializer.Deserialize(json);
        /// string jsonString = serializer.Serialize(data);
        /// </code>
        /// </example>
        public static IDynamicsJsonSerializer CreateDefaultSerializer()
        {
            return new SystemTextJsonDynamicsSerializer();
        }

        /// <summary>
        /// Creates a unified serializer/deserializer with custom JsonSerializerOptions.
        /// </summary>
        /// <param name="serializeOptions">Custom JsonSerializerOptions for serialization.</param>
        /// <param name="deserializeOptions">Custom JsonSerializerOptions for deserialization.</param>
        /// <returns>A new SystemTextJsonDynamicsSerializer instance with the provided options.</returns>
        /// <example>
        /// <code>
        /// var serOptions = new JsonSerializerOptions { WriteIndented = false };
        /// var deserOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        /// var serializer = DynamicDictionaryJsonExtensions.CreateSerializer(serOptions, deserOptions);
        /// </code>
        /// </example>
        public static IDynamicsJsonSerializer CreateSerializer(JsonSerializerOptions? serializeOptions = null, JsonSerializerOptions? deserializeOptions = null)
        {
            return new SystemTextJsonDynamicsSerializer(serializeOptions ?? GetDefaultSerializerOptions(), deserializeOptions ?? GetDefaultDeserializerOptions());
        }

        /// <summary>
        /// Gets the default JsonSerializerOptions for serialization.
        /// Returns options with WriteIndented=true, CamelCase naming, and IgnoreNull.
        /// </summary>
        /// <returns>Default JsonSerializerOptions for serialization.</returns>
        /// <example>
        /// <code>
        /// var serOptions = DynamicDictionaryJsonExtensions.GetDefaultSerializerOptions();
        /// serOptions.WriteIndented = false; // Customize if needed
        /// var serializer = DynamicDictionaryJsonExtensions.CreateSerializer(serOptions);
        /// </code>
        /// </example>
        public static JsonSerializerOptions GetDefaultSerializerOptions()
        {
            return SystemTextJsonDynamicsSerializer.GetDefaultSerializeOptions();
        }

        /// <summary>
        /// Gets the default JsonSerializerOptions for deserialization.
        /// Returns options with CaseInsensitive=true, AllowTrailingCommas=true, SkipComments, and DynamicDictionaryJsonConverter.
        /// </summary>
        /// <returns>Default JsonSerializerOptions for deserialization.</returns>
        /// <example>
        /// <code>
        /// var deserOptions = DynamicDictionaryJsonExtensions.GetDefaultDeserializerOptions();
        /// deserOptions.AllowTrailingCommas = false; // Customize if needed
        /// var serializer = DynamicDictionaryJsonExtensions.CreateSerializer(null, deserOptions);
        /// </code>
        /// </example>
        public static JsonSerializerOptions GetDefaultDeserializerOptions()
        {
            return SystemTextJsonDynamicsSerializer.GetDefaultDeserializeOptions();
        }

        #region File Operations

        /// <summary>
        /// Saves a DynamicDictionary to a JSON file asynchronously using default serializer.
        /// </summary>
        public static async Task ToJsonFileAsync(this DynamicDictionary dictionary, string filePath)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            try
            {
                var json = dictionary.Serialize();
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while saving JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Saves a DynamicDictionary to a JSON file asynchronously with custom options.
        /// </summary>
        public static async Task ToJsonFileAsync(this DynamicDictionary dictionary, string filePath, JsonSerializerOptions options)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            try
            {
                var serializer = new SystemTextJsonDynamicsSerializer(options);
                var json = dictionary.Serialize(serializer);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while saving JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Saves a DynamicDictionary to a JSON file asynchronously using a custom serializer.
        /// </summary>
        public static async Task ToJsonFileAsync(this DynamicDictionary dictionary, string filePath, IDynamicsJsonSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            try
            {
                var json = dictionary.Serialize(serializer);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while saving JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Saves a DynamicDictionary to a JSON file synchronously using default serializer.
        /// </summary>
        public static void ToJsonFile(this DynamicDictionary dictionary, string filePath)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            try
            {
                var json = dictionary.Serialize();
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while saving JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Saves a DynamicDictionary to a JSON file synchronously with custom options.
        /// </summary>
        public static void ToJsonFile(this DynamicDictionary dictionary, string filePath, JsonSerializerOptions options)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            try
            {
                var serializer = new SystemTextJsonDynamicsSerializer(options);
                var json = dictionary.Serialize(serializer);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while saving JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Saves a DynamicDictionary to a JSON file synchronously using a custom serializer.
        /// </summary>
        public static void ToJsonFile(this DynamicDictionary dictionary, string filePath, IDynamicsJsonSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            try
            {
                var json = dictionary.Serialize(serializer);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while saving JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Creates a DynamicDictionary from a JSON file asynchronously using a serializer.
        /// Must provide an IDynamicsJsonSerializer implementation for clean architecture.
        /// </summary>
        /// <param name="filePath">Path to the JSON file.</param>
        /// <param name="serializer">The IDynamicsJsonSerializer to use for deserialization.</param>
        /// <returns>A DynamicDictionary containing the deserialized data.</returns>
        /// <exception cref="ArgumentException">Thrown when filePath is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when serializer is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when file is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown when an error occurs.</exception>
        /// <example>
        /// <code>
        /// var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
        /// var data = await DynamicDictionaryJsonExtensions.FromJsonFileAsync("data.json", serializer);
        /// </code>
        /// </example>
        public static async Task<DynamicDictionary> FromJsonFileAsync(string filePath, IDynamicsJsonSerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return serializer.Deserialize(json);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while reading JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Creates a DynamicDictionary from a JSON file synchronously using a serializer.
        /// Must provide an IDynamicsJsonSerializer implementation for clean architecture.
        /// </summary>
        /// <param name="filePath">Path to the JSON file.</param>
        /// <param name="serializer">The IDynamicsJsonSerializer to use for deserialization.</param>
        /// <returns>A DynamicDictionary containing the deserialized data.</returns>
        /// <exception cref="ArgumentException">Thrown when filePath is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when serializer is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when file is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown when an error occurs.</exception>
        /// <example>
        /// <code>
        /// var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
        /// var data = DynamicDictionaryJsonExtensions.FromJsonFile("data.json", serializer);
        /// </code>
        /// </example>
        public static DynamicDictionary FromJsonFile(string filePath, IDynamicsJsonSerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            try
            {
                var json = File.ReadAllText(filePath);
                return serializer.Deserialize(json);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while reading JSON file: {filePath}", ex);
            }
        }

        #endregion
    }
}
