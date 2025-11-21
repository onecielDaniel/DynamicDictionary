# Strongly-Typed DynamicDictionary Models

## Overview

This guide demonstrates how to create strongly-typed models that inherit from `DynamicDictionary`, providing the best of both worlds:
- **Type Safety**: IntelliSense support and compile-time checking for known properties
- **Flexibility**: Dynamic access for nested objects and unknown properties
- **Dictionary Access**: Direct key-value pair manipulation when needed

## Key Features

### 1. Generic Create Methods with Type Constraints

The `DynamicDictionary` class provides generic factory methods that enforce type safety:

```csharp
public static T Create<T>(string json, IDynamicsJsonSerializer serializer) 
    where T : DynamicDictionary, new()

public static T[] CreateArray<T>(string json, IDynamicsJsonSerializer serializer) 
    where T : DynamicDictionary, new()
```

**Type Constraint**: `where T : DynamicDictionary, new()`
- ✅ T **must** inherit from `DynamicDictionary`
- ✅ T **must** have a parameterless constructor
- ❌ Regular classes, `Dictionary<string, object>`, or other types **cannot** be used

## Creating Strongly-Typed Models

### Basic Model Structure

```csharp
public class JsonPlaceholderUser : DynamicDictionary
{
    // Strongly-typed properties with safe type conversion
    public int Id => GetValue<int>("id");
    public string Name => GetValue<string>("name");
    public string Email => GetValue<string>("email");
    
    // Dynamic access for nested objects
    public dynamic Address => this["address"];
    public dynamic Company => this["company"];
    
    // Convenience methods
    public string GetCity() => Address?.city ?? "Unknown";
    
    // Custom ToString for better debugging
    public override string ToString()
    {
        return $"User #{Id}: {Name} ({Email})";
    }
}
```

### Key Principles

1. **Use `GetValue<T>(key)`** for strongly-typed properties
   - Provides type safety and default value handling
   - Returns `default(T)` if key doesn't exist

2. **Use indexer** for dynamic nested objects
   - `public dynamic Address => this["address"];`
   - Allows flexible access: `user.Address.city`

3. **Override `ToString()`** for better debugging
   - Provides meaningful output in logs and debugger

## Usage Examples

### Example 1: Single Object

```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var json = await httpClient.GetStringAsync("https://api.example.com/users/1");

// Generic Create<T> method - strongly typed!
var user = DynamicDictionary.Create<JsonPlaceholderUser>(json, serializer);

// IntelliSense support for properties
Console.WriteLine(user.Name);   // Type: string
Console.WriteLine(user.Id);     // Type: int
Console.WriteLine(user.Email);  // Type: string

// Dynamic access for nested objects
Console.WriteLine(user.Address.city);
Console.WriteLine(user.Company.name);
```

### Example 2: Array of Objects

```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var json = await httpClient.GetStringAsync("https://api.example.com/posts");

// Generic CreateArray<T> method - returns T[] (not dynamic!)
var posts = DynamicDictionary.CreateArray<JsonPlaceholderPost>(json, serializer);

// Type-safe iteration with IntelliSense
foreach (var post in posts)
{
    Console.WriteLine($"Post #{post.Id}: {post.Title}");
    Console.WriteLine($"By User {post.UserId}");
}
```

### Example 3: Hybrid Access Pattern

```csharp
var user = DynamicDictionary.Create<JsonPlaceholderUser>(json, serializer);

// 1. Strongly-typed property access (recommended for known properties)
Console.WriteLine(user.Name);
Console.WriteLine(user.Email);

// 2. Dynamic access (for nested/unknown properties)
Console.WriteLine(user.Address.geo.lat);
Console.WriteLine(user.Company.catchPhrase);

// 3. Dictionary access (direct key lookup)
Console.WriteLine(user["username"]);
Console.WriteLine(user["phone"]);

// 4. Type-safe method access (explicit type conversion)
var id = user.GetValue<int>("id");
var website = user.GetValue<string>("website");
```

## Type Constraint Benefits

### ✅ Compile-Time Safety

```csharp
// ✅ WORKS - JsonPlaceholderUser inherits from DynamicDictionary
var user = DynamicDictionary.Create<JsonPlaceholderUser>(json, serializer);

// ✅ WORKS - JsonPlaceholderPost inherits from DynamicDictionary
var post = DynamicDictionary.Create<JsonPlaceholderPost>(json, serializer);

// ❌ COMPILE ERROR - string does not inherit from DynamicDictionary
var invalid = DynamicDictionary.Create<string>(json, serializer);

// ❌ COMPILE ERROR - Dictionary<string, object> does not inherit from DynamicDictionary
var invalid2 = DynamicDictionary.Create<Dictionary<string, object>>(json, serializer);

// ❌ COMPILE ERROR - Custom POCO class does not inherit from DynamicDictionary
public class MyClass { public int Id { get; set; } }
var invalid3 = DynamicDictionary.Create<MyClass>(json, serializer);
```

### Why This Matters

1. **Prevents Runtime Errors**: Invalid types are caught at compile time
2. **Clear Intent**: Code clearly shows it's working with DynamicDictionary
3. **IntelliSense Support**: Full IDE support for derived types
4. **Type Safety**: Ensures consistent behavior across all derived models

## Available Models

The Examples project includes ready-to-use models for JSONPlaceholder API:

