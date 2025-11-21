# OneCiel.System.Dynamics.JsonExtension

JSON serialization and deserialization extensions for OneCiel.System.Dynamics. Provides seamless System.Text.Json integration with DynamicDictionary for .NET 8 and .NET 9, including automatic JsonElement type conversion via the `JsonElementValueResolver`.

## Installation

```bash
dotnet add package OneCiel.System.Dynamics.JsonExtension
```

## Requirements

- OneCiel.System.Dynamics (required dependency)
- .NET 8.0 or later
- .NET 9.0 or later

## Features

- **JSON String Conversion**: Convert DynamicDictionary to/from JSON strings
- **File Operations**: Save and load JSON files asynchronously and synchronously
- **Automatic Type Conversion**: JsonElement values are automatically converted to appropriate .NET types via resolver
- **JsonConverter Support**: Use with System.Text.Json serializer options
- **Nested Object Support**: Seamlessly handles nested objects and arrays
- **Flexible Formatting**: Optional indented output for readability
- **Comment Handling**: Support for JSON comments and trailing commas when parsing
- **Extensible Resolvers**: Provides `JsonElementValueResolver` for custom JSON type handling

## Quick Start

### Modern Fluent API

JSON support is automatically initialized on first use. No manual setup required:

```csharp
using OneCiel.System.Dynamics;

// Modern extension method - just call .ToDynamicDictionary() on any JSON string!
var dict = @"{ ""name"": ""John"" }".ToDynamicDictionary();
// JsonElementValueResolver is automatically registered
```

### Fluent Extension Methods

This package provides intuitive string extension methods:

```csharp
// Parse JSON object
var user = jsonString.ToDynamicDictionary();

// Parse JSON array
var users = jsonArrayString.ToDynamicArray();
```

### JSON String Operations

```csharp
using OneCiel.System.Dynamics;

// Create a DynamicDictionary
var dict = new DynamicDictionary
{
    { "name", "John Doe" },
    { "age", 30 },
    { "active", true }
};

// Convert to JSON string
string json = dict.ToJson();
// Output: {"name":"John Doe","age":30,"active":true}

// Parse from JSON string - Modern fluent API!
var parsed = json.ToDynamicDictionary();
string name = parsed.GetValue<string>("name"); // "John Doe"
int age = parsed.GetValue<int>("age"); // 30
```

### File Operations

```csharp
// Asynchronous save
await dict.ToJsonFileAsync("data.json");

// Asynchronous load
var loaded = await DynamicDictionaryJsonExtensions.FromJsonFileAsync("data.json");

// Synchronous operations
dict.ToJsonFile("sync.json");
var syncLoaded = DynamicDictionaryJsonExtensions.FromJsonFile("sync.json");
```

### Nested Objects and Arrays

```csharp
string json = @"
{
    ""user"": {
        ""id"": 1,
        ""name"": ""Alice"",
        ""email"": ""alice@example.com""
    },
    ""items"": [
        { ""id"": 1, ""title"": ""Item 1"" },
        { ""id"": 2, ""title"": ""Item 2"" }
    ]
}";

// Modern fluent API - beautifully simple!
var data = json.ToDynamicDictionary();

// Access nested properties
int userId = data.GetValue<int>("user.id"); // 1
string userName = data.GetValue<string>("user.name"); // "Alice"

// Access array elements
string firstItemTitle = data.GetValue<string>("items[0].title"); // "Item 1"
int secondItemId = data.GetValue<int>("items[1].id"); // 2
```

### Using with System.Text.Json

```csharp
using System.Text.Json;

// Configure JsonSerializerOptions with the converter
var options = new JsonSerializerOptions
{
    Converters = 
    { 
        new DynamicDictionaryJsonConverter()
    },
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

// Serialize
var dict = new DynamicDictionary { { "message", "Hello" } };
string json = JsonSerializer.Serialize(dict, options);

// Deserialize (automatically uses JsonElementValueResolver)
var deserialized = JsonSerializer.Deserialize<DynamicDictionary>(json, options);
```

### Pretty Printing

```csharp
var dict = new DynamicDictionary
{
    { "user", new DynamicDictionary { { "name", "Bob" }, { "age", 25 } } },
    { "items", new List<object> { "a", "b", "c" } }
};

// Indented JSON (human readable)
string prettyJson = dict.ToJson(writeIndented: true);

// Compact JSON
string compactJson = dict.ToJson(writeIndented: false);
```

