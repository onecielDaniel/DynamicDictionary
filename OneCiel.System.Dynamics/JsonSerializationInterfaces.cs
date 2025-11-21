using System;

namespace OneCiel.System.Dynamics
{
    /// <summary>
    /// Unified interface for JSON serialization and deserialization operations with DynamicDictionary.
    /// Provides a standard contract for converting between objects/DynamicDictionary and JSON strings.
    /// </summary>
    public interface IDynamicsJsonSerializer
    {
        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when serialization fails.</exception>
        string Serialize(object obj);

        /// <summary>
        /// Deserializes a JSON string to a DynamicDictionary.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A DynamicDictionary containing the deserialized data.</returns>
        /// <exception cref="ArgumentException">Thrown when json is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when deserialization fails.</exception>
        DynamicDictionary Deserialize(string json);

        /// <summary>
        /// Deserializes a JSON array string to an array of DynamicDictionary objects.
        /// </summary>
        /// <param name="json">The JSON array string to deserialize.</param>
        /// <returns>An array of DynamicDictionary objects.</returns>
        /// <exception cref="ArgumentException">Thrown when json is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when deserialization fails.</exception>
        DynamicDictionary[] DeserializeArray(string json);
    }
}