| Model | API Endpoint | Key Properties |
|-------|-------------|----------------|
| `JsonPlaceholderUser` | `/users` | Id, Name, Username, Email, Phone, Website, Address, Company |
| `JsonPlaceholderPost` | `/posts` | Id, UserId, Title, Body |
| `JsonPlaceholderComment` | `/comments` | Id, PostId, Name, Email, Body |
| `JsonPlaceholderAlbum` | `/albums` | Id, UserId, Title |
| `JsonPlaceholderPhoto` | `/photos` | Id, AlbumId, Title, Url, ThumbnailUrl |
| `JsonPlaceholderTodo` | `/todos` | Id, UserId, Title, Completed |

## Debugging Tips

### Watch Window Expressions

When debugging strongly-typed models, use these watch expressions:

```
// For strongly-typed properties (works in watch)
user.Name
user.Id
user.Email

// For dynamic nested objects (use _data)
user._data["address"]
((DynamicDictionary)user.Address)._data["city"]

// For explicit type checking
user.GetValue<string>("name")
user["email"]
```

### Breakpoint Inspection

```csharp
var user = DynamicDictionary.Create<JsonPlaceholderUser>(json, serializer);
// ⬅️ Set breakpoint here

// In watch window:
// ✅ user.Name          - works (strongly-typed property)
// ✅ user._data["name"] - works (direct dictionary access)
// ⚠️ user.Address.city  - may not show in watch (dynamic)
// ✅ ((DynamicDictionary)user.Address)._data["city"] - works
```

## Best Practices

### 1. Choose the Right Access Pattern

```csharp
public class JsonPlaceholderUser : DynamicDictionary
{
    // ✅ GOOD: Strongly-typed for known, flat properties
    public int Id => GetValue<int>("id");
    public string Name => GetValue<string>("name");
    
    // ✅ GOOD: Dynamic for nested objects (structure may vary)
    public dynamic Address => this["address"];
    
    // ✅ GOOD: Convenience methods for common nested access
    public string GetCity() => Address?.city ?? "Unknown";
    
    // ❌ AVOID: Deeply nested strongly-typed chains
    // public string City => GetValue<DynamicDictionary>("address")?.GetValue<string>("city");
    // Better to use: Address?.city
}
```

### 2. Provide Default Values

```csharp
public class SafeModel : DynamicDictionary
{
    // With default value
    public string Name => GetValue<string>("name") ?? "Unknown";
    public int Age => GetValue<int>("age"); // Returns 0 if missing
    
    // Nullable for explicit missing value handling
    public int? OptionalAge => ContainsKey("age") ? GetValue<int>("age") : (int?)null;
}
```

### 3. Override ToString() for Better Debugging

```csharp
public override string ToString()
{
    return $"User #{Id}: {Name} ({Email})";
}
```

### 4. Add Validation Methods

```csharp
public class ValidatedUser : DynamicDictionary
{
    public int Id => GetValue<int>("id");
    public string Email => GetValue<string>("email");
    
    public bool IsValid()
    {
        return Id > 0 && !string.IsNullOrWhiteSpace(Email);
    }
    
    public void Validate()
    {
        if (!IsValid())
            throw new InvalidOperationException($"Invalid user data: {this}");
    }
}
```

## Migration from Dynamic-Only Code

### Before (Dynamic Only)

```csharp
var serializer = new SystemTextJsonDynamicsSerializer();
dynamic user = DynamicDictionary.Create(json, serializer);

// No IntelliSense, no type safety
Console.WriteLine(user.name);
Console.WriteLine(user.email);

// Easy to make typos
Console.WriteLine(user.naem);  // Runtime error - no compile-time check
```

### After (Strongly-Typed)

```csharp
var serializer = new SystemTextJsonDynamicsSerializer();
var user = DynamicDictionary.Create<JsonPlaceholderUser>(json, serializer);

// Full IntelliSense and type safety
Console.WriteLine(user.Name);   // Compile-time checked
Console.WriteLine(user.Email);  // Autocomplete support

// Typos caught at compile time
// Console.WriteLine(user.Naem);  // Compile error!
```

## Advanced Scenarios

### Custom Business Logic

```csharp
public class EnhancedTodo : DynamicDictionary
{
    public int Id => GetValue<int>("id");
    public string Title => GetValue<string>("title");
    public bool Completed => GetValue<bool>("completed");
    
    // Business logic
    public string Status => Completed ? "✓ Completed" : "○ Pending";
    
    public void MarkComplete()
    {
        this["completed"] = true;
    }
    
    public void MarkPending()
    {
        this["completed"] = false;
    }
}
```

### Computed Properties

```csharp
public class EnhancedUser : DynamicDictionary
{
    public string Name => GetValue<string>("name");
    public string Email => GetValue<string>("email");
    public dynamic Company => this["company"];
    
    // Computed properties
    public string DisplayName => $"{Name} ({Email})";
    public string CompanyInfo => $"{Company?.name} - {Company?.catchPhrase}";
    public bool HasCompany => Company != null;
}
```

## Summary

| Feature | Benefit |
|---------|---------|
| **Type Constraints** | Only `DynamicDictionary`-derived types allowed |
| **Generic Methods** | `Create<T>()` and `CreateArray<T>()` with compile-time safety |
| **IntelliSense** | Full IDE support for strongly-typed properties |
| **Flexibility** | Dynamic access still available for nested/unknown data |
| **Debugging** | Custom `ToString()` and type-safe access methods |
| **Migration** | Easy to migrate existing dynamic code incrementally |

## See Also

- **JsonPlaceholderModels.cs**: Ready-to-use model examples
- **StronglyTypedModelExample.cs**: Comprehensive usage examples
- **DEBUGGING_GUIDE.md**: Tips for debugging dynamic types
- **EXAMPLES_SUMMARY.md**: Overview of all examples

