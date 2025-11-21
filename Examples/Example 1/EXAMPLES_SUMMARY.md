# Examples Project - Summary

## 📁 Files Created

```
E:\OneCiel\Examples/
├── Examples.csproj                    # .NET 8.0 console application
├── JsonPlaceholderModels.cs           # DynamicDictionary-based model classes
├── RestApiUsageExample.cs             # 7 practical usage examples
├── README.md                          # Detailed guide and patterns
└── QUICKSTART.md                      # Quick start and common patterns
```

## 🎯 Project Purpose

Demonstrates the real-world advantages of `DynamicDictionary` when working with REST APIs and JSON data, compared to traditional POCO (Plain Old CLR Object) classes.

## 📦 What's Included

### JsonPlaceholderModels.cs (260 lines)
Defines 5 classes extending `DynamicDictionary`:

1. **JsonPlaceholderUser** - User profiles with nested company/address
2. **JsonPlaceholderPost** - Blog posts with formatting
3. **JsonPlaceholderTodo** - Todo items with status display
4. **JsonPlaceholderComment** - Comments with author information
5. **JsonPlaceholderPhoto** - Photos with thumbnail URLs

Each class:
- Only defines properties you actually use
- Includes XML documentation
- Provides custom methods where useful
- Handles nested data with dot notation

### RestApiUsageExample.cs (400+ lines)
7 complete, runnable examples:

| Example | Demonstrates |
|---------|--------------|
| Example 1 | Fetch and parse a single REST API response |
| Example 2 | Process collections from API array responses |
| Example 3 | Access nested properties using dot notation |
| Example 4 | Extend DynamicDictionary with custom methods |
| Example 5 | Handle optional/missing fields gracefully |
| Example 6 | Dynamic property access and runtime modifications |
| Example 7 | Merge data from multiple API endpoints |

Each example includes:
- Full implementation with error handling
- Detailed comments explaining the approach
- Real API calls to JSONPlaceholder
- Output demonstrations

### README.md (350+ lines)
Comprehensive guide covering:
- Why DynamicDictionary is better than POCO classes
- Key advantages and benefits
- Description of each example
- Common patterns and best practices
- Real-world use cases
- Comparison table: POCO vs DynamicDictionary

### QUICKSTART.md (400+ lines)
Beginner-friendly guide including:
- How to run the examples
- Example-by-example walkthrough
- Key concepts demonstrated
- JSONPlaceholder API reference
- Common issues and solutions
- Next steps for learning

## 🚀 How to Run

### Quick Start
```bash
cd E:\OneCiel
dotnet run --project Examples
```

### What Happens
1. Connects to free JSONPlaceholder API
2. Fetches real data from public endpoints
3. Processes data using DynamicDictionary
4. Displays formatted output

### Expected Output
```
╔════════════════════════════════════════════════════════════╗
║   DynamicDictionary with REST API - Practical Examples      ║
║   Using JSONPlaceholder API                                ║
╚════════════════════════════════════════════════════════════╝

=== Example 1: Fetch Post ===
Post #1
Author: User 1
Title: sunt aut facere repellat provident...
...
```

## 💡 Key Concepts Shown

### 1. Selective Property Definition
```csharp
public sealed class JsonPlaceholderPost : DynamicDictionary
{
    public int Id => this.GetValue<int>("id");
    public string Title => this["title"] as string;
    // Only define properties you use
}
```

### 2. Nested Property Access
```csharp
// No need for nested classes
var companyName = user["company.name"];
var latitude = user["address.geo.lat"];
```

### 3. Type-Safe Access with Defaults
```csharp
// Automatic type conversion with sensible defaults
int id = post.GetValue<int>("id", -1);
bool active = post.GetValue<bool>("active", false);
decimal price = post.GetValue<decimal>("price", 0m);
```

### 4. Dynamic Property Addition
```csharp
// Add runtime properties
post["fetched_at"] = DateTime.UtcNow;
post["is_important"] = true;
post["tags"] = new[] { "example" };
```

### 5. Custom Methods
```csharp
public sealed class JsonPlaceholderTodo : DynamicDictionary
{
    public string GetSummary() => 
        $"[{(Completed ? "✓" : "○")}] {Title}";
}
```

