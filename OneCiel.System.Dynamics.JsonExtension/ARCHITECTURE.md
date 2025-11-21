# DynamicDictionary - Standardized JSON Interfaces

## Architecture Overview

The refactored JSON functionality uses standardized interfaces for clean, testable, and extensible code.

The interfaces are defined in the **core library** (`OneCiel.System.Dynamics`) and the System.Text.Json implementations are provided in the **JSON extension library** (`OneCiel.System.Dynamics.JsonExtension`).

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│   OneCiel.System.Dynamics (Core Library)                    │
│   - IJsonSerializer interface                               │
│   - IJsonDeserializer interface                             │
└──────────────┬──────────────────────────────────────────────┘
               │
               │ Implemented by
               │
┌──────────────▼──────────────────────────────────────────────┐
│   OneCiel.System.Dynamics.JsonExtension                      │
│   - SystemTextJsonSerializer : IJsonSerializer              │
│   - SystemTextJsonDeserializer : IJsonDeserializer          │
│   - DynamicDictionaryJsonConverter : JsonConverter          │
│   - DynamicDictionaryJsonExtensions (static methods)        │
└─────────────────────────────────────────────────────────────┘
```

## Key Interfaces (in Core Library)

### IJsonSerializer
```csharp
public interface IJsonSerializer
{
    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    string Serialize(object obj);
}
```

### IJsonDeserializer
```csharp
public interface IJsonDeserializer
{
    /// <summary>
    /// Deserializes a JSON string to a DynamicDictionary.
    /// </summary>
    DynamicDictionary Deserialize(string json);

    /// <summary>
    /// Deserializes a JSON array string to an array of DynamicDictionary.
    /// </summary>
    DynamicDictionary[] DeserializeArray(string json);
}
```

## Standard Implementations (in JsonExtension)

### SystemTextJsonSerializer
```csharp
// Default implementation using System.Text.Json
var serializer = new SystemTextJsonSerializer();
string json = serializer.Serialize(dictionary.Data);

// With custom options
var options = new JsonSerializerOptions 
{ 
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
};
var customSerializer = new SystemTextJsonSerializer(options);
```

### SystemTextJsonDeserializer
```csharp
// Default implementation
var deserializer = new SystemTextJsonDeserializer();
var dict = deserializer.Deserialize(json);
var array = deserializer.DeserializeArray(jsonArray);

// With custom options
var options = new JsonSerializerOptions 
{ 
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true 
};
var customDeserializer = new SystemTextJsonDeserializer(options);
```

## Usage Patterns

### Pattern 1: Default Behavior (Simplest)
```csharp
// Uses built-in defaults
var dict = DynamicDictionaryJsonExtensions.FromJson(json);
string json = dict.ToJson();
```

### Pattern 2: Custom JsonSerializerOptions
```csharp
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNameCaseInsensitive = true
};

var dict = DynamicDictionaryJsonExtensions.FromJson(json, options);
string json = dict.ToJson(options);
```

### Pattern 3: Custom Serializer/Deserializer
```csharp
var serializer = new SystemTextJsonSerializer(myOptions);
var deserializer = new SystemTextJsonDeserializer(myOptions);

string json = dict.ToJson(serializer);
var dict = DynamicDictionaryJsonExtensions.FromJson(json, deserializer);
```

### Pattern 4: Global Configuration
```csharp
// Set globally for the entire application
var globalSerializer = new SystemTextJsonSerializer(myOptions);
var globalDeserializer = new SystemTextJsonDeserializer(myOptions);

DynamicDictionaryJsonExtensions.SetJsonSerializer(globalSerializer);
DynamicDictionaryJsonExtensions.SetJsonDeserializer(globalDeserializer);

// Now all operations use these defaults
var dict = DynamicDictionaryJsonExtensions.FromJson(json);
string json = dict.ToJson();
```

### Pattern 5: Custom Implementation
```csharp
// Implement your own serializer
public class MyCustomSerializer : IJsonSerializer
{
    public string Serialize(object obj)
    {
        // Custom serialization logic
    }
}

var customSerializer = new MyCustomSerializer();
string json = dict.ToJson(customSerializer);
```

## Method Overloads

### FromJson (Multiple Overloads)
```csharp
// 1. Default options
DynamicDictionary FromJson(string json)

