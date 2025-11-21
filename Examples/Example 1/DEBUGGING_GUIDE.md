# DynamicDictionary Debugging Guide

## ⚠️ Problem: Dynamic Properties Not Visible in Debugger

When using the `dynamic` type, properties are not automatically displayed in Visual Studio's Watch window or QuickWatch. This is because `dynamic` types are **interpreted at runtime**.

### Example Code

```csharp
// Using the new unified interface
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
dynamic post = DynamicDictionary.Create(postJson, serializer);

// ❌ In debugger Watch window, typing post.id won't show the value!
```

### Why Can't I See It?

The `dynamic` type resolves members at **runtime**, not **compile time**. The debugger uses compile-time information, so it cannot evaluate dynamic member access like `post.id`.

---

## ✅ Solutions

### Solution 1: Access Internal Dictionary Directly (★ Recommended)

In the debugger Watch window, type:

```
post._data["id"]
```

Or to see all data:

```
post._data
```

**Advantages:**
- ✅ View all key-value pairs at a glance
- ✅ Understand the actual stored data structure
- ✅ Fastest and most intuitive

**Example Output:**
```
post._data["id"] = 1
post._data["title"] = "sunt aut facere..."
post._data["userId"] = 1
```

---

### Solution 2: Cast Explicitly Then Use Indexer

In the debugger Watch window, type:

```
((DynamicDictionary)post)["id"]
```

**Advantages:**
- ✅ Maintains type safety
- ✅ Clear access through indexer
- ✅ Supports nested paths (e.g., `["user.address.city"]`)

---

### Solution 3: Change Code to Use Explicit Type

Use explicit type instead of dynamic at runtime:

```csharp
// Using the new unified serializer
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();

// ✅ Use explicit type instead of dynamic
DynamicDictionary post = serializer.Deserialize(postJson);

// Access Method 1: Use indexer
var id = post["id"];

// Access Method 2: Use GetValue<T> (type-safe)
int id = post.GetValue<int>("id");

// Access Method 3: Cast to dynamic (still available)
dynamic dynPost = post;
var id = dynPost.id;
```

**Advantages:**
- ✅ View type information in debugger
- ✅ IntelliSense support (Keys, Count, GetValue, etc.)
- ✅ Type safety
- ✅ Can use `post["id"]` directly in Watch window

**Disadvantages:**
- ⚠️ Less convenience of dynamic property access (`post.id`)

---

### Solution 4: Use GetValue<T> Method (Type-Safe)

Type-safe value retrieval:

```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
dynamic post = DynamicDictionary.Create(postJson, serializer);

// In debugger Watch window:
((DynamicDictionary)post).GetValue<int>("id")
((DynamicDictionary)post).GetValue<string>("title")
((DynamicDictionary)post).GetValue<int>("userId", 0)  // with default value
```

**Advantages:**
- ✅ Automatic type conversion
- ✅ Can specify default values
- ✅ Null-safe

---

## 📋 Real-World Debugging Examples

### Scenario 1: Checking Values at Breakpoint

```csharp
public static async Task Example_Debugging()
{
    var postJson = await _httpClient.GetStringAsync($"{url}/posts/1");
    
    // Using new unified serializer
    var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
    dynamic post = DynamicDictionary.Create(postJson, serializer);
    
    // ← Set breakpoint here
    Console.WriteLine($"Post #{post.id}");
    
    // Add to Watch window:
    // post._data                          → View all data
    // post._data["id"]                    → Check ID value (recommended)
    // ((DynamicDictionary)post)["id"]     → Access via indexer
    // ((DynamicDictionary)post).GetValue<int>("id")  → Type-safe access
}
```

### Scenario 2: Debugging Array Data

```csharp
public static async Task DebugArrayExample()
{
    var postsJson = await _httpClient.GetStringAsync($"{url}/posts?_limit=3");
    
    // Parse array with unified serializer
    var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
    dynamic posts = DynamicDictionary.CreateArray(postsJson, serializer);
    
    foreach (dynamic post in posts)
    {
        // ← Set breakpoint here
        Console.WriteLine($"[{post.id}] {post.title}");
        
        // Add to Watch window:
        // posts.Length                     → Array length
        // post._data                       → Current post's all data
        // post._data["id"]                 → Current post's ID
        // ((DynamicDictionary)post).Keys   → Current post's all keys
    }
}
```

