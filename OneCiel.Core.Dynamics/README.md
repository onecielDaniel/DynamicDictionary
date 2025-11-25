# OneCiel.Core.Dynamics

A flexible and dynamic dictionary implementation for .NET Standard 2.1 that provides convenient access to loosely-typed data structures with extensible type resolution.

## Installation

```bash
dotnet add package OneCiel.Core.Dynamics
```

## Features

- **Dynamic Object Access**: Access dictionary properties as dynamic members
- **Nested Property Navigation**: Support for deep object navigation using dot notation and array indexing
- **Case-Insensitive Keys**: All key lookups are performed case-insensitively
- **Type-Safe Value Retrieval**: Generic `GetValue<T>()` method with automatic type conversion
- **Value Resolvers**: Extensible resolver pattern for custom type handling and transformation
- **Collection Interfaces**: Full implementation of `IDictionary<string, object>` and related interfaces
- **Deep Cloning**: Support for shallow and deep copying of dictionaries
- **Dictionary Merging**: Merge multiple dictionaries with flexible control options

## Supported Frameworks

- .NET Standard 2.1
- .NET 5.0 and later
- .NET Framework 4.7.2 and later

## Core Concepts

### IValueResolver

The `IValueResolver` interface allows you to implement custom type handling and transformation:

```csharp
public interface IValueResolver
{
    bool CanResolve(object value);
    object Resolve(object value);
}
```

Register resolvers globally:

```csharp
DynamicDictionary.RegisterValueResolver(customResolver);
```

Resolvers are checked in the order they are registered, with the most recently registered resolver checked first.

## Quick Usage

### Creating a Dictionary

```csharp
using OneCiel.Core.Dynamics;

// Empty dictionary
var dict = new DynamicDictionary();

// From key-value pairs
var dict2 = new DynamicDictionary(new[] 
{
    new KeyValuePair<string, object>("name", "John"),
    new KeyValuePair<string, object>("age", 30)
});

// From existing dictionary
var source = new Dictionary<string, object> { { "key", "value" } };
var dict3 = new DynamicDictionary(source);
```

### Adding and Retrieving Values

```csharp
dict["name"] = "Alice";
dict["age"] = 25;

var name = dict["name"]; // "Alice"
var age = dict["age"]; // 25
```

### Dynamic Member Access

```csharp
dynamic person = new DynamicDictionary();
person.firstName = "John";
person.lastName = "Doe";

string fullName = person.firstName + " " + person.lastName;
```

### Nested Property Access

```csharp
var user = new DynamicDictionary
{
    { "profile", new DynamicDictionary 
    {
        { "address", new DynamicDictionary 
        {
            { "city", "New York" },
            { "country", "USA" }
        }}
    }}
};

// Access nested properties
string city = user["profile.address.city"] as string; // "New York"

// Set nested values (creates intermediate objects)
user["profile.contact.email"] = "user@example.com";
```

### Array Element Access

```csharp
var data = new DynamicDictionary
{
    { "items", new List<object> { "apple", "banana", "cherry" } },
    { "users", new object[]
    {
        new DynamicDictionary { { "id", 1 }, { "name", "Alice" } },
        new DynamicDictionary { { "id", 2 }, { "name", "Bob" } }
    }}
};

var firstItem = data["items[0]"]; // "apple"
var secondUserName = data["users[1].name"]; // "Bob"
```

### Type-Safe Value Retrieval

```csharp
var dict = new DynamicDictionary
{
    { "count", "123" },
    { "active", true }
};

// Automatic type conversion with safe fallback
int count = dict.GetValue<int>("count", 0); // 123
bool active = dict.GetValue<bool>("active"); // true
decimal price = dict.GetValue<decimal>("price", 0m); // 0m (not found)
```

### Cloning and Merging

```csharp
// Shallow copy
var shallowCopy = dict.Clone(deepCopy: false);

// Deep copy
var deepCopy = dict.Clone(deepCopy: true);

// Merge dictionaries
var dict1 = new DynamicDictionary { { "a", 1 }, { "b", 2 } };
var dict2 = new DynamicDictionary { { "b", 3 }, { "c", 4 } };
dict1.Merge(dict2, overwriteExisting: true); // dict1: a=1, b=3, c=4
```

## Custom Value Resolvers

Implement `IValueResolver` to handle custom types:

```csharp
public class CustomTypeResolver : IValueResolver
{
    public bool CanResolve(object value)
    {
        return value is MyCustomType;
    }

    public object Resolve(object value)
    {
        var custom = (MyCustomType)value;
        // Transform the value as needed
        return custom.Transform();
    }
}

// Register the resolver
DynamicDictionary.RegisterValueResolver(new CustomTypeResolver());

// Now all DynamicDictionary instances will automatically resolve MyCustomType values
```

## API Reference

### Static Methods

| Method | Description |
|--------|-------------|
| `RegisterValueResolver(IValueResolver)` | Register a global value resolver |
| `UnregisterValueResolver(IValueResolver)` | Unregister a resolver |
| `ClearValueResolvers()` | Remove all registered resolvers |
| `GetRegisteredResolvers()` | Get read-only list of resolvers |

### Key Methods

| Method | Description |
|--------|-------------|
| `Get(string key)` | Gets value by key or path |
| `GetValue<T>(string key, T defaultValue)` | Gets typed value with default |
| `Add(string key, object value)` | Adds or overwrites a key-value pair |
| `Remove(string key)` | Removes a key |
| `RemoveWhere(Func predicate)` | Removes items matching condition |
| `Clear()` | Removes all items |
| `Clone(bool deepCopy)` | Creates a copy |
| `Merge(DynamicDictionary src, ...)` | Merges another dictionary |
| `ContainsKey(string key)` | Checks key existence |

## License

MIT License
