# OneCiel.System.Dynamics - Project Structure Summary

## 📁 Directory Structure

```
E:\OneCiel/
├── OneCiel.System.Dynamics.sln                    # Solution file
│
├── OneCiel.System.Dynamics/                       # Core Library Project
│   ├── OneCiel.System.Dynamics.csproj            # Project file (.NET Standard 2.1)
│   ├── DynamicDictionary.cs                      # Main class with resolver support
│   └── README.md                                 # Package documentation
│
├── OneCiel.System.Dynamics.JsonExtension/        # JSON Extension Project
│   ├── OneCiel.System.Dynamics.JsonExtension.csproj
│   ├── DynamicDictionaryJsonExtensions.cs        # Extension methods for JSON
│   ├── DynamicDictionaryJsonConverter.cs         # JsonConverter implementation
│   ├── JsonElementValueResolver.cs               # Resolver for JsonElement conversion
│   └── README.md                                 # Package documentation
│
├── README.md                                     # Main documentation
├── CHANGELOG.md                                  # Version history
├── CONTRIBUTING.md                               # Contribution guidelines
├── PACKAGING.md                                  # Build & publish guide
├── PROJECT_STRUCTURE.md                          # This file
├── LICENSE                                       # MIT License
└── .gitignore                                    # Git ignore rules
```

## 📦 Package Structure

### OneCiel.System.Dynamics (v1.0.0)
**Target Framework**: .NET Standard 2.1

**Key Features**:
- Dynamic dictionary inheriting from DynamicObject
- Case-insensitive key lookup with StringComparer
- Nested property navigation with dot notation
- Array element access with bracket notation
- Extensible value resolver pattern via `IValueResolver` interface
- Type-safe `GetValue<T>()` with automatic conversion
- Dictionary cloning (shallow/deep copy)
- Dictionary merging with flexible options
- Conditional item removal with `RemoveWhere()`

**Key Classes**:
- `IValueResolver` - Interface for custom type resolution
- `DynamicDictionary` - Main dictionary class with resolver support

**Resolver Management Methods**:
- `RegisterValueResolver(IValueResolver)` - Register a global resolver
- `UnregisterValueResolver(IValueResolver)` - Unregister a resolver
- `ClearValueResolvers()` - Clear all resolvers
- `GetRegisteredResolvers()` - Get current resolvers

### OneCiel.System.Dynamics.JsonExtension (v1.0.0)
**Target Frameworks**: .NET 8.0, .NET 9.0

**Key Features**:
- JSON string serialization/deserialization
- Asynchronous and synchronous file operations
- `JsonElementValueResolver` for automatic JsonElement conversion
- System.Text.Json integration with custom converter
- Automatic initialization of resolvers
- Support for nested objects and arrays
- Pretty-printing with indentation control
- Comment handling and trailing comma support

**Key Classes**:
- `IValueResolver` (inherited from core)
- `JsonElementValueResolver` - Resolves JsonElement to .NET types
- `DynamicDictionaryJsonExtensions` - Extension methods with auto-init
- `DynamicDictionaryJsonConverter` - JsonConverter<DynamicDictionary>

## 🎯 Architecture

### Resolver Pattern

```csharp
// Core Library Interface
public interface IValueResolver
{
    bool CanResolve(object value);
    object Resolve(object value);
}

// Usage in DynamicDictionary
private static readonly List<IValueResolver> _resolvers = new();

private static object ApplyResolvers(object value)
{
    foreach (var resolver in _resolvers)
    {
        if (resolver.CanResolve(value))
            return resolver.Resolve(value);
    }
    return value;
}
```

### JSON Element Resolution

```csharp
// JsonExtension Resolver
public class JsonElementValueResolver : IValueResolver
{
    public bool CanResolve(object value) => value is JsonElement;
    
    public object Resolve(object value)
    {
        var element = (JsonElement)value;
        return element.ValueKind switch
        {
            JsonValueKind.Object => new DynamicDictionary(...),
            JsonValueKind.Array => ConvertJsonArray(element),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => ConvertJsonNumber(element),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
}
```

## 🚀 Building & Publishing

### Build Release Configuration

```bash
cd E:\OneCiel

# Clean previous builds
dotnet clean --configuration Release

# Build the solution
dotnet build --configuration Release

# Create NuGet packages
dotnet pack OneCiel.System.Dynamics\OneCiel.System.Dynamics.csproj --configuration Release
dotnet pack OneCiel.System.Dynamics.JsonExtension\OneCiel.System.Dynamics.JsonExtension.csproj --configuration Release
```

### Package Output