### 6. Data Merging
```csharp
// Combine multiple API responses
var post = FetchPost();
var user = FetchUser(post.UserId);
post["author_name"] = user["name"];
post["author_email"] = user["email"];
```

## 📊 Benefits Demonstrated

| Scenario | POCO Classes | DynamicDictionary |
|----------|--------------|------------------|
| API adds new field | ❌ Breaking change | ✅ Works fine |
| Only use 3 of 20 fields | ❌ Define all 20 | ✅ Define 3 only |
| Need nested access | ❌ Create nested classes | ✅ Use dot notation |
| Add runtime metadata | ❌ Not possible | ✅ Easy |
| Multiple API formats | ❌ Different classes | ✅ Same class |
| Rapid prototyping | ❌ Setup time | ✅ Immediate |

## 🌐 API Used

**JSONPlaceholder** - Free online JSON API
- URL: https://jsonplaceholder.typicode.com
- No authentication required
- Perfect for testing and learning
- Includes Posts, Users, Todos, Comments, Photos, Albums

Endpoints used in examples:
- `GET /posts/1`
- `GET /posts?_limit=5`
- `GET /users/1`
- `GET /todos?userId=1&_limit=5`
- `GET /comments?postId=1&_limit=3`

## 📚 Documentation Structure

```
E:\OneCiel/
├── README.md                          # Main project overview
├── OneCiel.System.Dynamics/
│   └── README.md                      # Core library docs
├── OneCiel.System.Dynamics.JsonExtension/
│   └── README.md                      # JSON extension docs
└── Examples/
    ├── README.md                      # Detailed guide & patterns
    ├── QUICKSTART.md                  # Quick start guide
    ├── JsonPlaceholderModels.cs       # Model definitions
    └── RestApiUsageExample.cs         # 7 complete examples
```

## 🎓 Learning Path

1. **Start Here**: Read Examples/QUICKSTART.md (10 min)
2. **Run Examples**: Execute `dotnet run --project Examples` (2 min)
3. **Study Code**: Review JsonPlaceholderModels.cs (10 min)
4. **Explore Examples**: Read RestApiUsageExample.cs (15 min)
5. **Deep Dive**: Read Examples/README.md (20 min)
6. **Full Understanding**: Check main README.md (30 min)

Total learning time: ~90 minutes

## 🔍 Code Quality

All example code includes:
- ✅ Full XML documentation
- ✅ Comprehensive comments
- ✅ Error handling
- ✅ Best practices
- ✅ Clear naming conventions
- ✅ Proper formatting
- ✅ English documentation

## 🚀 Next Steps After Examples

1. **Try with Your API**: Adapt examples to your REST API
2. **Create Models**: Build DynamicDictionary subclasses for your data
3. **Add Methods**: Extend with business logic methods
4. **Integrate**: Use in your production application
5. **Share Knowledge**: Show your team how to use DynamicDictionary

## 📝 Example Use Cases

### Microservices
```csharp
// Consume multiple microservices without creating DTO for each
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var userService = await client.GetStringAsync("http://user-api/users/1");
var orderService = await client.GetStringAsync("http://order-api/orders/1");
var userObj = serializer.Deserialize(userService);
var orderObj = serializer.Deserialize(orderService);
```

### Third-Party APIs
```csharp
// Work with GitHub, Twitter, Stripe APIs without maintaining DTO models
var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
var github = await client.GetStringAsync("https://api.github.com/repos/owner/repo");
var repo = serializer.Deserialize(github);
```

### Event Processing
```csharp
// Handle Kafka messages with varying structures
public void ProcessMessage(string json)
{
    var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
    var message = serializer.Deserialize(json);
    var type = message.GetValue<string>("type");
    // Handle different message types
}
```

### Database Documents
```csharp
// Work with MongoDB/CosmosDB documents
var dbDocument = await collection.Find(filter).FirstOrDefaultAsync();
var doc = new DynamicDictionary(dbDocument.ToBsonDocument().ToJson());
```

## ⚠️ Requirements

- .NET 8.0 or later
- Internet connection (for JSONPlaceholder API)
- Projects: OneCiel.System.Dynamics and JsonExtension

## 📄 License

MIT License - Same as main project

## 💬 Questions?

Review the comments in the example files or check the documentation in README.md files.

---

**Status**: ✅ Complete and Ready to Run
**Created**: January 2024
**Version**: 1.0.0
