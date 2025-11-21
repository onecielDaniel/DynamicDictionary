using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

// Nullable disabled for this file due to extensive use of dynamic types and DynamicObject
// which inherently work with runtime type information rather than compile-time null checking.
#nullable disable

namespace OneCiel.System.Dynamics
{
 

    /// <summary>
    /// A flexible and dynamic dictionary implementation that supports dynamic object access,
    /// nested property navigation, and case-insensitive key lookup.
    /// </summary>
    public class DynamicDictionary : DynamicObject, IEnumerable<KeyValuePair<string, object>>, IDictionary<string, object>, IReadOnlyDictionary<string, object>
    {
        /// <summary>
        /// The internal dictionary with case-insensitive key comparison.
        /// </summary>
        protected Dictionary<string, object> _data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Global list of value resolvers for type conversion and transformation.
        /// Resolvers are checked in the order they are registered.
        /// </summary>
        private static readonly List<IValueResolver> _resolvers = new List<IValueResolver>();

        /// <summary>
        /// Default JSON serializer used when no serializer is explicitly provided.
        /// This is automatically initialized to SystemTextJsonDynamicsSerializer on first use.
        /// Can be set globally via SetDefaultSerializer method.
        /// </summary>
        private static IDynamicsJsonSerializer _defaultSerializer;

        /// <summary>
        /// Gets the internal dictionary data.
        /// </summary>
        public Dictionary<string, object> Data => _data;

        /// <summary>
        /// Gets a collection of all keys in the dictionary.
        /// </summary>
        public ICollection<string> Keys => _data.Keys;

        /// <summary>
        /// Gets a collection of all values in the dictionary.
        /// </summary>
        public ICollection<object> Values => _data.Values;

        /// <summary>
        /// Gets the key collection for IReadOnlyDictionary implementation.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => _data.Keys;

        /// <summary>
        /// Gets the value collection for IReadOnlyDictionary implementation.
        /// </summary>
        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => _data.Values;

        /// <summary>
        /// Gets a value indicating whether the dictionary is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the number of elements in the dictionary.
        /// </summary>
        public int Count => _data.Count;

        #region Static Methods for Resolver Management

        /// <summary>
        /// Registers a value resolver globally for all DynamicDictionary instances.
        /// Resolvers registered later are checked first.
        /// </summary>
        /// <param name="resolver">The resolver to register.</param>
        /// <exception cref="ArgumentNullException">Thrown when resolver is null.</exception>
        public static void RegisterValueResolver(IValueResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            _resolvers.Insert(0, resolver);
        }

        /// <summary>
        /// Unregisters a value resolver.
        /// </summary>
        /// <param name="resolver">The resolver to unregister.</param>
        /// <returns>True if the resolver was removed; otherwise, false.</returns>
        public static bool UnregisterValueResolver(IValueResolver resolver)
        {
            return _resolvers.Remove(resolver);
        }

        /// <summary>
        /// Clears all registered value resolvers.
        /// </summary>
        public static void ClearValueResolvers()
        {
            _resolvers.Clear();
        }

        /// <summary>
        /// Gets a copy of the currently registered resolvers.
        /// </summary>
        /// <returns>A list of currently registered resolvers.</returns>
        public static IReadOnlyList<IValueResolver> GetRegisteredResolvers()
        {
            return _resolvers.AsReadOnly();
        }

        /// <summary>
        /// Applies registered resolvers to transform a value.
        /// </summary>
        /// <param name="value">The value to resolve.</param>
        /// <returns>The resolved value, or the original value if no resolver matches.</returns>
        private static object ApplyResolvers(object value)
        {
            if (value == null)
                return null;

            foreach (var resolver in _resolvers)
            {
                if (resolver.CanResolve(value))
                {
                    return resolver.Resolve(value);
                }
            }

            return value;
        }

        #endregion

        #region Static Methods for Default Serializer Management

