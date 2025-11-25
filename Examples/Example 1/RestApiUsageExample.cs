using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using OneCiel.Core.Dynamics;

namespace Examples
{
    /// <summary>
    /// Demonstrates practical usage of DynamicDictionary with REST APIs using standardized JSON interfaces.
    /// This example uses the free JSONPlaceholder API (https://jsonplaceholder.typicode.com)
    /// to fetch and process JSON data with clean, testable code.
    /// </summary>
    public sealed class DynamicDictionaryRestApiExample
    {
        private static readonly HttpClient _httpClient = new();
        private const string JsonPlaceholderBaseUrl = "https://jsonplaceholder.typicode.com";

        /// <summary>
        /// Example 1: Fetch and display a single post with unified JSON serializer.
        /// Shows how DynamicDictionary.Create factory method integrates with IDynamicsJsonSerializer.
        /// </summary>
        public static async Task Example1_FetchAndDisplayPost()
        {
            Console.WriteLine("=== Example 1: Fetch Post with Default Serializer ===\n");

            try
            {
                // Fetch post from REST API
                var postJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts/1");

                // Parse using DynamicDictionary.Create with default serializer (Simplest way!)
                // No need to pass serializer - it uses the default SystemTextJsonDynamicsSerializer
                dynamic post = DynamicDictionary.Create(postJson);
                
                // ⚠️ DEBUG TIP: When breakpoint stops here, in debugger watch window use:
                //   post._data["id"]                  - to see the actual value (RECOMMENDED)
                //   ((DynamicDictionary)post)["id"]   - to access via indexer
                //   ((DynamicDictionary)post).GetValue<int>("id")  - type-safe access
                //
                // WHY? 'post.id' won't work in watch because 'dynamic' is evaluated at runtime.

                Console.WriteLine($"Post #{post.id}");
                Console.WriteLine($"Author: User {post.userId}");
                Console.WriteLine($"Title: {post.title}");
                Console.WriteLine($"Content: {post.body}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 2: Using default serializer with Serialize method.
        /// Shows how to serialize back to JSON using the instance method.
        /// </summary>
        public static async Task Example2_CustomJsonOptions()
        {
            Console.WriteLine("=== Example 2: Serialize with Default Serializer ===\n");

            try
            {
                var postJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts/1");
                
                // Create with default serializer
                dynamic post = DynamicDictionary.Create(postJson);
                
                // Add custom field
                post.fetched_at = DateTime.UtcNow.ToString("O");

                // Serialize back to JSON using instance method
                string json = ((DynamicDictionary)post).Serialize();

                Console.WriteLine("Serialized with default options:");
                var preview = json.Length > 200 ? json.Substring(0, 200) + "..." : json;
                Console.WriteLine(preview + "\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 3: Different ways to create and serialize.
        /// Shows the flexibility of using default or custom serializers.
        /// </summary>
        public static async Task Example3_UnifiedSerializer()
        {
            Console.WriteLine("=== Example 3: Default vs Custom Serializer ===\n");

            try
            {
                var postJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts/1");

                // Method 1: Default serializer (simplest)
                dynamic post1 = DynamicDictionary.Create(postJson);
                Console.WriteLine($"Method 1 (Default):  ID = {post1.id}");

                // Method 2: Explicit default serializer
                dynamic post2 = DynamicDictionary.Create(postJson, DynamicDictionaryJsonExtensions.CreateDefaultSerializer());
                Console.WriteLine($"Method 2 (Explicit): ID = {post2.id}");

                // Method 3: Custom options
                var serOptions = new JsonSerializerOptions { WriteIndented = false };
                var customSerializer = new SystemTextJsonDynamicsSerializer(serOptions);
                dynamic post3 = DynamicDictionary.Create(postJson, customSerializer);
                Console.WriteLine($"Method 3 (Custom):   ID = {post3.id}");
                
                // Serialize with custom serializer
                string json = ((DynamicDictionary)post3).Serialize(customSerializer);
                Console.WriteLine($"Serialized length: {json.Length} characters\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 4: Process array with default serializer.
        /// Demonstrates array deserialization using the default serializer.
        /// </summary>
        public static async Task Example4_ProcessArrayWithInterface()
        {
            Console.WriteLine("=== Example 4: Array Processing with Default Serializer ===\n");

            try
            {
                var postsJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts?_limit=3");

                // Parse array using DynamicDictionary.CreateArray with default serializer
                dynamic posts = DynamicDictionary.CreateArray(postsJson);

                Console.WriteLine($"Fetched {posts.Length} posts:\n");

                foreach (dynamic post in posts)
                {
                    // ⚠️ DEBUG TIP: Set breakpoint here
                    // In watch window, use: post._data["id"] to see the ID value
                    Console.WriteLine($"  [{post.id}] {post.title}");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 5: Serialize with custom options using unified serializer.
        /// Shows how to control JSON output formatting with IDynamicsJsonSerializer.
        /// </summary>
        public static async Task Example5_SerializeWithOptions()
        {
            Console.WriteLine("=== Example 5: Serialize with Unified Serializer ===\n");

            try
            {
                var postJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts/1");
                var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();
                dynamic post = serializer.Deserialize(postJson);

                // Add custom properties
                post.fetched_at = DateTime.UtcNow.ToString("O");
                post.source = "jsonplaceholder";

                // Serialize with custom options (camelCase naming)
                var serOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var customSerializer = DynamicDictionaryJsonExtensions.CreateSerializer(serOptions);
                var json = customSerializer.Serialize(post);

                Console.WriteLine("Serialized with camelCase naming:");
                var preview = json.Length > 200 ? json.Substring(0, 200) + "..." : json;
                Console.WriteLine(preview + "\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 6: Using JsonConverter with System.Text.Json.
        /// Shows integration with the standard JsonConverter pattern.
        /// </summary>
        public static async Task Example6_JsonConverterIntegration()
        {
            Console.WriteLine("=== Example 6: JsonConverter Integration ===\n");

            try
            {
                var postJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts/1");

                // Create options with custom converter
                var options = new JsonSerializerOptions
                {
                    Converters = { new DynamicDictionaryJsonConverter() },
                    WriteIndented = false
                };

                // Deserialize using the converter
                var post = JsonSerializer.Deserialize<DynamicDictionary>(postJson, options);

                if (post != null)
                {
                    Console.WriteLine("Deserialized with DynamicDictionaryJsonConverter:");
                    Console.WriteLine($"  Keys: {string.Join(", ", post.Keys.Take(3))}...");
                    Console.WriteLine($"  Total properties: {post.Count}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 7: Set default unified serializer globally.
        /// Shows how to configure default behavior for the entire application.
        /// </summary>
        public static async Task Example7_GlobalConfiguration()
        {
            Console.WriteLine("=== Example 7: Global Configuration with Unified Serializer ===\n");

            try
            {
                // Create unified serializer with custom options
                var serOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                var deserOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var serializer = new SystemTextJsonDynamicsSerializer(serOptions, deserOptions);
                
                // Set as default serializer globally
                DynamicDictionaryJsonExtensions.SetDynamicsJsonSerializer(serializer);

                // Fetch and parse using DynamicDictionary.Create
                var postJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts/1");
                dynamic post = DynamicDictionary.Create(postJson, serializer);

                post.custom_field = "added via unified serializer";

                var serialized = serializer.Serialize(post);

                Console.WriteLine("Using custom configured unified serializer:");
                var preview = serialized.Length > 150 ? serialized.Substring(0, 150) + "..." : serialized;
                Console.WriteLine(preview + "\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Example 8: File operations with unified serializer.
        /// Demonstrates saving and loading with IDynamicsJsonSerializer.
        /// </summary>
        public static async Task Example8_FileOperationsWithOptions()
        {
            Console.WriteLine("=== Example 8: File Operations with Unified Serializer ===\n");

            string tempFile = string.Empty;

            try
            {
                // Create unified serializer with custom options
                var options = new JsonSerializerOptions { WriteIndented = true };
                var serializer = DynamicDictionaryJsonExtensions.CreateSerializer(options, options);

                // Fetch and parse
                var postJson = await _httpClient.GetStringAsync($"{JsonPlaceholderBaseUrl}/posts/1");
                dynamic post = serializer.Deserialize(postJson);

                // Save to temp file using extension methods
                tempFile = Path.Combine(Path.GetTempPath(), $"post_{Guid.NewGuid()}.json");
                await DynamicDictionaryJsonExtensions.ToJsonFileAsync((DynamicDictionary)post, tempFile, serializer);

                Console.WriteLine($"Saved to: {tempFile}");

                // Load from file using extension methods
                dynamic loaded = await DynamicDictionaryJsonExtensions.FromJsonFileAsync(tempFile, serializer);

                Console.WriteLine($"Loaded successfully: {loaded.title}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
            finally
            {
                if (tempFile != null && File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        /// <summary>
        /// Run all examples demonstrating standardized JSON interfaces.
        /// </summary>
        public static async Task RunAllExamples()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   DynamicDictionary - Unified IDynamicsJsonSerializer        ║");
            Console.WriteLine("║   Using JSONPlaceholder API (jsonplaceholder.typicode.com)   ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            await Example1_FetchAndDisplayPost();
            await Example2_CustomJsonOptions();
            await Example3_UnifiedSerializer();
            await Example4_ProcessArrayWithInterface();
            await Example5_SerializeWithOptions();
            await Example6_JsonConverterIntegration();
            await Example7_GlobalConfiguration();
            await Example8_FileOperationsWithOptions();

            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   All examples completed!                                    ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║   Key Benefits of IDynamicsJsonSerializer:                   ║");
            Console.WriteLine("║   ✓ Unified interface for serialization & deserialization   ║");
            Console.WriteLine("║   ✓ Clean separation of concerns                            ║");
            Console.WriteLine("║   ✓ Testable and mockable JSON operations                   ║");
            Console.WriteLine("║   ✓ Flexible custom implementation support                  ║");
            Console.WriteLine("║   ✓ No code duplication                                     ║");
            Console.WriteLine("║   ✓ Easy to extend and maintain                             ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║   💡 DEBUG TIP: Use post._data[\"id\"] in watch window       ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        }
    }

    /// <summary>
    /// Example usage in a console application.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // Example 0: Basic DynamicDictionary usage (no JSON required)
            // This demonstrates pure DynamicDictionary functionality
            BasicDynamicDictionaryExample.RunAllExamples();

            // Initialize the default JSON serializer for DynamicDictionary
            // This allows using DynamicDictionary.Create(json) without explicitly passing a serializer
            DynamicDictionary.SetDefaultSerializer(DynamicDictionaryJsonExtensions.CreateDefaultSerializer());
            
            Console.WriteLine("✓ Default JSON serializer initialized");
            Console.WriteLine("  You can now use DynamicDictionary.Create(json) without passing a serializer\n");

            // Run the strongly-typed model examples using DynamicDictionary.Create<T>
            await StronglyTypedModelExample.RunAllExamples();

            // Uncomment to run other examples:
            // await QuickTest.TestStronglyTypedModels();
            // await DynamicDictionaryRestApiExample.RunAllExamples();
        }
    }
}