## JSON Element Value Resolver

The `JsonElementValueResolver` is automatically registered and provides automatic conversion of JsonElement values:

### Automatic Type Conversion

When JSON data is loaded, JsonElement values are automatically converted:

| JSON Type | .NET Type |
|-----------|-----------|
| Object | `DynamicDictionary` |
| Array | `object[]` or `DynamicDictionary[]` |
| String | `string` |
| Number | `decimal`, `long`, `int`, or `double` |
| Boolean | `bool` |
| Null | `null` |

### Dynamic Member Access with Conversion

```csharp
// Modern fluent API with dynamic type
string json = @"{ ""name"": ""Test"", ""count"": 42 }";
dynamic data = json.ToDynamicDictionary();

// Values are automatically converted from JsonElement
string name = data.name; // "Test" (string)
int count = data.count; // 42 (int)
```

### Resolver Details

The `JsonElementValueResolver`:
- Checks if a value is a JsonElement
- Automatically converts JsonElement to appropriate .NET types
- Recursively processes nested objects and arrays
- Preserves typed arrays (all DynamicDictionary array elements)
- Is thread-safe and global to all DynamicDictionary instances

## API Reference

### Extension Methods

#### String Conversion (Modern Fluent API)

```csharp
// Parse JSON object - Extension method on string!
DynamicDictionary ToDynamicDictionary(this string json)
DynamicDictionary ToDynamicDictionary(this string json, JsonSerializerOptions options)

// Parse JSON array - Extension method on string!
DynamicDictionary[] ToDynamicArray(this string json)
DynamicDictionary[] ToDynamicArray(this string json, JsonSerializerOptions options)

// Convert to JSON string
string ToJson(this DynamicDictionary dictionary, bool writeIndented = true)
string ToJson(this DynamicDictionary dictionary, JsonSerializerOptions options)
```

#### File Operations

```csharp
// Save to file asynchronously
static Task ToJsonFileAsync(this DynamicDictionary dictionary, string filePath, bool writeIndented = true)

// Save to file synchronously
static void ToJsonFile(this DynamicDictionary dictionary, string filePath, bool writeIndented = true)

// Load from file asynchronously
static Task<DynamicDictionary> FromJsonFileAsync(string filePath)

// Load from file synchronously
static DynamicDictionary FromJsonFile(string filePath)

// Initialize JSON support (called automatically)
static void InitializeJsonSupport()
```

### JsonElementValueResolver

```csharp
public class JsonElementValueResolver : IValueResolver
{
    public bool CanResolve(object value) // Returns true for JsonElement
    public object Resolve(object value) // Converts JsonElement to .NET type
}
```

### JsonConverter Class

```csharp
public class DynamicDictionaryJsonConverter : JsonConverter<DynamicDictionary>
{
    public override bool CanConvert(Type typeToConvert)
    public override DynamicDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    public override void Write(Utf8JsonWriter writer, DynamicDictionary value, JsonSerializerOptions options)
}
```

## Supported JSON Types

| JSON Type | .NET Type | Example |
|-----------|-----------|---------|
| Object | `DynamicDictionary` | `{"key":"value"}` |
| Array | `object[]` or `DynamicDictionary[]` | `[1, 2, 3]` |
| String | `string` | `"text"` |
| Number | `decimal`, `long`, `int`, or `double` | `123`, `123.45` |
| Boolean | `bool` | `true`, `false` |
| Null | `null` | `null` |

## Error Handling

All methods provide proper error handling:

```csharp
try
{
    var dict = "invalid json".ToDynamicDictionary();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("JSON parsing failed: " + ex.Message);
}

try
{
    await dict.ToJsonFileAsync("/invalid/path/file.json");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("File write failed: " + ex.Message);
}
```

## Advanced: Custom JSON Resolvers

You can also create custom value resolvers for special JSON handling:

```csharp
public class CustomJsonResolver : IValueResolver
{
    public bool CanResolve(object value)
    {
        return value is MySpecialJsonType;
    }

    public object Resolve(object value)
    {
        var special = (MySpecialJsonType)value;
        // Custom transformation logic
        return TransformToDesiredType(special);
    }
}

// Register for use alongside JsonElementValueResolver
DynamicDictionary.RegisterValueResolver(new CustomJsonResolver());
```

## License

MIT License