        /// <summary>
        /// Sets the default JSON serializer used when no serializer is explicitly provided.
        /// This allows setting a custom serializer globally for all DynamicDictionary JSON operations.
        /// </summary>
        /// <param name="serializer">The serializer to use as default. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when serializer is null.</exception>
        /// <example>
        /// <code>
        /// // Set custom default serializer with specific options
        /// var customSerializer = new SystemTextJsonDynamicsSerializer(
        ///     new JsonSerializerOptions { WriteIndented = false }
        /// );
        /// DynamicDictionary.SetDefaultSerializer(customSerializer);
        /// 
        /// // Now all Create/Serialize calls without explicit serializer use this
        /// var user = DynamicDictionary.Create&lt;JsonPlaceholderUser&gt;(json);
        /// </code>
        /// </example>
        public static void SetDefaultSerializer(IDynamicsJsonSerializer serializer)
        {
            _defaultSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// Gets the default JSON serializer. Throws an exception if no default serializer has been set.
        /// The default serializer should be set by calling SetDefaultSerializer or by using the JsonExtension library.
        /// </summary>
        /// <returns>The current default serializer.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no default serializer has been set.</exception>
        private static IDynamicsJsonSerializer GetDefaultSerializer()
        {
            if (_defaultSerializer == null)
            {
                throw new InvalidOperationException(
                    "No default JSON serializer has been set. " +
                    "Please call DynamicDictionary.SetDefaultSerializer() or reference OneCiel.System.Dynamics.JsonExtension which sets it automatically, " +
                    "or use the overload that accepts an IDynamicsJsonSerializer parameter.");
            }
            return _defaultSerializer;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new empty DynamicDictionary with dynamic return type.
        /// </summary>
        /// <returns>A new empty DynamicDictionary as dynamic type.</returns>
        /// <example>
        /// <code>
        /// dynamic user = DynamicDictionary.Create();
        /// user.Name = "John";
        /// user.Age = 30;
        /// </code>
        /// </example>
        public static dynamic Create()
        {
            return new DynamicDictionary();
        }

        /// <summary>
        /// Creates a DynamicDictionary initialized with key-value pairs and returns it as dynamic.
        /// </summary>
        /// <param name="items">The collection of key-value pairs to initialize the dictionary with.</param>
        /// <returns>A new DynamicDictionary initialized with the provided items as dynamic type.</returns>
        /// <example>
        /// <code>
        /// var data = new[] 
        /// { 
        ///     new KeyValuePair&lt;string, object&gt;("Name", "John"),
        ///     new KeyValuePair&lt;string, object&gt;("Age", 30)
        /// };
        /// dynamic user = DynamicDictionary.Create(data);
        /// </code>
        /// </example>
        public static dynamic Create(IEnumerable<KeyValuePair<string, object>> items)
        {
            return new DynamicDictionary(items);
        }

        /// <summary>
        /// Creates a DynamicDictionary initialized with an existing dictionary and returns it as dynamic.
        /// </summary>
        /// <param name="dictionary">The dictionary to initialize from.</param>
        /// <returns>A new DynamicDictionary initialized with the provided dictionary as dynamic type.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var source = new Dictionary<string, object> 
        /// { 
        ///     { "Name", "John" },
        ///     { "Age", 30 }
        /// };
        /// dynamic user = DynamicDictionary.Create(source);
        /// ]]>
        /// </code>
        /// </example>
       
        public static dynamic Create(IDictionary<string, object> dictionary)
        {
            return new DynamicDictionary(dictionary);
        }

        /// <summary>
        /// Creates a DynamicDictionary from a JSON string using the provided deserializer function.
        /// This allows dependency injection of any JSON deserialization implementation.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="deserializer">The deserializer function that converts JSON string to DynamicDictionary.</param>
        /// <returns>A new DynamicDictionary deserialized from the JSON string as dynamic type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json or deserializer is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Using with JsonExtension package
        /// dynamic user = DynamicDictionary.Create(json, DynamicDictionaryJsonExtensions.FromJson);
        /// 
        /// // Using with custom deserializer
        /// dynamic user = DynamicDictionary.Create(json, myJson => {
        ///     // Custom deserialization logic
        ///     return new DynamicDictionary { { "parsed", true } };
        /// });
        /// </code>
        /// </example>
        public static dynamic Create(string json, Func<string, DynamicDictionary> deserializer)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be empty", nameof(json));
            if (deserializer == null)
                throw new ArgumentNullException(nameof(deserializer));

            return deserializer(json);
        }

        /// <summary>
        /// Creates an array of DynamicDictionary from a JSON array string using the provided deserializer function.
        /// This allows dependency injection of any JSON array deserialization implementation.
        /// </summary>
        /// <param name="json">The JSON array string to deserialize.</param>
        /// <param name="arrayDeserializer">The deserializer function that converts JSON array string to DynamicDictionary array.</param>
        /// <returns>An array of DynamicDictionary objects deserialized from the JSON string as dynamic type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json or arrayDeserializer is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Using with JsonExtension package
        /// dynamic users = DynamicDictionary.CreateArray(json, DynamicDictionaryJsonExtensions.FromJsonArray);
        /// 
        /// foreach (var user in users)
        /// {
        ///     Console.WriteLine(user.name);
        /// }
        /// </code>
        /// </example>
        public static dynamic CreateArray(string json, Func<string, DynamicDictionary[]> arrayDeserializer)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be empty", nameof(json));
            if (arrayDeserializer == null)
                throw new ArgumentNullException(nameof(arrayDeserializer));

            return arrayDeserializer(json);
        }