// 2. Custom JsonSerializerOptions
DynamicDictionary FromJson(string json, JsonSerializerOptions options)

// 3. Custom deserializer
DynamicDictionary FromJson(string json, IJsonDeserializer deserializer)
```

### FromJsonArray (Multiple Overloads)
```csharp
// 1. Default options
DynamicDictionary[] FromJsonArray(string json)

// 2. Custom JsonSerializerOptions
DynamicDictionary[] FromJsonArray(string json, JsonSerializerOptions options)

// 3. Custom deserializer
DynamicDictionary[] FromJsonArray(string json, IJsonDeserializer deserializer)
```

### ToJson (Multiple Overloads)
```csharp
// 1. Default options
string ToJson(this DynamicDictionary dictionary)

// 2. Custom JsonSerializerOptions
string ToJson(this DynamicDictionary dictionary, JsonSerializerOptions options)
```

### File Operations (Similar Pattern)
```csharp
// Async with default options
Task ToJsonFileAsync(this DynamicDictionary dictionary, string filePath)

// Async with custom options
Task ToJsonFileAsync(this DynamicDictionary dictionary, string filePath, JsonSerializerOptions options)

// Async with custom serializer
Task ToJsonFileAsync(this DynamicDictionary dictionary, string filePath, IJsonSerializer serializer)

// Sync variants also available
void ToJsonFile(this DynamicDictionary dictionary, string filePath)
void ToJsonFile(this DynamicDictionary dictionary, string filePath, JsonSerializerOptions options)
void ToJsonFile(this DynamicDictionary dictionary, string filePath, IJsonSerializer serializer)
```

## Benefits of This Design

### 1. **Separation of Concerns**
- Interfaces in core library (framework-independent)
- Implementations in extension library (depends on System.Text.Json)
- Clean dependency direction

### 2. **Testability**
Easy to mock and test with custom serializer/deserializer implementations.

```csharp
// Mock serializer for testing
public class MockJsonDeserializer : IJsonDeserializer
{
    public DynamicDictionary Deserialize(string json)
    {
        // Return test data
    }
    
    public DynamicDictionary[] DeserializeArray(string json)
    {
        // Return test array
    }
}

// Use in tests
[Test]
public void TestWithMockDeserializer()
{
    var mockDeserializer = new MockJsonDeserializer();
    var dict = DynamicDictionaryJsonExtensions.FromJson(json, mockDeserializer);
    Assert.IsNotNull(dict);
}
```

### 3. **Flexibility**
Easily swap implementations or add custom logic.

```csharp
// Use different serializers for different scenarios
var compactSerializer = new SystemTextJsonSerializer(compactOptions);
var prettySerializer = new SystemTextJsonSerializer(prettyOptions);
var customSerializer = new MyCustomJsonSerializer();

var compact = dict.ToJson(compactSerializer);
var pretty = dict.ToJson(prettySerializer);
var custom = dict.ToJson(customSerializer);
```

### 4. **No Code Duplication**
All JSON operations use the same standardized interfaces.

### 5. **Extensibility**
Implement custom serializers for:
- Different JSON libraries (Newtonsoft.Json, etc.)
- Custom formatting
- Validation
- Transformation
- Compression

```csharp
public class NewtonsoftJsonDeserializer : IJsonDeserializer
{
    public DynamicDictionary Deserialize(string json)
    {
        var jObject = JObject.Parse(json);
        var dict = new DynamicDictionary();
        // Convert JObject to DynamicDictionary
        return dict;
    }
    
    public DynamicDictionary[] DeserializeArray(string json)
    {
        var jArray = JArray.Parse(json);
        // Convert JArray to array of DynamicDictionary
        return array;
    }
}
```

### 6. **Clean Dependency Management**
- Core library has NO external dependencies
- Extension library only depends on System.Text.Json (built-in)
- Can use interfaces without pulling in JSON extension

## Examples

### Simple Serialization
```csharp
var dict = new DynamicDictionary 
{ 
    { "name", "John" },
    { "age", 30 }
};

// Simple
string json = dict.ToJson();

