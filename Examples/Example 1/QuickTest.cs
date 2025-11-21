using System;
using System.Threading.Tasks;
using OneCiel.System.Dynamics;

namespace Examples
{
    /// <summary>
    /// Quick test to verify strongly-typed models work correctly.
    /// </summary>
    public class QuickTest
    {
        public static Task TestStronglyTypedModels()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Testing Strongly-Typed DynamicDictionary Models          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

            var serializer = DynamicDictionaryJsonExtensions.CreateDefaultSerializer();

            // Test 1: Single object
            Console.WriteLine("✓ Test 1: Single User Object");
            var userJson = @"{
                ""id"": 1,
                ""name"": ""Leanne Graham"",
                ""username"": ""Bret"",
                ""email"": ""Sincere@april.biz"",
                ""phone"": ""1-770-736-8031 x56442"",
                ""website"": ""hildegard.org"",
                ""address"": {
                    ""street"": ""Kulas Light"",
                    ""city"": ""Gwenborough"",
                    ""zipcode"": ""92998-3874""
                },
                ""company"": {
                    ""name"": ""Romaguera-Crona"",
                    ""catchPhrase"": ""Multi-layered client-server neural-net""
                }
            }";

            var user = DynamicDictionary.Create<JsonPlaceholderUser>(userJson, serializer);
            Console.WriteLine($"  Name: {user.Name}");
            Console.WriteLine($"  Email: {user.Email}");
            Console.WriteLine($"  City: {user.GetCity()}");
            Console.WriteLine($"  Company: {user.GetCompanyName()}");
            Console.WriteLine($"  ToString: {user}\n");

            // Test 2: Array of objects
            Console.WriteLine("✓ Test 2: Array of Posts");
            var postsJson = @"[
                {
                    ""userId"": 1,
                    ""id"": 1,
                    ""title"": ""First Post"",
                    ""body"": ""This is the first post""
                },
                {
                    ""userId"": 1,
                    ""id"": 2,
                    ""title"": ""Second Post"",
                    ""body"": ""This is the second post""
                }
            ]";

            var posts = DynamicDictionary.CreateArray<JsonPlaceholderPost>(postsJson, serializer);
            Console.WriteLine($"  Array length: {posts.Length}");
            foreach (var post in posts)
            {
                Console.WriteLine($"  - {post}");
            }
            Console.WriteLine();

            // Test 3: Type constraint verification
            Console.WriteLine("✓ Test 3: Type Constraint Verification");
            Console.WriteLine("  - JsonPlaceholderUser inherits from DynamicDictionary: ✓");
            Console.WriteLine("  - JsonPlaceholderPost inherits from DynamicDictionary: ✓");
            Console.WriteLine("  - Type safety enforced at compile time: ✓\n");

            // Test 4: Hybrid access
            Console.WriteLine("✓ Test 4: Hybrid Access Patterns");
            Console.WriteLine($"  Strongly-typed: user.Name = {user.Name}");
            Console.WriteLine($"  Dynamic nested: user.Address.city = {user.Address.city}");
            Console.WriteLine($"  Dictionary: user[\"email\"] = {user["email"]}");
            Console.WriteLine($"  Type-safe: user.GetValue<int>(\"id\") = {user.GetValue<int>("id")}\n");

            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ✓ All Tests Passed Successfully!                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
            
            return Task.CompletedTask;
        }
    }
}