### Scenario 3: Improved Debugging with Explicit Type

```csharp
public static async Task DebugExplicitType()
{
    var postJson = await _httpClient.GetStringAsync($"{url}/posts/1");
    
    // Method B: Explicit type (much easier debugging!)
    var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
    DynamicDictionary post = serializer.Deserialize(postJson);
    
    // Set debugger breakpoint 👇
    var id = post["id"];                    // ← Set breakpoint here
    var title = post.GetValue<string>("title");
    
    // ✅ Direct use in Watch window:
    // post                            → View entire object (IntelliSense supported)
    // post["id"]                      → Access via indexer
    // post.GetValue<int>("id")        → Type-safe access
    // post.Keys                       → All keys list
    // post.Count                      → Number of properties
    // post._data                      → Direct internal Dictionary access
}
```

---

## Visual Studio Debugger Tips

### Useful Expressions in Watch Window

| Expression | Description | Example Result |
|------------|-------------|----------------|
| `post._data` | View entire Dictionary | `{[id, 1], [title, "..."], ...}` |
| `post._data["id"]` | Access specific value | `1` |
| `post._data.Keys` | All keys list | `{id, userId, title, body}` |
| `post._data.Values` | All values list | `{1, 1, "...", "..."}` |
| `post._data.Count` | Number of properties | `4` |
| `((DynamicDictionary)post)["id"]` | Access after casting | `1` |
| `((DynamicDictionary)post).GetValue<int>("id")` | Type-safe access | `1` |

### Using QuickWatch

1. Hover over the `post` variable in code
2. Click magnifying glass icon (🔍) or press `Shift+F9`
3. Type `post._data` in Expression field
4. View entire Dictionary contents

### Using Immediate Window

Open Immediate window with `Ctrl+Alt+I` in debug mode:

```csharp
// Immediate evaluation
? post._data["id"]
1

? post._data.Keys
{id, userId, title, body}

// Complex expressions also work
? post._data.Where(x => x.Value is string).Select(x => x.Key)
{title, body}
```

---

## Performance Considerations

### dynamic vs Explicit Type

| Characteristic | dynamic | DynamicDictionary |
|----------------|---------|-------------------|
| Runtime Overhead | Some overhead | None |
| Type Safety | No | Yes (when using indexer/GetValue) |
| IntelliSense | Not supported | Supported |
| Debugging | Difficult | Easy |
| Code Conciseness | High | Medium |

**Recommendations:**
- **Production Code**: Explicit type + `GetValue<T>` (type-safe)
- **Prototype/Scripts**: `dynamic` (rapid development)
- **During Debugging**: Explicit type (debugging convenience)

---

## Best Practices

### ✅ Recommended

```csharp
// 1. Production: Type-safe + clear error handling (using unified serializer)
public int GetPostId(string json)
{
    var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
    var post = serializer.Deserialize(json);
    return post.GetValue<int>("id", -1); // Provide default value
}

// 2. Debugging: Explicit type (easy to see in debugger!)
public void DebugPost(string json)
{
    var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
    DynamicDictionary post = serializer.Deserialize(json);
    // ← Set breakpoint - can see post["id"], post._data in Watch
    Console.WriteLine(post["id"]);
}

// 3. Quick prototype: dynamic (convenient but harder to debug)
public void QuickPrototype(string json)
{
    var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
    dynamic post = DynamicDictionary.Create(json, serializer);
    Console.WriteLine(post.id); // Quick and concise
    // ⚠️ Can't see post.id in debugger! Use post._data["id"]
}

// 4. Reuse unified serializer (performance optimization)
private static readonly IDynamicsJsonSerializer _serializer = 
    DynamicDictionaryJsonExtensions.CreateDefaultSerializer();

public void ProcessMultiplePosts(string[] jsonArray)
{
    foreach (var json in jsonArray)
    {
        var post = _serializer.Deserialize(json);
        // Reuse same serializer - more efficient
    }
}
```

### ❌ Avoid

