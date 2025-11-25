# Quick Start Guide - DynamicDictionary Examples

## What Are These Examples?

These examples demonstrate how `DynamicDictionary` simplifies working with REST APIs and JSON data compared to traditional strongly-typed POCO classes.

## How to Run

### Option 1: Run from Command Line
```bash
cd E:\OneCiel
dotnet run --project Examples
```

### Option 2: Run from Visual Studio
1. Open `OneCiel.Core.Dynamics.sln` in Visual Studio
2. Set `Examples` as the startup project
3. Press F5 or click Run

### Option 3: Build and Run Executable
```bash
cd E:\OneCiel
dotnet build --configuration Release
dotnet run --project Examples --configuration Release
```

## Example Output

When you run the examples, you'll see output like:

```
╔════════════════════════════════════════════════════════════╗
║   DynamicDictionary with REST API - Practical Examples      ║
║   Using JSONPlaceholder API (https://jsonplaceholder.typicode.com) ║
╚════════════════════════════════════════════════════════════╝

=== Example 1: Fetch Post ===

Post #1
Author: User 1
Title: sunt aut facere repellat provident...
Content: quia et suscipit...

Direct access - UserId: 1

=== Example 2: Process Multiple Posts ===

Fetched 5 posts:

  [1] User 1: sunt aut facere repellat provident...
  [2] User 1: qui est esse
  [3] User 1: et ea vero quia laudantium...
  [4] User 1: quia molestias consequuntur neque...
  [5] User 1: dignissimos ducimus qui blanditiis...

[... more examples follow ...]
```

## Understanding the Examples

### Example 1: Basic POST Request
```csharp
// Fetch from REST API
var postJson = await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/posts/1");

// Create serializer and parse JSON to DynamicDictionary
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var post = serializer.Deserialize(postJson);

// Access properties
var id = post.GetValue<int>("id");
var title = post["title"] as string;
var content = post["body"] as string;
```

### Example 2: Handling Arrays
```csharp
// Multiple posts
var postsJson = await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/posts?_limit=5");

// Parse array
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var postsArray = serializer.Deserialize($"{{\"posts\": {postsJson}}}");

// Iterate through results
if (postsArray.TryGetValue("posts", out var posts) && posts is object[] array)
{
    foreach (var post in array)
    {
        var typedPost = (DynamicDictionary)post;
        Console.WriteLine(typedPost["title"]);
    }
}
```

### Example 3: Nested Properties
```csharp
// User with nested data
var userJson = await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/users/1");
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var user = serializer.Deserialize(userJson);

// Access nested properties with dot notation
string companyName = user["company.name"] as string;
decimal latitude = user.GetValue<decimal>("address.geo.lat");
```

### Example 4: Custom Methods
```csharp
public sealed class JsonPlaceholderTodo : DynamicDictionary
{
    public int Id => this.GetValue<int>("id");
    public string Title => this["title"] as string;
    public bool Completed => this.GetValue<bool>("completed");
    
    // Custom method
    public string GetSummary()
    {
        return $"[{(Completed ? "✓" : "○")}] {Title}";
    }
}

// Usage
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var todoDict = serializer.Deserialize(json);
var todo = new JsonPlaceholderTodo();
foreach (var kvp in todoDict)
    todo[kvp.Key] = kvp.Value;

Console.WriteLine(todo.GetSummary()); // [✓] Learn DynamicDictionary
```

### Example 5: Optional Fields
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var data = serializer.Deserialize(json);

// Safe access with defaults
int id = data.GetValue<int>("id", -1);              // -1 if not found
string name = data.GetValue<string>("name", "N/A"); // "N/A" if null
bool active = data.GetValue<bool>("active", false); // false if not found
```

### Example 6: Dynamic Access
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var post = serializer.Deserialize(json);

// Iterate all fields
foreach (var kvp in post)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}

// Add custom fields
post["fetched_at"] = DateTime.UtcNow;
post["is_important"] = true;
post["tags"] = new[] { "example", "demo" };
```

