using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneCiel.System.Dynamics;

namespace Examples
{
    /// <summary>
    /// Demonstrates basic usage of DynamicDictionary without JSON.
    /// Shows how to use DynamicDictionary as a flexible, dynamic data structure.
    /// This is the foundation example that shows pure DynamicDictionary functionality.
    /// </summary>
    public sealed class BasicDynamicDictionaryExample
    {
        /// <summary>
        /// Example 1: Create and use DynamicDictionary with dynamic properties.
        /// Shows the most basic usage without any JSON serialization.
        /// </summary>
        public static void Example1_BasicUsage()
        {
            Console.WriteLine("=== Example 1: Basic DynamicDictionary Usage ===\n");

            // Create an empty DynamicDictionary
            dynamic person = DynamicDictionary.Create();

            // Add properties dynamically
            person.Name = "John Doe";
            person.Age = 30;
            person.Email = "john@example.com";
            person.IsActive = true;

            // Access properties
            Console.WriteLine($"Name: {person.Name}");
            Console.WriteLine($"Age: {person.Age}");
            Console.WriteLine($"Email: {person.Email}");
            Console.WriteLine($"Is Active: {person.IsActive}\n");
        }

        /// <summary>
        /// Example 2: Using indexer and GetValue methods.
        /// Shows different ways to access values in DynamicDictionary.
        /// </summary>
        public static void Example2_AccessMethods()
        {
            Console.WriteLine("=== Example 2: Different Access Methods ===\n");

            // Create with initial data
            var data = new Dictionary<string, object>
            {
                { "id", 1 },
                { "name", "Jane Smith" },
                { "score", 95.5 },
                { "tags", new[] { "developer", "designer", "manager" } }
            };
            dynamic user = DynamicDictionary.Create(data);

            // Method 1: Dynamic property access
            Console.WriteLine($"Dynamic: {user.name}");

            // Method 2: Indexer access
            Console.WriteLine($"Indexer: {user["name"]}");

            // Method 3: GetValue with type safety
            int id = user.GetValue<int>("id");
            double score = user.GetValue<double>("score");
            Console.WriteLine($"GetValue<int>: {id}");
            Console.WriteLine($"GetValue<double>: {score}");

            // Method 4: GetValue with default
            string missing = user.GetValue<string>("missing", "N/A");
            Console.WriteLine($"GetValue with default: {missing}\n");
        }

        /// <summary>
        /// Example 3: Nested objects and complex structures.
        /// Shows how to create and access nested DynamicDictionary objects.
        /// </summary>
        public static void Example3_NestedObjects()
        {
            Console.WriteLine("=== Example 3: Nested Objects ===\n");

            dynamic user = DynamicDictionary.Create();
            user.Name = "Alice";
            user.Age = 28;

            // Create nested address object
            dynamic address = DynamicDictionary.Create();
            address.Street = "123 Main St";
            address.City = "New York";
            address.ZipCode = "10001";
            user.Address = address;

            // Create nested company object
            dynamic company = DynamicDictionary.Create();
            company.Name = "Tech Corp";
            company.Department = "Engineering";
            user.Company = company;

            // Access nested properties
            Console.WriteLine($"User: {user.Name}");
            Console.WriteLine($"Address: {user.Address.Street}, {user.Address.City}");
            Console.WriteLine($"Company: {user.Company.Name} - {user.Company.Department}");

            // Using nested path notation
            Console.WriteLine($"City (path): {user["Address.City"]}");
            Console.WriteLine($"Department (path): {user["Company.Department"]}\n");
        }

