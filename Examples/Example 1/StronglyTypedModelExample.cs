using System;
using System.Net.Http;
using System.Threading.Tasks;
using OneCiel.System.Dynamics;

namespace Examples
{
    /// <summary>
    /// Demonstrates using strongly-typed models that inherit from DynamicDictionary.
    /// Shows how to use the generic Create&lt;T&gt; and CreateArray&lt;T&gt; methods for type-safe REST API consumption.
    /// </summary>
    public sealed class StronglyTypedModelExample
    {
        private static readonly HttpClient _httpClient = new();
        private const string JsonPlaceholderBaseUrl = "https://jsonplaceholder.typicode.com";

        /// <summary>
        /// Example 1: Fetch a single user with strongly-typed model.
        /// Demonstrates the generic Create&lt;T&gt; method with type constraint.
        /// </summary>
        public static async Task Example1_FetchStronglyTypedUser()
        {
            Console.WriteLine("=== Example 1: Strongly-Typed User Model ===\n");

            try
            {
                // Fetch user JSON from REST API
                var userJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/users/1");

                // Use generic Create<T> method with default serializer - T must inherit from DynamicDictionary
                var user = DynamicDictionary.Create<JsonPlaceholderUser>(userJson);

                // Access via strongly-typed properties (IntelliSense support!)
                Console.WriteLine($"ID: {user.Id}");
                Console.WriteLine($"Name: {user.Name}");
                Console.WriteLine($"Username: {user.Username}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Phone: {user.Phone}");
                Console.WriteLine($"Website: {user.Website}");
                Console.WriteLine($"City: {user.GetCity()}");
                Console.WriteLine($"Company: {user.GetCompanyName()}");

                // Still supports dynamic access for nested objects
                Console.WriteLine($"\nAddress Street: {user.Address.street}");
                Console.WriteLine($"Company CatchPhrase: {user.Company.catchPhrase}");

                // Direct dictionary access also works
                Console.WriteLine($"\nDirect access to 'name': {user["name"]}");

                Console.WriteLine($"\nToString: {user}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 2: Fetch multiple posts with strongly-typed array.
        /// Demonstrates the generic CreateArray&lt;T&gt; method.
        /// </summary>
        public static async Task Example2_FetchStronglyTypedPosts()
        {
            Console.WriteLine("=== Example 2: Strongly-Typed Post Array ===\n");

            try
            {
                // Fetch posts JSON array from REST API
                var postsJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts?userId=1");

                // Use generic CreateArray<T> method with default serializer - returns T[] instead of dynamic
                var posts = DynamicDictionary.CreateArray<JsonPlaceholderPost>(postsJson);

                Console.WriteLine($"Fetched {posts.Length} posts:\n");

                // Type-safe iteration with IntelliSense support
                foreach (var post in posts)
                {
                    Console.WriteLine($"Post #{post.Id}");
                    Console.WriteLine($"  User: {post.UserId}");
                    Console.WriteLine($"  Title: {post.Title}");
                    Console.WriteLine($"  Body: {post.Body.Substring(0, Math.Min(50, post.Body.Length))}...");
                    Console.WriteLine($"  ToString: {post}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 3: Fetch todos with filtering using strongly-typed model.
        /// Shows practical use case with business logic.
        /// </summary>
        public static async Task Example3_FetchAndFilterTodos()
        {
            Console.WriteLine("=== Example 3: Filter Todos (Strongly-Typed) ===\n");

            try
            {
                // Fetch todos for a specific user
                var todosJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/todos?userId=1");

                // Deserialize to strongly-typed array with default serializer
                var todos = DynamicDictionary.CreateArray<JsonPlaceholderTodo>(todosJson);

                // Business logic with type-safe access
                int completedCount = 0;
                int pendingCount = 0;

                Console.WriteLine("Todo List:");
                foreach (var todo in todos)
                {
                    Console.WriteLine($"  {todo}"); // Uses custom ToString()
                    
                    if (todo.Completed)
                        completedCount++;
                    else
                        pendingCount++;
                }

                Console.WriteLine($"\nSummary:");
                Console.WriteLine($"  Total: {todos.Length}");
                Console.WriteLine($"  Completed: {completedCount}");
                Console.WriteLine($"  Pending: {pendingCount}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 4: Demonstrate type constraint enforcement.
        /// Shows that only DynamicDictionary-derived types can be used.
        /// </summary>
        public static void Example4_TypeConstraintDemo()
        {
            Console.WriteLine("=== Example 4: Type Constraint Demonstration ===\n");

            var json = @"{""id"": 1, ""name"": ""Test""}";

            // ✅ This works - JsonPlaceholderUser inherits from DynamicDictionary (uses default serializer)
            var user = DynamicDictionary.Create<JsonPlaceholderUser>(json);
            Console.WriteLine($"✅ Created {user.GetType().Name}: {user.Name}");

            // ✅ This works - JsonPlaceholderPost inherits from DynamicDictionary (uses default serializer)
            var postJson = @"{""id"": 1, ""userId"": 1, ""title"": ""Test"", ""body"": ""Body""}";
            var post = DynamicDictionary.Create<JsonPlaceholderPost>(postJson);
            Console.WriteLine($"✅ Created {post.GetType().Name}: {post.Title}");

            // ❌ This would NOT compile - string does not inherit from DynamicDictionary
            // var invalid = DynamicDictionary.Create<string>(json); // Compile error!

            // ❌ This would NOT compile - Dictionary<string, object> does not inherit from DynamicDictionary
            // var invalid2 = DynamicDictionary.Create<Dictionary<string, object>>(json); // Compile error!

            Console.WriteLine("\n✅ Type constraints are working correctly!");
            Console.WriteLine("   Only types that inherit from DynamicDictionary can be used.\n");
        }

        /// <summary>
        /// Example 5: Mixing strongly-typed and dynamic access.
        /// Shows the flexibility of the hybrid approach.
        /// </summary>
        public static async Task Example5_HybridAccess()
        {
            Console.WriteLine("=== Example 5: Hybrid Strongly-Typed + Dynamic Access ===\n");

            try
            {
                var userJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/users/1");
                var user = DynamicDictionary.Create<JsonPlaceholderUser>(userJson);

                Console.WriteLine("Strongly-typed access (IntelliSense support):");
                Console.WriteLine($"  user.Name = {user.Name}");
                Console.WriteLine($"  user.Email = {user.Email}");

                Console.WriteLine("\nDynamic access (flexible for nested/unknown properties):");
                Console.WriteLine($"  user.Address.city = {user.Address.city}");
                Console.WriteLine($"  user.Address.geo.lat = {user.Address.geo.lat}");
                Console.WriteLine($"  user.Company.bs = {user.Company.bs}");

                Console.WriteLine("\nDictionary access (direct key lookup):");
                Console.WriteLine($"  user[\"username\"] = {user["username"]}");
                Console.WriteLine($"  user[\"phone\"] = {user["phone"]}");

                Console.WriteLine("\nType-safe method access:");
                Console.WriteLine($"  user.GetValue<string>(\"website\") = {user.GetValue<string>("website")}");
                Console.WriteLine($"  user.GetValue<int>(\"id\") = {user.GetValue<int>("id")}");

                Console.WriteLine("\n✅ All access methods work seamlessly together!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 6: Using different model types in one workflow.
        /// Demonstrates a real-world scenario with multiple API calls.
        /// </summary>
        public static async Task Example6_MultiModelWorkflow()
        {
            Console.WriteLine("=== Example 6: Multi-Model Workflow ===\n");

            try
            {
                var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();

                // Fetch user
                var userJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/users/1");
                var user = DynamicDictionary.Create<JsonPlaceholderUser>(userJson, serializer);
                Console.WriteLine($"User: {user.Name} ({user.Email})");

                // Fetch user's posts
                var postsJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts?userId={user.Id}");
                var posts = DynamicDictionary.CreateArray<JsonPlaceholderPost>(postsJson, serializer);
                Console.WriteLine($"\nUser has {posts.Length} posts:");
                
                // Show first 3 posts
                for (int i = 0; i < Math.Min(3, posts.Length); i++)
                {
                    Console.WriteLine($"  - {posts[i].Title}");
                }

                // Fetch comments for first post
                if (posts.Length > 0)
                {
                    var firstPostId = posts[0].Id;
                    var commentsJson = await _httpClient.GetStringAsync(
                        $"{JsonPlaceholderBaseUrl}/comments?postId={firstPostId}");
                    var comments = DynamicDictionary.CreateArray<JsonPlaceholderComment>(commentsJson, serializer);
                    
                    Console.WriteLine($"\nFirst post has {comments.Length} comments:");
                    foreach (var comment in comments)
                    {
                        Console.WriteLine($"  - {comment.Email}: {comment.Name}");
                    }
                }

                // Fetch user's todos
                var todosJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/todos?userId={user.Id}");
                var todos = DynamicDictionary.CreateArray<JsonPlaceholderTodo>(todosJson, serializer);
                
                int completed = 0;
                foreach (var todo in todos)
                {
                    if (todo.Completed) completed++;
                }
                
                Console.WriteLine($"\nTodo Statistics:");
                Console.WriteLine($"  Total: {todos.Length}");
                Console.WriteLine($"  Completed: {completed} ({(completed * 100.0 / todos.Length):F1}%)");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Runs all strongly-typed model examples.
        /// </summary>
        public static async Task RunAllExamples()
        {
            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("STRONGLY-TYPED DYNAMICDICTIONARY MODEL EXAMPLES");
            Console.WriteLine(new string('=', 70) + "\n");

            await Example1_FetchStronglyTypedUser();
            await Task.Delay(500); // Small delay between API calls

            await Example2_FetchStronglyTypedPosts();
            await Task.Delay(500);

            await Example3_FetchAndFilterTodos();
            await Task.Delay(500);

            Example4_TypeConstraintDemo();

            await Example5_HybridAccess();
            await Task.Delay(500);

            await Example6_MultiModelWorkflow();

            Console.WriteLine(new string('=', 70));
            Console.WriteLine("ALL EXAMPLES COMPLETED");
            Console.WriteLine(new string('=', 70) + "\n");
        }
    }
}

