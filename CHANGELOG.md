# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - 2024-01-01

### Added

#### OneCiel.Core.Dynamics (1.0.0)
- Initial release of core DynamicDictionary class
- Dynamic object access with DynamicObject implementation
- Nested property navigation using dot notation (e.g., "user.profile.name")
- Array element access using bracket notation (e.g., "items[0]", "users[1].address")
- Case-insensitive key lookup with StringComparer.OrdinalIgnoreCase
- Generic `GetValue<T>()` method with automatic type conversion
- Support for numeric type conversion (byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal)
- Enum type conversion from strings and numeric values
- Nullable type support
- Dictionary cloning with shallow and deep copy options
- Dictionary merging with overwrite control and deep merge support
- `RemoveWhere()` method for conditional item removal
- Full implementation of IDictionary<string, object> interface
- Full implementation of IEnumerable<KeyValuePair<string, object>> interface
- Support for IReadOnlyDictionary<string, object> interface
- Delegate storage and invocation support
- Comprehensive XML documentation
- Compatible with .NET Standard 2.1

#### OneCiel.Core.Dynamics.JsonExtension (1.0.0)
- JSON string serialization with `ToJson()` extension method
- JSON string deserialization with `FromJson()` static method
- Asynchronous file operations: `ToJsonFileAsync()` and `FromJsonFileAsync()`
- Synchronous file operations: `ToJsonFile()` and `FromJsonFile()`
- Custom `DynamicDictionaryJsonConverter` for System.Text.Json
- Automatic JsonElement conversion to appropriate .NET types
- Support for nested objects, arrays, and primitive types
- Pretty-printing with optional indentation
- Case-insensitive JSON property parsing
- Support for JSON comments and trailing commas when parsing
- Proper error handling with descriptive exceptions
- Compatible with .NET 8.0 and .NET 9.0

## Version Support

| Version | Status | .NET Standard | .NET 8 | .NET 9 |
|---------|--------|:-------------:|:------:|:------:|
| 1.0.0   | Release | 2.1 ✓ | ✓ | ✓ |

## Future Roadmap

- [ ] Unit tests and test coverage
- [ ] Performance optimization benchmarks
- [ ] Extended serialization format support
- [ ] Advanced querying capabilities
- [ ] Validation framework integration