        /// <summary>
        /// Example 4: Working with arrays and collections.
        /// Shows how to store and access arrays in DynamicDictionary.
        /// </summary>
        public static void Example4_ArraysAndCollections()
        {
            Console.WriteLine("=== Example 4: Arrays and Collections ===\n");

            dynamic product = DynamicDictionary.Create();
            product.Name = "Laptop";
            product.Price = 1299.99;
            product.Features = new[] { "16GB RAM", "512GB SSD", "4K Display" };
            product.Tags = new List<string> { "electronics", "computers", "gaming" };

            // Access array elements
            Console.WriteLine($"Product: {product.Name}");
            Console.WriteLine($"Price: ${product.Price}");

            // Access array via indexer
            Console.WriteLine($"First Feature: {product["Features[0]"]}");
            Console.WriteLine($"Second Feature: {product["Features[1]"]}");

            // Iterate through features
            if (product.Features is Array features)
            {
                Console.WriteLine("Features:");
                foreach (var feature in features)
                {
                    Console.WriteLine($"  - {feature}");
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Example 5: Dictionary operations.
        /// Shows standard dictionary operations like iteration, checking keys, etc.
        /// </summary>
        public static void Example5_DictionaryOperations()
        {
            Console.WriteLine("=== Example 5: Dictionary Operations ===\n");

            dynamic config = DynamicDictionary.Create();
            config["app.name"] = "MyApp";
            config["app.version"] = "1.0.0";
            config["database.host"] = "localhost";
            config["database.port"] = 5432;
            config["features.enabled"] = true;

            // Check if key exists
            Console.WriteLine($"Has 'app.name': {config.ContainsKey("app.name")}");
            Console.WriteLine($"Has 'missing.key': {config.ContainsKey("missing.key")}");

            // Get all keys
            Console.WriteLine("\nAll keys:");
            foreach (var key in config.Keys)
            {
                Console.WriteLine($"  - {key}");
            }

            // Iterate key-value pairs
            Console.WriteLine("\nAll key-value pairs:");
            foreach (var kvp in config)
            {
                Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
            }

            // Count
            Console.WriteLine($"\nTotal items: {config.Count}\n");
        }

        /// <summary>
        /// Example 6: Type conversion and GetValue.
        /// Shows how DynamicDictionary handles type conversions automatically.
        /// </summary>
        public static void Example6_TypeConversion()
        {
            Console.WriteLine("=== Example 6: Type Conversion ===\n");

            dynamic data = DynamicDictionary.Create();
            data["number"] = "42";        // String
            data["decimal"] = 3.14;       // Double
            data["boolean"] = "true";     // String
            data["integer"] = 100;        // Int

            // Automatic type conversion
            int number = data.GetValue<int>("number");        // String to int
            decimal dec = data.GetValue<decimal>("decimal");  // Double to decimal
            bool flag = data.GetValue<bool>("boolean");       // String to bool
            long bigInt = data.GetValue<long>("integer");     // Int to long

            Console.WriteLine($"String '42' -> int: {number}");
            Console.WriteLine($"Double 3.14 -> decimal: {dec}");
            Console.WriteLine($"String 'true' -> bool: {flag}");
            Console.WriteLine($"Int 100 -> long: {bigInt}");

            // With default values
            int missing = data.GetValue<int>("nonexistent", -1);
            Console.WriteLine($"Missing key with default: {missing}\n");
        }

        /// <summary>
        /// Example 7: Creating from existing dictionary.
        /// Shows how to initialize DynamicDictionary from existing data structures.
        /// </summary>
        public static void Example7_FromDictionary()
        {
            Console.WriteLine("=== Example 7: Create from Dictionary ===\n");

            // From Dictionary<string, object>
            var dict = new Dictionary<string, object>
            {
                { "id", 1 },
                { "name", "Bob" },
                { "active", true }
            };
            dynamic user1 = DynamicDictionary.Create(dict);
            Console.WriteLine($"From Dictionary: {user1.name} (ID: {user1.id})");

            // From KeyValuePair collection
            var items = new[]
            {
                new KeyValuePair<string, object>("product", "Widget"),
                new KeyValuePair<string, object>("price", 29.99),
                new KeyValuePair<string, object>("inStock", true)
            };
            dynamic product = DynamicDictionary.Create(items);
            Console.WriteLine($"From KeyValuePairs: {product.product} - ${product.price}\n");
        }

        /// <summary>
        /// Example 8: Modifying and removing properties.
        /// Shows how to update and remove properties dynamically.
        /// </summary>
        public static void Example8_ModifyAndRemove()
        {
            Console.WriteLine("=== Example 8: Modify and Remove ===\n");

            dynamic item = DynamicDictionary.Create();
            item.Name = "Original Name";
            item.Value = 100;
            item.Category = "A";

            Console.WriteLine("Initial state:");
            Console.WriteLine($"  Name: {item.Name}, Value: {item.Value}, Category: {item.Category}");

            // Modify properties
            item.Name = "Updated Name";
            item.Value = 200;
            item["Category"] = "B";

            Console.WriteLine("\nAfter modification:");
            Console.WriteLine($"  Name: {item.Name}, Value: {item.Value}, Category: {item.Category}");

            // Add new property
            item.Status = "Active";

            // Remove property
            item.Remove("Category");

            Console.WriteLine("\nAfter removal:");
            Console.WriteLine($"  Name: {item.Name}, Value: {item.Value}, Status: {item.Status}");
            Console.WriteLine($"  Has Category: {item.ContainsKey("Category")}\n");
        }

        /// <summary>
        /// Example 9: Clone and merge operations.
        /// Shows how to copy and merge DynamicDictionary instances.
        /// </summary>
        public static void Example9_CloneAndMerge()
        {
            Console.WriteLine("=== Example 9: Clone and Merge ===\n");

            // Original
            dynamic original = DynamicDictionary.Create();
            original.Name = "Original";
            original.Id = 1;

            // Shallow clone
            dynamic cloned = original.Clone();
            cloned.Name = "Cloned";
            Console.WriteLine($"Original: {original.Name}, Cloned: {cloned.Name}");

            // Merge
            dynamic source = DynamicDictionary.Create();
            source.Name = "Merged";
            source.Age = 25;
            source.City = "Seattle";

            dynamic target = DynamicDictionary.Create();
            target.Name = "Target";
            target.Id = 2;

            target.Merge(source);
            Console.WriteLine($"\nAfter merge:");
            Console.WriteLine($"  Name: {target.Name} (overwritten)");
            Console.WriteLine($"  Id: {target.Id} (preserved)");
            Console.WriteLine($"  Age: {target.Age} (added)");
            Console.WriteLine($"  City: {target.City} (added)\n");
        }

        /// <summary>
        /// Example 10: ToString and utility methods.
        /// Shows utility methods for debugging and display.
        /// </summary>
        public static void Example10_UtilityMethods()
        {
            Console.WriteLine("=== Example 10: Utility Methods ===\n");

            dynamic data = DynamicDictionary.Create();
            data.FirstName = "John";
            data.LastName = "Doe";
            data.Age = 30;
            data.Email = "john@example.com";

            // ToString
            Console.WriteLine("ToString():");
            Console.WriteLine(data.ToString());

            // ToString with custom separator
            Console.WriteLine("\nToString with custom separator:");
            Console.WriteLine(data.ToString(" = ", false));

            // GetFirstValue
            var firstValue = data.GetFirstValue();
            Console.WriteLine($"\nFirst value: {firstValue}");

            // GetFirstValue with type
            string firstName = data.GetFirstValue<string>();
            Console.WriteLine($"First value as string: {firstName}\n");
        }

        /// <summary>
        /// Run all basic DynamicDictionary examples.
        /// </summary>
        public static void RunAllExamples()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   DynamicDictionary - Basic Usage Examples                  ║");
            Console.WriteLine("║   Pure DynamicDictionary without JSON                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            Example1_BasicUsage();
            Example2_AccessMethods();
            Example3_NestedObjects();
            Example4_ArraysAndCollections();
            Example5_DictionaryOperations();
            Example6_TypeConversion();
            Example7_FromDictionary();
            Example8_ModifyAndRemove();
            Example9_CloneAndMerge();
            Example10_UtilityMethods();

            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   All basic examples completed!                              ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║   Key Features Demonstrated:                                ║");
            Console.WriteLine("║   ✓ Dynamic property access                                 ║");
            Console.WriteLine("║   ✓ Multiple access methods (dynamic, indexer, GetValue)     ║");
            Console.WriteLine("║   ✓ Nested object support                                    ║");
            Console.WriteLine("║   ✓ Array and collection handling                           ║");
            Console.WriteLine("║   ✓ Type conversion                                          ║");
            Console.WriteLine("║   ✓ Dictionary operations                                    ║");
            Console.WriteLine("║   ✓ Clone and merge                                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");
        }
    }
}

