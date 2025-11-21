# DynamicDictionary REST API Examples

This folder contains practical examples demonstrating the advantages of `DynamicDictionary` when working with REST APIs and JSON data.

## 🐛 Debugging Tips

**Can't see `dynamic` variable properties in the debugger?**

This is because the `dynamic` type is evaluated at runtime.

**Quick Solution:**
- In Watch window, type `post._data["id"]` to see the actual value
- For detailed guide, see **[DEBUGGING_GUIDE.md](DEBUGGING_GUIDE.md)**

---

## Why DynamicDictionary?

### Traditional Approach (POCO Classes)
```csharp
public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    // Need to define ALL properties, even if you only use a few
    // Breaking changes if API adds new fields
    // Strict typing reduces flexibility
}

// Consume
var post = JsonConvert.DeserializeObject<Post>(json);
var title = post.Title;
```

### DynamicDictionary Approach
```csharp
public sealed class JsonPlaceholderPost : DynamicDictionary
{
    public int Id => this.GetValue<int>("id");
    public int UserId => this.GetValue<int>("userId");
    public string Title => this["title"] as string;
    public string Body => this["body"] as string;
    // Only define the properties you need
    // Automatically handles extra fields from API
    // Add custom properties dynamically if needed
}

// Consume
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var post = serializer.Deserialize(json);
var typedPost = new JsonPlaceholderPost();
foreach (var kvp in post)
    typedPost[kvp.Key] = kvp.Value;

var title = typedPost.Title;
```

## Key Advantages

### 1. **Selective Property Definition**
Only define the fields you actually use, not the entire API response structure.

### 2. **API Evolution Resilience**
When APIs add new fields, your code doesn't break. Just access new fields as needed.

### 3. **Dynamic Property Access**
Access any field without pre-definition:
```csharp
var customField = post["some_new_field"];
post["custom_metadata"] = "added at runtime";
```

### 4. **Nested Path Navigation**
Easily access deeply nested properties with dot notation:
```csharp
var companyName = user["company.name"];
var latitude = user["address.geo.lat"];
```

### 5. **Flexible Type Conversion**
Automatic type conversion with sensible defaults:
```csharp
int id = post.GetValue<int>("id", -1);                    // -1 if not found
bool active = post.GetValue<bool>("active", false);       // false if not found
decimal price = post.GetValue<decimal>("price", 0m);      // 0m if not found
```

### 6. **Merge Multiple API Responses**
Combine data from different endpoints effortlessly:
```csharp
var post = FetchPost();
var user = FetchUser(post.UserId);
post["author_name"] = user["name"];
post["author_email"] = user["email"];
```

## Examples Included

### JsonPlaceholderModels.cs
Defines strongly-typed classes extending `DynamicDictionary` for:
- **JsonPlaceholderUser** - User data with nested company/address info
- **JsonPlaceholderPost** - Blog posts with formatting methods
- **JsonPlaceholderTodo** - Todo items with status display
- **JsonPlaceholderComment** - Comments with author information
- **JsonPlaceholderPhoto** - Photos with thumbnail URLs

### RestApiUsageExample.cs
Seven practical examples demonstrating:

1. **Example 1: Fetch and Display a Single Post**
   - Basic JSON parsing and property access
   - Direct field access vs. method calls

2. **Example 2: Process Multiple Posts**
   - Handling array responses
   - Filtering and iteration

3. **Example 3: Fetch User with Nested Data**
   - Accessing nested objects using dot notation
   - Combining multiple property access methods

4. **Example 4: Todos with Status Display**
   - Extending DynamicDictionary with custom methods
   - Working with boolean fields and formatting

5. **Example 5: Comments with Optional Fields**
   - Handling missing/null fields gracefully
   - Type conversion with defaults

6. **Example 6: Dynamic Property Access**
   - Iterating through all fields
   - Adding custom properties at runtime
   - Accessing unknown fields

7. **Example 7: Merge Data from Multiple Endpoints**
   - Combining data from different API sources
   - Enriching responses with additional data

## Running the Examples

### Prerequisites
- .NET 8.0 or later