        /// <summary>
        /// Creates a DynamicDictionary from a JSON string using a standardized IDynamicsJsonSerializer.
        /// This method provides a clean interface-based approach to JSON deserialization.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="serializer">The IDynamicsJsonSerializer implementation to use for deserialization.</param>
        /// <returns>A new DynamicDictionary deserialized from the JSON string as dynamic type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json or serializer is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Using with SystemTextJsonDynamicsSerializer
        /// var serializer = new SystemTextJsonDynamicsSerializer();
        /// dynamic user = DynamicDictionary.Create(json, serializer);
        /// 
        /// // With custom options
        /// var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        /// var customSerializer = new SystemTextJsonDynamicsSerializer(options, options);
        /// dynamic data = DynamicDictionary.Create(json, customSerializer);
        /// </code>
        /// </example>
        public static dynamic Create(string json, IDynamicsJsonSerializer serializer)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be empty", nameof(json));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            return serializer.Deserialize(json);
        }

        /// <summary>
        /// Creates an array of DynamicDictionary from a JSON array string using a standardized IDynamicsJsonSerializer.
        /// This method provides a clean interface-based approach to JSON array deserialization.
        /// </summary>
        /// <param name="json">The JSON array string to deserialize.</param>
        /// <param name="serializer">The IDynamicsJsonSerializer implementation to use for deserialization.</param>
        /// <returns>An array of DynamicDictionary objects deserialized from the JSON string as dynamic type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json or serializer is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Using with SystemTextJsonDynamicsSerializer
        /// var serializer = new SystemTextJsonDynamicsSerializer();
        /// dynamic users = DynamicDictionary.CreateArray(json, serializer);
        /// 
        /// foreach (var user in users)
        /// {
        ///     Console.WriteLine(user.name);
        /// }
        /// </code>
        /// </example>
        public static dynamic CreateArray(string json, IDynamicsJsonSerializer serializer)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be empty", nameof(json));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            return serializer.DeserializeArray(json);
        }

        /// <summary>
        /// Creates a strongly-typed DynamicDictionary-derived object from a JSON string using a standardized IDynamicsJsonSerializer.
        /// This generic method allows using custom model classes that inherit from DynamicDictionary.
        /// </summary>
        /// <typeparam name="T">The type that inherits from DynamicDictionary and has a parameterless constructor.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="serializer">The IDynamicsJsonSerializer implementation to use for deserialization.</param>
        /// <returns>A new instance of T initialized with data from the JSON string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json or serializer is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Define a custom model class
        /// public class JsonPlaceholderUser : DynamicDictionary
        /// {
        ///     public int Id => GetValue&lt;int&gt;("id");
        ///     public string Name => GetValue&lt;string&gt;("name");
        ///     public string Email => GetValue&lt;string&gt;("email");
        /// }
        /// 
        /// // Use the generic Create method
        /// var serializer = new SystemTextJsonDynamicsSerializer();
        /// var user = DynamicDictionary.Create&lt;JsonPlaceholderUser&gt;(json, serializer);
        /// Console.WriteLine(user.Name); // Strongly-typed property access
        /// Console.WriteLine(user.Email);
        /// </code>
        /// </example>
        public static T Create<T>(string json, IDynamicsJsonSerializer serializer) where T : DynamicDictionary, new()
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be empty", nameof(json));
            serializer = serializer ?? _defaultSerializer;
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            // Deserialize to base DynamicDictionary first
            var baseDictionary = serializer.Deserialize(json);
            
            // Create a new instance of T and assign the data directly
            var instance = new T();
            instance._data = baseDictionary._data;
            
            return instance;
        }

        /// <summary>
        /// Creates an array of strongly-typed DynamicDictionary-derived objects from a JSON array string using a standardized IDynamicsJsonSerializer.
        /// This generic method allows using custom model classes that inherit from DynamicDictionary.
        /// </summary>
        /// <typeparam name="T">The type that inherits from DynamicDictionary and has a parameterless constructor.</typeparam>
        /// <param name="json">The JSON array string to deserialize.</param>
        /// <param name="serializer">The IDynamicsJsonSerializer implementation to use for deserialization.</param>
        /// <returns>An array of T instances initialized with data from the JSON string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json or serializer is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Define a custom model class
        /// public class JsonPlaceholderPost : DynamicDictionary
        /// {
        ///     public int Id => GetValue&lt;int&gt;("id");
        ///     public int UserId => GetValue&lt;int&gt;("userId");
        ///     public string Title => GetValue&lt;string&gt;("title");
        ///     public string Body => GetValue&lt;string&gt;("body");
        /// }
        /// 
        /// // Use the generic CreateArray method
        /// var serializer = new SystemTextJsonDynamicsSerializer();
        /// var posts = DynamicDictionary.CreateArray&lt;JsonPlaceholderPost&gt;(json, serializer);
        /// foreach (var post in posts)
        /// {
        ///     Console.WriteLine($"#{post.Id}: {post.Title}");
        /// }
        /// </code>
        /// </example>
        public static T[] CreateArray<T>(string json, IDynamicsJsonSerializer serializer) where T : DynamicDictionary, new()
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be empty", nameof(json));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            // Deserialize to base DynamicDictionary array first
            var baseArray = serializer.DeserializeArray(json);
            
            // Create array of T and assign data directly
            var result = new T[baseArray.Length];
            for (int i = 0; i < baseArray.Length; i++)
            {
                var instance = new T();
                instance._data = baseArray[i]._data;
                result[i] = instance;
            }
            
            return result;
        }

        /// <summary>
        /// Creates a DynamicDictionary from a JSON string using the default serializer.
        /// If no default serializer has been set, a new SystemTextJsonDynamicsSerializer is used.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A new DynamicDictionary initialized with data from the JSON string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Simple usage with default serializer
        /// var post = DynamicDictionary.Create(json);
        /// Console.WriteLine(post.title);
        /// </code>
        /// </example>
        public static dynamic Create(string json)
        {
            return Create(json, GetDefaultSerializer());
        }

        /// <summary>
        /// Creates an array of DynamicDictionary objects from a JSON array string using the default serializer.
        /// If no default serializer has been set, a new SystemTextJsonDynamicsSerializer is used.
        /// </summary>
        /// <param name="json">The JSON array string to deserialize.</param>
        /// <returns>An array of DynamicDictionary objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Simple usage with default serializer
        /// var posts = DynamicDictionary.CreateArray(json);
        /// foreach (var post in posts)
        /// {
        ///     Console.WriteLine(post.title);
        /// }
        /// </code>
        /// </example>
        public static dynamic CreateArray(string json)
        {
            return CreateArray(json, GetDefaultSerializer());
        }

        /// <summary>
        /// Creates a strongly-typed DynamicDictionary-derived object from a JSON string using the default serializer.
        /// If no default serializer has been set, a new SystemTextJsonDynamicsSerializer is used.
        /// </summary>
        /// <typeparam name="T">The type that inherits from DynamicDictionary and has a parameterless constructor.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A new instance of T initialized with data from the JSON string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Define a custom model class
        /// public class JsonPlaceholderUser : DynamicDictionary
        /// {
        ///     public int Id => GetValue&lt;int&gt;("id");
        ///     public string Name => GetValue&lt;string&gt;("name");
        /// }
        /// 
        /// // Simple usage with default serializer
        /// var user = DynamicDictionary.Create&lt;JsonPlaceholderUser&gt;(json);
        /// Console.WriteLine(user.Name);
        /// </code>
        /// </example>
        public static T Create<T>(string json) where T : DynamicDictionary, new()
        {
            return Create<T>(json, GetDefaultSerializer());
        }

        /// <summary>
        /// Creates an array of strongly-typed DynamicDictionary-derived objects from a JSON array string using the default serializer.
        /// If no default serializer has been set, a new SystemTextJsonDynamicsSerializer is used.
        /// </summary>
        /// <typeparam name="T">The type that inherits from DynamicDictionary and has a parameterless constructor.</typeparam>
        /// <param name="json">The JSON array string to deserialize.</param>
        /// <returns>An array of T instances initialized with data from the JSON string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json is null.</exception>
        /// <exception cref="ArgumentException">Thrown when json is empty.</exception>
        /// <example>
        /// <code>
        /// // Define a custom model class
        /// public class JsonPlaceholderPost : DynamicDictionary
        /// {
        ///     public int Id => GetValue&lt;int&gt;("id");
        ///     public string Title => GetValue&lt;string&gt;("title");
        /// }
        /// 
        /// // Simple usage with default serializer
        /// var posts = DynamicDictionary.CreateArray&lt;JsonPlaceholderPost&gt;(json);
        /// foreach (var post in posts)
        /// {
        ///     Console.WriteLine($"#{post.Id}: {post.Title}");
        /// }
        /// </code>
        /// </example>
        public static T[] CreateArray<T>(string json) where T : DynamicDictionary, new()
        {
            return CreateArray<T>(json, GetDefaultSerializer());
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DynamicDictionary class.
        /// </summary>
        public DynamicDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DynamicDictionary class with key-value pairs.
        /// </summary>
        /// <param name="items">The collection of key-value pairs to initialize the dictionary with.</param>
        public DynamicDictionary(IEnumerable<KeyValuePair<string, object>> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    _data[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the DynamicDictionary class with an existing dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to initialize from.</param>
        public DynamicDictionary(IDictionary<string, object> dictionary)
        {
            if (dictionary != null)
            {
                foreach (var item in dictionary)
                {
                    _data[item.Key] = item.Value;
                }
            }
        }

        #endregion

        #region Dynamic Object Overrides

        /// <summary>
        /// Provides the implementation for operations that get member values.
        /// Applies registered value resolvers to transform the retrieved value.
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_data.TryGetValue(binder.Name, out var value))
            {
                // Apply resolvers to transform the value
                value = ApplyResolvers(value);

                if (value is IDictionary<string, object> dict)
                {
                    result = new DynamicDictionary(dict);
                    return true;
                }

                if (value is IList list)
                {
                    result = list;
                    return true;
                }

                if (value is Array array)
                {
                    result = array;
                    return true;
                }

                result = value;
                return true;
            }

            result = null;
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values.
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _data[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that invoke a member.
        /// Supports invoking delegate values stored in the dictionary.
        /// </summary>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            string name = binder.Name;
            if (_data.ContainsKey(name) && _data[name] is Delegate)
            {
                result = (_data[name] as Delegate).DynamicInvoke(args);
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _data.Keys;
        }

        #endregion

        #region Indexer and Key Operations

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <remarks>
        /// Supports nested path notation for accessing nested objects and arrays:
        /// - Use dot (.) for object property access: "user.name"
        /// - Use brackets ([]) for array index access: "items[0]" or "users[1].address.city"
        /// </remarks>
        public object this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    return null;
                return GetNestedValue(key);
            }
            set
            {
                if (string.IsNullOrEmpty(key))
                    return;
                SetNestedValue(key, value);
            }
        }

        /// <summary>
        /// Gets a value using nested path notation.
        /// </summary>
        private object GetNestedValue(string path)
        {
            if (!path.Contains('.') && !path.Contains('['))
            {
                if (_data.TryGetValue(path, out var value))
                {
                    // Apply resolvers to transform the value
                    value = ApplyResolvers(value);
                    return value;
                }
                return null;
            }

            var segments = ParsePath(path);
            object current = this;

            foreach (var segment in segments)
            {
                if (current == null)
                    return null;

                if (segment.IsArrayIndex)
                {
                    if (current is IList list)
                    {
                        if (segment.Index >= 0 && segment.Index < list.Count)
                        {
                            current = list[segment.Index];
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (current is Array array)
                    {
                        if (segment.Index >= 0 && segment.Index < array.Length)
                        {
                            current = array.GetValue(segment.Index);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (current is DynamicDictionary dd)
                    {
                        current = dd._data.TryGetValue(segment.Key, out var value) ? value : null;
                    }
                    else if (current is IDictionary<string, object> dict)
                    {
                        current = dict.TryGetValue(segment.Key, out var value) ? value : null;
                    }
                    else
                    {
                        return null;
                    }
                }

                // Apply resolvers to transform the current value
                if (current != null)
                {
                    current = ApplyResolvers(current);
                }
            }

            return current;
        }

        /// <summary>
        /// Sets a value using nested path notation.
        /// Creates intermediate objects as needed.
        /// </summary>
        private void SetNestedValue(string path, object value)
        {
            if (!path.Contains('.') && !path.Contains('['))
            {
                _data[path] = value;
                return;
            }

            var segments = ParsePath(path);
            if (segments.Count == 0)
                return;

            object current = this;
            for (int i = 0; i < segments.Count - 1; i++)
            {
                var segment = segments[i];

                if (segment.IsArrayIndex)
                {
                    throw new NotSupportedException("Cannot set values through array indices in the middle of a path.");
                }

                if (current is DynamicDictionary dd)
                {
                    if (!dd._data.TryGetValue(segment.Key, out var next) || next == null)
                    {
                        if (i + 1 < segments.Count && segments[i + 1].IsArrayIndex)
                        {
                            next = new List<object>();
                        }
                        else
                        {
                            next = new DynamicDictionary();
                        }
                        dd._data[segment.Key] = next;
                    }
                    current = next;
                }
                else
                {
                    throw new InvalidOperationException($"Cannot navigate through non-dictionary object at '{segment.Key}'");
                }
            }

            var lastSegment = segments[segments.Count - 1];
            if (lastSegment.IsArrayIndex)
            {
                throw new NotSupportedException("Cannot set array element directly. Set the parent array instead.");
            }

            if (current is DynamicDictionary dd2)
            {
                dd2._data[lastSegment.Key] = value;
            }
            else
            {
                throw new InvalidOperationException("Cannot set value on non-dictionary object.");
            }
        }

        /// <summary>
        /// Parses a path string into path segments.
        /// </summary>
        private List<PathSegment> ParsePath(string path)
        {
            var segments = new List<PathSegment>();
            var currentKey = new StringBuilder();

            for (int i = 0; i < path.Length; i++)
            {
                char c = path[i];

                if (c == '.')
                {
                    if (currentKey.Length > 0)
                    {
                        segments.Add(new PathSegment { Key = currentKey.ToString() });
                        currentKey.Clear();
                    }
                }
                else if (c == '[')
                {
                    if (currentKey.Length > 0)
                    {
                        segments.Add(new PathSegment { Key = currentKey.ToString() });
                        currentKey.Clear();
                    }

                    int endIdx = path.IndexOf(']', i);
                    if (endIdx == -1)
                        throw new ArgumentException($"Unmatched '[' in path: {path}");

                    var indexStr = path.Substring(i + 1, endIdx - i - 1);
                    if (int.TryParse(indexStr, out var index))
                    {
                        segments.Add(new PathSegment { Index = index, IsArrayIndex = true });
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid array index '{indexStr}' in path: {path}");
                    }

                    i = endIdx;
                }
                else
                {
                    currentKey.Append(c);
                }
            }

            if (currentKey.Length > 0)
            {
                segments.Add(new PathSegment { Key = currentKey.ToString() });
            }

            return segments;
        }

        /// <summary>
        /// Represents a segment in a navigation path.
        /// </summary>
        private class PathSegment
        {
            public string Key { get; set; }
            public int Index { get; set; }
            public bool IsArrayIndex { get; set; }
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        public bool TryGetValue(string key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the value associated with the specified key as the specified type.
        /// </summary>
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            if (!_data.TryGetValue(key, out var value) || value == null)
                return defaultValue;

            try
            {
                // Apply resolvers first
                value = ApplyResolvers(value);

                if (value is T directValue)
                    return directValue;

                if (value is DynamicDictionary dict && typeof(T).IsSubclassOf(typeof(DynamicDictionary)))
                {
                    var instance = (DynamicDictionary)Activator.CreateInstance(typeof(T));
                    foreach (var kvp in dict._data)
                    {
                        instance[kvp.Key] = kvp.Value;
                    }
                    return (T)(object)instance;
                }

                var targetType = typeof(T);
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    targetType = underlyingType;
                }

                if (targetType.IsEnum)
                {
                    object enumValue;
                    if (value is string strValue)
                    {
                        enumValue = Enum.Parse(targetType, strValue, true);
                    }
                    else
                    {
                        var intValue = Convert.ToInt32(value);
                        enumValue = Enum.ToObject(targetType, intValue);
                    }

                    if (underlyingType != null)
                    {
                        return (T)Activator.CreateInstance(typeof(T), enumValue);
                    }
                    return (T)enumValue;
                }

                if (IsNumericType(value.GetType()) && IsNumericType(targetType))
                {
                    var converted = ConvertNumericType(value, targetType);
                    if (underlyingType != null)
                    {
                        return (T)Activator.CreateInstance(typeof(T), converted);
                    }
                    return (T)converted;
                }

                var result = Convert.ChangeType(value, targetType);
                if (underlyingType != null)
                {
                    return (T)Activator.CreateInstance(typeof(T), result);
                }
                return (T)result;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Determines whether the specified type is a numeric type.
        /// </summary>
        private static bool IsNumericType(Type type)
        {
            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal);
        }

        /// <summary>
        /// Converts a value to the specified numeric type.
        /// </summary>
        private static object ConvertNumericType(object value, Type targetType)
        {
            if (targetType == typeof(byte)) return Convert.ToByte(value);
            if (targetType == typeof(sbyte)) return Convert.ToSByte(value);
            if (targetType == typeof(short)) return Convert.ToInt16(value);
            if (targetType == typeof(ushort)) return Convert.ToUInt16(value);
            if (targetType == typeof(int)) return Convert.ToInt32(value);
            if (targetType == typeof(uint)) return Convert.ToUInt32(value);
            if (targetType == typeof(long)) return Convert.ToInt64(value);
            if (targetType == typeof(ulong)) return Convert.ToUInt64(value);
            if (targetType == typeof(float)) return Convert.ToSingle(value);
            if (targetType == typeof(double)) return Convert.ToDouble(value);
            if (targetType == typeof(decimal)) return Convert.ToDecimal(value);

            throw new InvalidCastException($"Cannot convert to {targetType.Name}");
        }

        #endregion

        #region Collection Operations

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        public void Add(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            _data[key] = value;
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes a key from the dictionary.
        /// </summary>
        public bool Remove(string key)
        {
            return _data.Remove(key);
        }

        /// <summary>
        /// Removes a key-value pair from the dictionary.
        /// </summary>
        public bool Remove(KeyValuePair<string, object> item)
        {
            if (_data.TryGetValue(item.Key, out var existingValue) &&
                Equals(existingValue, item.Value))
            {
                return _data.Remove(item.Key);
            }
            return false;
        }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific key-value pair.
        /// </summary>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return _data.TryGetValue(item.Key, out var value) && Equals(value, item.Value);
        }

        /// <summary>
        /// Copies all key-value pairs to an array.
        /// </summary>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _data.Count) throw new ArgumentException("Not enough space in array");

            int index = arrayIndex;
            foreach (var kvp in _data)
            {
                array[index++] = kvp;
            }
        }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Creates a copy of the DynamicDictionary.
        /// </summary>
        public DynamicDictionary Clone(bool deepCopy = false)
        {
            var clonedDic = new DynamicDictionary();

            if (deepCopy)
            {
                foreach (var kvp in _data)
                {
                    if (kvp.Value is DynamicDictionary nestedDict)
                    {
                        clonedDic._data[kvp.Key] = nestedDict.Clone(true);
                    }
                    else if (kvp.Value is IList list)
                    {
                        var newList = new List<object>();
                        foreach (var item in list)
                        {
                            if (item is DynamicDictionary itemDict)
                            {
                                newList.Add(itemDict.Clone(true));
                            }
                            else
                            {
                                newList.Add(item);
                            }
                        }
                        clonedDic._data[kvp.Key] = newList;
                    }
                    else
                    {
                        clonedDic._data[kvp.Key] = kvp.Value;
                    }
                }
            }
            else
            {
                foreach (var kvp in _data)
                {
                    clonedDic._data[kvp.Key] = kvp.Value;
                }
            }

            return clonedDic;
        }

        /// <summary>
        /// Serializes the current DynamicDictionary to a JSON string using a specific serializer.
        /// </summary>
        /// <param name="serializer">The serializer to use for serialization.</param>
        /// <returns>A JSON string representation of the DynamicDictionary.</returns>
        /// <exception cref="ArgumentNullException">Thrown when serializer is null.</exception>
        /// <example>
        /// <code>
        /// var dict = new DynamicDictionary();
        /// dict["name"] = "John";
        /// dict["age"] = 30;
        /// 
        /// var customSerializer = new SystemTextJsonDynamicsSerializer(
        ///     new JsonSerializerOptions { WriteIndented = true }
        /// );
        /// string json = dict.Serialize(customSerializer);
        /// </code>
        /// </example>
        public string Serialize(IDynamicsJsonSerializer serializer = null)
        {
            serializer = serializer ?? _defaultSerializer;
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            
            return serializer.Serialize(this);
        }

        /// <summary>
        /// Returns a string representation of the DynamicDictionary.
        /// </summary>
        public override string ToString()
        {
            return ToString(" : ", true);
        }

        /// <summary>
        /// Returns a string representation of the DynamicDictionary with custom formatting.
        /// </summary>
        public string ToString(string separator = " : ", bool includeCount = true)
        {
            if (_data == null || _data.Count == 0)
                return includeCount ? "Empty DynamicDictionary (0 items)" : "Empty";

            var sb = new StringBuilder();

            if (includeCount)
                sb.AppendLine($"DynamicDictionary ({_data.Count} items):");

            foreach (var kvp in _data.OrderBy(x => x.Key))
            {
                var valueStr = kvp.Value?.ToString() ?? "<null>";
                sb.AppendLine($"{kvp.Key}{separator}{valueStr}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the first value in the dictionary.
        /// </summary>
        public object GetFirstValue()
        {
            return _data.FirstOrDefault().Value;
        }

        /// <summary>
        /// Returns the first value as the specified type.
        /// </summary>
        public T GetFirstValue<T>(T defaultValue = default(T))
        {
            var firstValue = GetFirstValue();
            if (firstValue == null)
                return defaultValue;

            try
            {
                if (firstValue is T directValue)
                    return directValue;

                return (T)Convert.ChangeType(firstValue, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Merges another DynamicDictionary into this one.
        /// </summary>
        public void Merge(DynamicDictionary src, bool overwriteExisting = true, bool deepMerge = false)
        {
            if (src == null)
                return;

            foreach (var (k, v) in src._data)
            {
                if (!deepMerge || !_data.TryGetValue(k, out var existing)
                    || existing is not IDictionary<string, object> eDict
                    || v is not IDictionary<string, object> vDict)
                {
                    if (overwriteExisting || !_data.ContainsKey(k))
                    {
                        _data[k] = v;
                    }
                }
                else
                {
                    new DynamicDictionary(eDict).Merge(new DynamicDictionary(vDict), overwriteExisting, true);
                }
            }
        }

        /// <summary>
        /// Removes items that match the specified condition.
        /// </summary>
        public int RemoveWhere(Func<string, object, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var keysToRemove = _data.Where(kvp => predicate(kvp.Key, kvp.Value))
                                   .Select(kvp => kvp.Key)
                                   .ToList();

            foreach (var key in keysToRemove)
            {
                _data.Remove(key);
            }

            return keysToRemove.Count;
        }

        #endregion
    }
}