// Compact
var compactOptions = new JsonSerializerOptions { WriteIndented = false };
string compact = dict.ToJson(compactOptions);
```

### REST API Integration
```csharp
var postJson = await httpClient.GetStringAsync("api/posts/1");

// Use custom options for API responses
var apiOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip
};

var post = DynamicDictionaryJsonExtensions.FromJson(postJson, apiOptions);
var title = post["title"];
```

### Global Configuration for Application
```csharp
// Startup: Configure once
var appSerializer = new SystemTextJsonSerializer(new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

var appDeserializer = new SystemTextJsonDeserializer(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

DynamicDictionaryJsonExtensions.SetJsonSerializer(appSerializer);
DynamicDictionaryJsonExtensions.SetJsonDeserializer(appDeserializer);

// Throughout application: Use defaults
var dict1 = DynamicDictionaryJsonExtensions.FromJson(json1);
var dict2 = DynamicDictionaryJsonExtensions.FromJson(json2);
var json3 = dict3.ToJson();
```

## File Structure

```
OneCiel.System.Dynamics/
└── JsonSerializationInterfaces.cs
    ├── IJsonSerializer (interface)
    └── IJsonDeserializer (interface)

OneCiel.System.Dynamics.JsonExtension/
├── SystemTextJsonImplementations.cs
│   ├── SystemTextJsonSerializer : IJsonSerializer
│   └── SystemTextJsonDeserializer : IJsonDeserializer
├── DynamicDictionaryJsonExtensions.cs
│   └── Static extension methods for convenience
├── DynamicDictionaryJsonConverter.cs
│   └── JsonConverter<DynamicDictionary> for System.Text.Json
└── ARCHITECTURE.md (this file)
```

## Comparison: Old vs New

### Old Approach
```csharp
// Scattered JSON logic in extensions
var dict = DynamicDictionaryJsonExtensions.FromJson(json);
var json = DynamicDictionaryJsonExtensions.ToJson(dict);
// Hard to test, hard to customize
// Interfaces in extension, not core
```

### New Approach
```csharp
// Interfaces in core library
public interface IJsonSerializer { }
public interface IJsonDeserializer { }

// Clean implementations in extension
public class SystemTextJsonSerializer : IJsonSerializer { }
public class SystemTextJsonDeserializer : IJsonDeserializer { }

// Easy to customize and test
var deserializer = new SystemTextJsonDeserializer(options);
var dict = DynamicDictionaryJsonExtensions.FromJson(json, deserializer);

// Or use defaults
var dict = DynamicDictionaryJsonExtensions.FromJson(json);
```

## Migration Guide

If you were using the old API, migration is straightforward:

```csharp
// Old
var dict = DynamicDictionaryJsonExtensions.FromJson(json);

// New (same, uses defaults)
var dict = DynamicDictionaryJsonExtensions.FromJson(json);

// New (with custom options)
var dict = DynamicDictionaryJsonExtensions.FromJson(json, customOptions);

// Old
var json = dict.ToJson();

// New (same, uses defaults)
var json = dict.ToJson();

// New (with custom options)
var json = dict.ToJson(customOptions);
```

All old code continues to work with the new architecture!

## Best Practices

1. **Use Extension Methods for Convenience**
   ```csharp
   var dict = DynamicDictionaryJsonExtensions.FromJson(json);
   ```

2. **Pass Options for Customization**
   ```csharp
   var dict = DynamicDictionaryJsonExtensions.FromJson(json, options);
   ```

3. **Configure Globally Once**
   ```csharp
   // In startup
   DynamicDictionaryJsonExtensions.SetJsonDeserializer(deserializer);
   ```

4. **Implement Custom Interfaces When Needed**
   ```csharp
   public class CustomSerializer : IJsonSerializer { }
   ```

5. **Use Type-Safe Methods on DynamicDictionary**
   ```csharp
   var value = dict.GetValue<T>("key", default);
   ```

## Summary

The standardized JSON interface design provides:
- ✅ Clean, maintainable code
- ✅ Easy testing with mocks
- ✅ Flexible customization
- ✅ No code duplication
- ✅ Easy extension
- ✅ Type safety
- ✅ Backward compatible
- ✅ Interfaces in core library
- ✅ Implementations in extension library

Perfect for REST APIs, Kafka messages, MongoDB documents, and any JSON-based data!