### Build and Run
```bash
cd E:\OneCiel
dotnet build
dotnet run --project Examples
```

### What to Expect
The example runs 7 different demonstrations that:
- Connect to the free JSONPlaceholder API
- Fetch real data from public endpoints
- Process and display the data using DynamicDictionary
- Show different patterns and best practices

**Note:** The examples use the free JSONPlaceholder API (https://jsonplaceholder.typicode.com), so you need internet access. No API key is required.

## Common Patterns

### Pattern 1: Wrap API Response
```csharp
var json = await httpClient.GetStringAsync(url);
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var raw = serializer.Deserialize(json);

var typed = new JsonPlaceholderPost();
foreach (var kvp in raw)
    typed[kvp.Key] = kvp.Value;

Console.WriteLine(typed.Title);
```

### Pattern 2: Access Only What You Need
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var data = serializer.Deserialize(json);
var id = data.GetValue<int>("id");
var name = data["name"] as string;
// Don't need to define all other fields
```

### Pattern 3: Extend with Methods
```csharp
public sealed class MyModel : DynamicDictionary
{
    public string Display => $"{this["first"]} {this["last"]}";
    
    public void PrintDetails()
    {
        foreach (var kvp in this)
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
    }
}
```

### Pattern 4: Add Custom Data
```csharp
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var response = serializer.Deserialize(json);
response["fetched_at"] = DateTime.UtcNow;
response["source"] = "my_api";
response["is_cached"] = false;
// Save/transmit enriched data
```

## API Used

These examples use [JSONPlaceholder](https://jsonplaceholder.typicode.com), a free online JSON API providing:
- **Posts** - Blog post data
- **Users** - User profiles with nested address/company
- **Comments** - Comments on posts
- **Todos** - Todo list items
- **Photos** - Photo gallery with thumbnails
- **Albums** - Album collections

No authentication required. Perfect for testing and learning.

## Benefits Over Strict POCO Classes

| Aspect | POCO Classes | DynamicDictionary |
|--------|--------------|------------------|
| API Evolution | ❌ Breaking changes | ✅ Automatic handling |
| Define All Fields | ✅ Required | ❌ Only define needed |
| Add Runtime Fields | ❌ Not possible | ✅ Easy |
| Nested Access | ❌ Requires classes | ✅ Dot notation |
| Type Flexibility | ❌ Strict types | ✅ Flexible |
| Merge Data | ❌ Manual merge | ✅ Easy merge |
| Learning Curve | ✅ Simple | ❌ More powerful |

## Real-World Use Cases

### 1. Microservices Integration
Consume APIs from multiple microservices without creating DTO for each.

### 2. Third-Party APIs
Use external APIs (GitHub, Twitter, Stripe) without maintaining matching models.

### 3. Kafka/Message Queue Processing
Handle messages with varying structures in a single handler.

### 4. MongoDB/NoSQL Database
Work with semi-structured or unstructured document data.

### 5. Event Processing
Process events with optional fields that vary between event types.

### 6. API Aggregation
Combine data from multiple APIs into enriched responses.

### 7. Rapid Prototyping
Quickly prototype without defining full data models upfront.

## Best Practices

1. **Define Common Properties**
   Define properties you access frequently in your subclass.

2. **Use Type-Safe Methods**
   Use `GetValue<T>()` instead of casting for type safety.

3. **Document Optional Fields**
   Add XML comments explaining which fields are optional.

4. **Provide Defaults**
   Always provide sensible default values in `GetValue<T>()`.

5. **Extend with Methods**
   Add business logic as methods in your DynamicDictionary subclass.

6. **Handle Missing Fields**
   Always check for field existence or provide defaults.

7. **Keep Subclasses Sealed**
   As shown in examples, prevent unintended inheritance.

## License

MIT License - See LICENSE in parent directory.

## See Also

- [OneCiel.System.Dynamics](../README.md) - Main library documentation
- [OneCiel.System.Dynamics.JsonExtension](../OneCiel.System.Dynamics.JsonExtension/README.md) - JSON extension documentation