```csharp
// Bad Example 1: Using dynamic without type checking
dynamic post = DynamicDictionary.Create(json, serializer);
int id = post.id; // Runtime error possible (id might not exist or be different type)

// Bad Example 2: Unnecessary double casting
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
dynamic post = (dynamic)serializer.Deserialize(json); // (dynamic) cast unnecessary

// Bad Example 3: Using try-catch for type validation
try {
    int id = post.id;
} catch {
    // Don't use exceptions for type checking
}

// Good Example: Handle safely with GetValue<T>
var postDict = serializer.Deserialize(json);
int id = postDict.GetValue<int>("id", -1);
```

---

## Frequently Asked Questions (FAQ)

### Q1: Why can't I see post.id in the Watch window at breakpoint?

**A:** The `dynamic` type **resolves members at runtime**, so the debugger cannot evaluate `post.id` using compile-time information only.

**Solutions:**
- Type `post._data["id"]` in Watch window (easiest)
- Or use `((DynamicDictionary)post)["id"]`
- Change code to `DynamicDictionary post = serializer.Deserialize(json)`

### Q2: Is it okay to use dynamic in production?

**A:** Possible but not recommended. Lack of type safety can cause runtime errors.

**Recommended Approach:**
```csharp
// ✅ Type-safe
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var post = serializer.Deserialize(json);
int id = post.GetValue<int>("id", -1); // Provide default value
```

### Q3: Is there a performance difference?

**A:** `dynamic` has some overhead (about 10-20%) because members are resolved at runtime. Usually negligible, but use explicit types in performance-critical loops.

```csharp
// For performance-critical code
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
DynamicDictionary post = serializer.Deserialize(json);
int id = post.GetValue<int>("id");  // Faster than dynamic
```

### Q4: How do I use IntelliSense?

**A:** Use explicit type declaration:
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
DynamicDictionary post = serializer.Deserialize(json);
// ✅ IntelliSense shows post.Keys, post.Count, post["id"], post.GetValue<T>, etc.
```

### Q5: What's the difference between IDynamicsJsonSerializer and the previous IJsonDeserializer?

**A:** `IDynamicsJsonSerializer` is a unified interface that provides both serialization and deserialization.

```csharp
// New approach (recommended)
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var data = serializer.Deserialize(json);      // Deserialize
string json = serializer.Serialize(data);     // Serialize (same object!)

// Previous approach (deprecated)
var deserializer = DynamicDictionaryJsonExtensions.CreateDefaultDeserializer();
var data = deserializer.Deserialize(json);
```

---

## Additional Resources

- [DynamicObject Official Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject)
- [Visual Studio Debugging Guide](https://docs.microsoft.com/en-us/visualstudio/debugger/)
- [C# dynamic Keyword](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/using-type-dynamic)

---

## Summary

### 🔴 Problem: post.id Not Visible in Watch Window at Breakpoint

**Cause:**
- `dynamic` type is evaluated at runtime with **no compile-time information**
- Debugger uses compile-time information only, so cannot evaluate `post.id`

### ✅ Solutions

#### Solution 1: Access Internal Dictionary in Watch Window (★ Recommended)
```
post._data["id"]        → Fastest and easiest
post._data              → View all data
```

#### Solution 2: Change Code to Use Explicit Type
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
DynamicDictionary post = serializer.Deserialize(json);  // Instead of dynamic
// ✅ Now can use post["id"], post.Keys, etc. in Watch window
```

#### Solution 3: Cast Then Access
```
((DynamicDictionary)post)["id"]
((DynamicDictionary)post).GetValue<int>("id")
```

### 🎯 Recommended Patterns

| Situation | Recommended Approach | Example |
|-----------|---------------------|---------|
| **Debugging** | Explicit type | `DynamicDictionary post = serializer.Deserialize(json)` |
| **Production** | GetValue<T> | `int id = post.GetValue<int>("id", -1)` |
| **Prototype** | dynamic | `dynamic post = DynamicDictionary.Create(json, serializer)` |

### 💡 Additional Tips

- **Use unified serializer**: Handle both serialization/deserialization with `IDynamicsJsonSerializer`
- **Performance optimization**: Reuse serializer instance for repeated operations
- **During debugging**: Always add `post._data` to Watch to see actual data
