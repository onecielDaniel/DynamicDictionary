# OneCiel.System.Dynamics

[![NuGet](https://img.shields.io/nuget/v/OneCiel.System.Dynamics.svg)](https://www.nuget.org/packages/OneCiel.System.Dynamics)
[![NuGet](https://img.shields.io/nuget/v/OneCiel.System.Dynamics.JsonExtension.svg)](https://www.nuget.org/packages/OneCiel.System.Dynamics.JsonExtension)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A flexible and powerful dynamic dictionary implementation for .NET that provides safe, resilient handling of REST API JSON data. Perfect for scenarios where API responses are unpredictable - no crashes from missing fields, extra fields, or API changes. Supports both dynamic access and strongly-typed models with selective property definition.

## 🚀 Features

- **🛡️ REST API Resilience**: Handle unpredictable JSON data safely - no crashes from missing fields, extra fields, or API changes. Perfect for REST API integration.
- **⭐ Natural Object-Like Access**: Use dot notation (`user.name`, `user.age = 31`) - access properties just like regular objects! No casting or indexers needed.
- **🎯 Selective Strong Typing**: Define only the properties you need by inheriting from `DynamicDictionary`. Server can add more fields without breaking your code.
- **Dynamic Object Access**: Full support for dynamic member access with IntelliSense-friendly syntax
- **Nested Path Navigation**: Support for dot notation (`user.name`) and array indexing (`items[0]`, `users[1].address.city`)
- **Case-Insensitive Keys**: All key lookups are case-insensitive for convenience
- **Type-Safe Conversion**: Generic `GetValue<T>()` method with automatic type conversion
- **Extensible Value Resolution**: Register custom value resolvers for type transformation and conversion
- **Collection Interfaces**: Implements `IDictionary<string, object>`, `IEnumerable<KeyValuePair<string, object>>`, and more
- **Deep Copy Support**: Clone dictionaries with optional deep copying
- **Merge Operations**: Merge multiple dictionaries with control over overwrite behavior
- **JSON Support**: Seamless JSON serialization/deserialization with System.Text.Json integration
- **Interface-Based Architecture**: Clean, testable, and extensible design with dependency injection support

## 📦 Packages

### OneCiel.System.Dynamics
The core library targeting .NET Standard 2.1, providing the base `DynamicDictionary` class with value resolver support.

```bash
dotnet add package OneCiel.System.Dynamics
```

**Target Framework**: .NET Standard 2.1 (compatible with .NET 5.0+, .NET Framework 4.7.2+)

**Features**:
- Dynamic dictionary with case-insensitive keys
- Nested property and array navigation
- Extensible `IValueResolver` interface
- Type-safe value retrieval with conversion
- Deep cloning and merging
- Factory methods for creating instances
- Standardized JSON serialization interfaces

### OneCiel.System.Dynamics.JsonExtension
Extension package for .NET 8/9 providing seamless JSON serialization and deserialization using `System.Text.Json` with automatic JsonElement conversion.

```bash
dotnet add package OneCiel.System.Dynamics.JsonExtension
```

**Target Frameworks**: .NET 8.0, .NET 9.0

**Features**:
- JSON string serialization/deserialization
- Async and sync file operations
- `SystemTextJsonDynamicsSerializer` implementation
- System.Text.Json integration with custom converter
- Pretty-printing and flexible formatting
- Automatic default serializer initialization

## 🛡️ Why OneCiel.System.Dynamics?

### Problem: REST API JSON Data is Unpredictable

When working with REST APIs, you often face these challenges:

❌ **Strict Typing Issues**: Server adds a new field → Your app crashes  
❌ **Deserialization Failures**: Missing or unexpected fields break your code  
❌ **API Evolution**: Backend changes break your frontend  
❌ **Over-Engineering**: Define all fields even when you only need a few  

### Solution: Flexible & Resilient JSON Handling

✅ **No Crashes**: Missing or extra fields won't break your application  
✅ **API Evolution Safe**: Server can add/remove fields without breaking your code  
✅ **Selective Properties**: Define only what you actually use  
✅ **Type Safety When Needed**: Use strongly-typed models when you want explicit control  

### Example: REST API Response Handling

```csharp
using OneCiel.System.Dynamics;
using System.Net.Http;
using System.Text.Json;

// REST API returns JSON - structure may change!
string jsonResponse = await httpClient.GetStringAsync("https://api.example.com/users/1");
// Response: { "id": 1, "name": "John", "email": "john@example.com", "newField": "value" }

// ❌ Traditional approach - CRASHES if structure changes
// var user = JsonSerializer.Deserialize<User>(jsonResponse); // Throws exception!

// ✅ DynamicDictionary approach - NEVER crashes!
dynamic user = DynamicDictionary.Create(jsonResponse);

// Access only what you need - safe and flexible
string name = user.name;        // "John"
string email = user.email;       // "john@example.com"

// New fields from server? No problem - they're just ignored
// user.newField exists but doesn't break anything

// Missing fields? No problem - returns null safely
string phone = user.phone;      // null (doesn't crash!)
```

### Strongly-Typed Models (When You Need Explicit Control)

Define only the properties you actually use - server can add more fields without breaking your code:

```csharp
// Define only what you need - server can add more fields safely!
public class ApiUser : DynamicDictionary
{
    // Explicitly define only the properties you use
    public int Id => GetValue<int>("id");
    public string Name => GetValue<string>("name");
    public string Email => GetValue<string>("email");
    
    // Server added "phone", "address", "preferences"? 
    // No problem - they're ignored unless you add them here
}

// Use with REST API
string jsonResponse = await httpClient.GetStringAsync("https://api.example.com/users/1");
var user = DynamicDictionary.Create<ApiUser>(jsonResponse);

// Strongly-typed access for defined properties
Console.WriteLine(user.Name);   // "John"
Console.WriteLine(user.Email);  // "john@example.com"

// Server added new fields? Your code still works!
// The new fields are stored but don't break your application
```

### Real-World Scenario: API Evolution

```csharp
// Month 1: API returns { "id": 1, "name": "John" }
dynamic user = DynamicDictionary.Create(apiResponse);
string name = user.name;  // Works!

// Month 2: API adds "email" field → { "id": 1, "name": "John", "email": "john@example.com" }
// Your code still works! No changes needed.
string name = user.name;   // Still works!
string email = user.email; // Now available!

// Month 3: API removes "name", adds "fullName" → { "id": 1, "fullName": "John Doe", "email": "john@example.com" }
// Your code handles it gracefully
string name = user.name;        // null (doesn't crash!)
string fullName = user.fullName; // "John Doe" (new field accessible)
```

## 🎯 Quick Start

### ⭐ Dynamic Member Access (Recommended)

The most intuitive way to use DynamicDictionary - access properties just like regular objects!

```csharp
using OneCiel.System.Dynamics;

// Create a user object with dynamic access
dynamic user = new DynamicDictionary();

// Set values using dot notation - clean and intuitive!
user.name = "John Doe";
user.age = 30;
user.email = "john@example.com";

// Access values directly - no casting needed!
string name = user.name;        // "John Doe"
int age = user.age;             // 30
string email = user.email;      // "john@example.com"

// Update values easily
user.age = 31;                  // Just like a regular property!

// Works with any property name
user.isActive = true;
user.createdAt = DateTime.Now;
```

### Indexer Access (Alternative)

You can also use indexer syntax if you prefer:

```csharp
var person = new DynamicDictionary();

// Add values using indexer
person["name"] = "John Doe";
person["age"] = 30;
person["email"] = "john@example.com";

// Access values dynamically
string name = person["name"] as string;
int age = (int)person["age"];

// Use GetValue<T>() for type-safe access
var email = person.GetValue<string>("email");
var invalid = person.GetValue<int>("email", -1); // Returns -1 (default value)
```

### Nested Properties

```csharp
// Create nested structure with dynamic access
dynamic user = new DynamicDictionary();
user.address = new DynamicDictionary { { "city", "New York" }, { "zip", "10001" } };

// Access nested properties using dot notation
string city = user.address.city;        // "New York"
string zip = user.address.zip;          // "10001"

// Or use path notation with indexer
var city2 = user["address.city"];       // "New York"
var zip2 = user.GetValue<string>("address.zip"); // "10001"

// Set nested values (creates intermediate objects automatically)
user.address.country = "USA";
user.contact.phone = "555-1234";        // Creates intermediate objects automatically

// Or using path notation
user["address.country"] = "USA";
user["contact.phone"] = "555-1234";
```

### Array Access

```csharp
dynamic data = new DynamicDictionary();
data.items = new List<object> { "apple", "banana", "cherry" };
data.users = new object[]
{
    new DynamicDictionary { { "id", 1 }, { "name", "Alice" } },
    new DynamicDictionary { { "id", 2 }, { "name", "Bob" } }
};

// Access array elements using path notation
var firstItem = data["items[0]"];              // "apple"
var firstUserName = data["users[0].name"];      // "Alice"
var secondUserId = data.GetValue<int>("users[1].id"); // 2

// Or access arrays directly
var items = data.items;                         // List<object>
var users = data.users;                         // object[]
```


### JSON Support (with JsonExtension package)

```csharp
using OneCiel.System.Dynamics;

// Create and use with dynamic access
dynamic person = new DynamicDictionary();
person.name = "John";
person.age = 30;

// Convert to JSON string
string json = person.Serialize();

// Parse from JSON string using factory method
string jsonData = @"
{
    ""user"": {
        ""id"": 1,
        ""name"": ""Alice""
    },
    ""items"": [1, 2, 3]
}";

// Modern factory method - returns dynamic for easy access!
dynamic parsed = DynamicDictionary.Create(jsonData);

// Access with dot notation - clean and intuitive!
int userId = parsed.user.id;              // 1
string userName = parsed.user.name;       // "Alice"
int secondItem = parsed.items[1];         // 2

// Or use path notation
var userId2 = parsed.GetValue<int>("user.id"); // 1
var secondItem2 = parsed["items[1]"];          // 2

// Parse JSON arrays
var usersJson = @"[{""id"": 1, ""name"": ""Alice""}, {""id"": 2, ""name"": ""Bob""}]";
dynamic[] users = DynamicDictionary.CreateArray(usersJson);

// Access array elements with dynamic access
string firstName = users[0].name;          // "Alice"
int secondId = users[1].id;                // 2
```

### Strongly-Typed Models (Selective Property Definition)

Define only the properties you need - server can add more fields without breaking your code!

```csharp
// REST API returns: { "id": 1, "name": "John", "email": "john@example.com", "phone": "123-456", "address": {...} }
// But you only need id, name, and email - define only those!

public class ApiUser : DynamicDictionary
{
    // Explicitly define only what you use
    public int Id => GetValue<int>("id");
    public string Name => GetValue<string>("name");
    public string Email => GetValue<string>("email");
    
    // Server added "phone" and "address"? No problem - they're safely ignored
    // Your code won't break, and you can access them dynamically if needed later
}

// Use with REST API response
string jsonResponse = await httpClient.GetStringAsync("https://api.example.com/users/1");
var user = DynamicDictionary.Create<ApiUser>(jsonResponse);

// Strongly-typed access for defined properties
Console.WriteLine(user.Name);   // "John" - type-safe!
Console.WriteLine(user.Email);  // "john@example.com" - type-safe!

// Server added new fields? Your code still works!
// Access additional fields dynamically if needed
dynamic dynamicUser = user;
string phone = dynamicUser.phone;  // "123-456" (if server added it)
```

**Benefits**:
- ✅ **API Evolution Safe**: Server can add/remove fields without breaking your code
- ✅ **Selective Properties**: Define only what you actually use
- ✅ **Type Safety**: Get compile-time safety for properties you care about
- ✅ **Flexibility**: Still access unexpected fields dynamically when needed

### File Operations

```csharp
// Create a configuration object
dynamic config = new DynamicDictionary();
config.appName = "MyApp";
config.version = "1.0.0";
config.debug = true;

// Asynchronous save
await config.ToJsonFileAsync("config.json");

// Asynchronous load
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var loadedConfig = await DynamicDictionaryJsonExtensions.FromJsonFileAsync("config.json", serializer);

// Synchronous operations
config.ToJsonFile("config-sync.json");
var syncLoadedConfig = DynamicDictionaryJsonExtensions.FromJsonFile("config-sync.json", serializer);
```

## 🏗️ Architecture

### Core Components

#### DynamicDictionary
The main class that provides dynamic dictionary functionality. It inherits from `DynamicObject` and implements multiple collection interfaces.

**Key Features**:
- Case-insensitive key lookup
- Nested path navigation
- Type-safe value retrieval
- Value resolver support
- Factory methods for creation

#### IValueResolver
Interface for custom type handling and transformation:

```csharp
public interface IValueResolver
{
    bool CanResolve(object value);
    object Resolve(object value);
}
```

**Usage**:
```csharp
// Register a custom resolver
DynamicDictionary.RegisterValueResolver(new MyCustomResolver());

// Resolvers are automatically applied when accessing values
var product = new DynamicDictionary { { "price", "99.99" } };
var price = product["price"]; // Custom resolver transforms the value
```

#### IDynamicsJsonSerializer
Unified interface for JSON serialization and deserialization:

```csharp
public interface IDynamicsJsonSerializer
{
    string Serialize(object obj);
    DynamicDictionary Deserialize(string json);
    DynamicDictionary[] DeserializeArray(string json);
}
```

**Implementation**: `SystemTextJsonDynamicsSerializer` (in JsonExtension package)

### Package Structure

```
OneCiel.System.Dynamics (Core)
├── DynamicDictionary.cs              # Main class
├── IValueResolver.cs                  # Value resolver interface
├── JsonSerializationInterfaces.cs     # JSON serialization interfaces
└── README.md

OneCiel.System.Dynamics.JsonExtension
├── SystemTextJsonImplementations.cs   # System.Text.Json implementations
├── DynamicDictionaryJsonExtensions.cs # Extension methods
├── DynamicDictionaryJsonConverter.cs  # JsonConverter for System.Text.Json
└── README.md
```

## 📚 Advanced Usage

### Custom Value Resolvers

```csharp
public class DateTimeResolver : IValueResolver
{
    public bool CanResolve(object value)
    {
        return value is string str && DateTime.TryParse(str, out _);
    }

    public object Resolve(object value)
    {
        if (DateTime.TryParse(value as string, out var dateTime))
        {
            return dateTime;
        }
        return value;
    }
}

// Register the resolver
DynamicDictionary.RegisterValueResolver(new DateTimeResolver());

// Now string dates are automatically converted
var order = new DynamicDictionary { { "created", "2024-01-01" } };
DateTime created = order.GetValue<DateTime>("created"); // Automatically converted
```

### Custom JSON Serializer

```csharp
// Create a custom serializer with specific options
var serializeOptions = new JsonSerializerOptions
{
    WriteIndented = false,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

var deserializeOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true
};

var serializer = new SystemTextJsonDynamicsSerializer(serializeOptions, deserializeOptions);

// Use with factory methods
var product = DynamicDictionary.Create(json, serializer);
string jsonOutput = product.Serialize(serializer);
```

### Dictionary Merging

```csharp
var defaultSettings = new DynamicDictionary { { "theme", "dark" }, { "language", "en" } };
var userSettings = new DynamicDictionary { { "language", "ko" }, { "notifications", true } };

// Merge with overwrite
defaultSettings.Merge(userSettings, overwriteExisting: true);
// Result: theme="dark", language="ko", notifications=true

// Merge without overwrite
defaultSettings.Merge(userSettings, overwriteExisting: false);
// Result: theme="dark", language="en", notifications=true (language not overwritten)

// Deep merge
defaultSettings.Merge(userSettings, overwriteExisting: true, deepMerge: true);
```

### Cloning

```csharp
var customer = new DynamicDictionary
{
    { "name", "John" },
    { "address", new DynamicDictionary { { "city", "NYC" } } }
};

// Shallow copy (references shared)
var shallowCopy = customer.Clone(deepCopy: false);

// Deep copy (independent objects)
var deepCopy = customer.Clone(deepCopy: true);
```

## 🔧 Building and Publishing

### Prerequisites

- .NET SDK 8.0 or later
- PowerShell (for Windows) or Bash (for Linux/macOS)

### Build Scripts

We provide automated build and publish scripts:

**Windows (PowerShell)**:
```powershell
.\build-and-publish.ps1
```

**Linux/macOS (Bash)**:
```bash
./build-and-publish.sh
```

**Options**:
- `--SkipBuild`: Skip the build step
- `--SkipPublish`: Skip the publish step (only create packages)
- Custom NuGet API key can be provided as parameter

### Manual Build

```bash
# Clean
dotnet clean --configuration Release

# Restore
dotnet restore

# Build
dotnet build --configuration Release

# Pack
dotnet pack OneCiel.System.Dynamics\OneCiel.System.Dynamics.csproj --configuration Release --output packages
dotnet pack OneCiel.System.Dynamics.JsonExtension\OneCiel.System.Dynamics.JsonExtension.csproj --configuration Release --output packages

# Publish (requires NuGet API key)
dotnet nuget push packages\OneCiel.System.Dynamics.*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### NuGet Configuration

The project includes a `.nuget.config` file for NuGet package source configuration. The API key is configured in the build scripts.

## 📖 Documentation

- **[Core Library README](OneCiel.System.Dynamics/README.md)**: Detailed guide for the core library
- **[JSON Extension README](OneCiel.System.Dynamics.JsonExtension/README.md)**: JSON features and usage
- **[Project Structure](PROJECT_STRUCTURE.md)**: Detailed project organization
- **[CHANGELOG](CHANGELOG.md)**: Version history and changes
- **[CONTRIBUTING](CONTRIBUTING.md)**: Guidelines for contributors

## 🎓 Examples

See the `Examples` directory for comprehensive usage examples:

- Basic dynamic dictionary operations
- JSON serialization/deserialization
- REST API integration
- Strongly-typed model examples
- Custom resolver implementations

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guide](CONTRIBUTING.md) for details on:

- Code of conduct
- Development setup
- Code style guidelines
- Pull request process

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with .NET and System.Text.Json
- Inspired by the need for flexible, dynamic data structures in .NET applications

## 📞 Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Check the documentation in the `README.md` files
- Review the examples in the `Examples` directory

---

**Made with ❤️ by OneCiel**