### Example 7: Merge Multiple API Calls
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();

// Fetch post
var postJson = await httpClient.GetStringAsync($".../posts/1");
var post = serializer.Deserialize(postJson);

// Fetch user data
var userId = post.GetValue<int>("userId");
var userJson = await httpClient.GetStringAsync($".../users/{userId}");
var user = serializer.Deserialize(userJson);

// Enrich post with user data
post["author_name"] = user["name"];
post["author_email"] = user["email"];
post["author_company"] = user["company.name"];

// Result: Single enriched object
```

## Key Concepts Demonstrated

### 1. JSON Parsing
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var dict = serializer.Deserialize(jsonString);
```

### 2. Type-Safe Access
```csharp
int id = dict.GetValue<int>("id", -1);
```

### 3. Cast Access
```csharp
string name = dict["name"] as string;
```

### 4. Nested Navigation
```csharp
var value = dict["parent.child.grandchild"];
```

### 5. Array Access
```csharp
var first = dict["items[0]"];
var second = dict["items[1].property"];
```

### 6. Iteration
```csharp
foreach (var kvp in dict)
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
```

### 7. Extension Methods
```csharp
public sealed class MyModel : DynamicDictionary
{
    public string Display => $"{this["first"]} {this["last"]}";
    
    public void PrintAll()
    {
        foreach (var kvp in this)
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
    }
}
```

## JSONPlaceholder API Endpoints Used

- `GET /posts/1` - Single post
- `GET /posts?_limit=5` - Multiple posts
- `GET /users/1` - Single user with nested data
- `GET /todos?userId=1&_limit=5` - User's todos
- `GET /comments?postId=1&_limit=3` - Post comments

No authentication required. Limit is usually 10 seconds between requests.

## Why This Matters

### Without DynamicDictionary:
```csharp
public class Post { 
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    // Must define EVERY field
    // Breaks if API adds fields
    // No way to add runtime data
}

// Deserialize
var post = JsonConvert.DeserializeObject<Post>(json);
```

### With DynamicDictionary:
```csharp
public sealed class Post : DynamicDictionary
{
    public int Id => this.GetValue<int>("id");
    public string Title => this["title"] as string;
    // Define only what you use
    // Extra fields don't break anything
    // Add data at runtime
}

// Deserialize
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var raw = serializer.Deserialize(json);
var post = new Post();
foreach (var kvp in raw)
    post[kvp.Key] = kvp.Value;
```

## Common Issues and Solutions

### Issue: "Unable to connect to JSONPlaceholder"
**Solution:** Ensure you have internet connection. The API is public and free.

### Issue: "Property not found"
**Solution:** Use `GetValue<T>()` with a default value or check with `TryGetValue()`.

```csharp
// Safe approach
string value = data.GetValue<string>("missing_field", "default");

// Alternative
if (data.TryGetValue("field", out var value))
    Console.WriteLine(value);
```

### Issue: "Cannot cast to type X"
**Solution:** Use `GetValue<T>()` instead of `as` casting for automatic conversion.

```csharp
// Bad
int id = (int)data["id"]; // May throw

// Good
int id = data.GetValue<int>("id", -1); // Returns -1 if conversion fails
```

## Next Steps

1. **Run the examples** to see DynamicDictionary in action
2. **Study the code** to understand the patterns
3. **Try the models** in your own projects
4. **Read the documentation** for more details
5. **Explore nested access** with your own JSON data

## Resources

- JSONPlaceholder API: https://jsonplaceholder.typicode.com
- Main Documentation: See [README.md](../README.md)
- Library Docs: See [OneCiel.Core.Dynamics](../OneCiel.Core.Dynamics/README.md)
- JSON Extension: See [JsonExtension](../OneCiel.Core.Dynamics.JsonExtension/README.md)

## Need Help?

Check the code comments in:
- `JsonPlaceholderModels.cs` - Model definitions
- `RestApiUsageExample.cs` - Usage examples

Each example is well-commented to explain what's happening.

Enjoy using DynamicDictionary! 🚀