- `OneCiel.System.Dynamics\bin\Release\OneCiel.System.Dynamics.1.0.0.nupkg`
- `OneCiel.System.Dynamics.JsonExtension\bin\Release\OneCiel.System.Dynamics.JsonExtension.1.0.0.nupkg`

### Publishing to NuGet

See PACKAGING.md for detailed instructions.

## ✨ Key Features Implemented

### Core Library
- ✅ Dynamic object access with DynamicObject
- ✅ Nested property navigation (dot notation)
- ✅ Array element access (bracket notation)
- ✅ Case-insensitive key lookup
- ✅ Type-safe value retrieval with conversion
- ✅ **Extensible Resolver Pattern**
- ✅ Resolver registration/unregistration
- ✅ Automatic resolver application in member access
- ✅ Deep cloning and merging
- ✅ All collection interfaces implemented

### JSON Extension
- ✅ JSON string serialization/deserialization
- ✅ Async and sync file I/O
- ✅ **JsonElementValueResolver** implementation
- ✅ Automatic resolver initialization
- ✅ System.Text.Json converter
- ✅ Nested object and array support
- ✅ Pretty-printing options
- ✅ Comment and trailing comma handling

## 📚 Documentation

- **README.md** (Root) - Overview and resolver pattern explanation
- **OneCiel.System.Dynamics/README.md** - Core library with resolver examples
- **OneCiel.System.Dynamics.JsonExtension/README.md** - JSON extension with resolver details
- **PROJECT_STRUCTURE.md** - This document (architecture and structure)
- **CHANGELOG.md** - Version history
- **CONTRIBUTING.md** - Developer guidelines
- **PACKAGING.md** - Build and publishing guide

## 🔧 Extension Points

### Creating Custom Resolvers

```csharp
// Example: Convert custom types
public class MyTypeResolver : IValueResolver
{
    public bool CanResolve(object value) => value is MyCustomType;
    
    public object Resolve(object value)
    {
        var custom = (MyCustomType)value;
        return custom.Transform();
    }
}

// Register globally
DynamicDictionary.RegisterValueResolver(new MyTypeResolver());
```

### Resolver Priority

Resolvers are checked in reverse registration order:
1. Last registered → checked first
2. First registered → checked last
3. No match → original value returned

## 📋 Type Conversion Support

### Core Library
- Numeric types (byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal)
- Enum types (from string or numeric values)
- Nullable types
- Generic types via Convert.ChangeType

### JSON Extension
Additional conversions via JsonElementValueResolver:
- JsonElement.Object → DynamicDictionary
- JsonElement.Array → object[] or DynamicDictionary[]
- JsonElement.String → string
- JsonElement.Number → decimal/long/int/double
- JsonElement.True/False → bool
- JsonElement.Null → null

## 🎓 Usage Example with Resolvers

```csharp
using OneCiel.System.Dynamics;
using System.Text.Json;

// 1. Load JSON (automatically registers JsonElementValueResolver)
var data = DynamicDictionaryJsonExtensions.FromJson(@"
{
    ""user"": { ""id"": 1, ""name"": ""Alice"" },
    ""items"": [""a"", ""b"", ""c""]
}");

// 2. Access with automatic type conversion via resolver
int userId = data.GetValue<int>("user.id"); // JsonElement converted to int
string userName = data["user.name"]; // JsonElement converted to string
var firstItem = data["items[0]"]; // JsonElement array converted to object[]

// 3. Register custom resolver for special types
DynamicDictionary.RegisterValueResolver(new MyCustomResolver());

// 4. Use with dynamic access
dynamic d = data;
var resolved = d.user.name; // Goes through resolver chain
```

## 🌐 Compatibility

| Component | .NET Std | .NET 5+ | .NET 6+ | .NET 7+ | .NET 8 | .NET 9 | .NET FW |
|-----------|:--------:|:-------:|:-------:|:-------:|:------:|:------:|:-------:|
| Core | 2.1 ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | 4.7.2+ |
| JSON Ext | - | - | - | - | ✓ | ✓ | - |

## 🔒 Thread Safety

- **Resolver Registration**: Thread-safe (uses locks in initialization)
- **Dictionary Instance**: Not thread-safe (use locks if accessed from multiple threads)
- **Global Resolver List**: Safe for concurrent read/write

## 📈 Performance Notes

- Path parsing is cached where possible
- Resolver checks are sequential (optimize registration order)
- Type conversion uses Convert.ChangeType for standard types
- JSON resolution happens automatically on value access

---

**Version**: 1.0.0
**Status**: ✅ Complete with Resolver Pattern Implementation
**Last Updated**: January 2024
