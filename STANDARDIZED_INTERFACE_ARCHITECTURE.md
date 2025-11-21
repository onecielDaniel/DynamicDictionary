# 표준화된 인터페이스 아키텍처 구현 완료

## 📋 개요

**요청사항**: "JsonSerializer.Deserialize를 표준화된 인터페이스로 만들어서 DynamicDictionary의 생성자나 Create 함수에 제공해서 사용할 수 있게 하는 방법으로 변경"

**완료일자**: 2025-11-19

---

## 🎯 목표

직접적인 `JsonSerializer.Deserialize` 호출을 표준화된 `IJsonDeserializer` 인터페이스로 추상화하여:
1. 깔끔한 의존성 주입(DI) 패턴 구현
2. 테스트 가능성 향상 (Mock 가능)
3. 확장 가능한 아키텍처
4. 명확한 관심사의 분리

---

## 🏗️ 아키텍처 개요

### Before (직접 호출)
```csharp
// Extension method에서 JsonSerializer를 직접 호출
public static DynamicDictionary ToDynamicDictionary(this string json)
{
    return JsonSerializer.Deserialize<DynamicDictionary>(json, options);
}
```

**문제점:**
- ❌ JsonSerializer에 강하게 결합
- ❌ 테스트하기 어려움
- ❌ 다른 JSON 라이브러리로 교체 불가능
- ❌ 의존성 주입 불가능

### After (인터페이스 기반)
```csharp
// 표준화된 인터페이스 사용
public static DynamicDictionary ToDynamicDictionary(this string json)
{
    return _defaultDeserializer.Deserialize(json);
}

// 또는 커스텀 deserializer 주입
public static DynamicDictionary ToDynamicDictionary(this string json, IJsonDeserializer deserializer)
{
    return deserializer.Deserialize(json);
}
```

**장점:**
- ✅ 인터페이스로 추상화
- ✅ 쉬운 테스트 (Mock 가능)
- ✅ 다양한 구현체 사용 가능
- ✅ 의존성 주입 지원
- ✅ SOLID 원칙 준수

---

## 📦 구현된 컴포넌트

### 1. IJsonDeserializer 인터페이스 (Core)

**위치**: `OneCiel.System.Dynamics/JsonSerializationInterfaces.cs`

```csharp
public interface IJsonDeserializer
{
    /// <summary>
    /// Deserializes a JSON string to a DynamicDictionary.
    /// </summary>
    DynamicDictionary Deserialize(string json);

    /// <summary>
    /// Deserializes a JSON array string to an array of DynamicDictionary objects.
    /// </summary>
    DynamicDictionary[] DeserializeArray(string json);
}
```

**특징:**
- 표준화된 계약(Contract) 정의
- 간단하고 명확한 API
- 구현체에 독립적

### 2. SystemTextJsonDeserializer (Implementation)

**위치**: `OneCiel.System.Dynamics.JsonExtension/SystemTextJsonImplementations.cs`

```csharp
public sealed class SystemTextJsonDeserializer : IJsonDeserializer
{
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonDeserializer(JsonSerializerOptions? options = null)
    {
        _options = options != null 
            ? EnsureConverterInOptions(options) 
            : GetDefaultOptions();
    }

    public DynamicDictionary Deserialize(string json)
    {
        return JsonSerializer.Deserialize<DynamicDictionary>(json, _options)
            ?? throw new InvalidOperationException("Failed to deserialize JSON string.");
    }

    public DynamicDictionary[] DeserializeArray(string json)
    {
        return JsonSerializer.Deserialize<DynamicDictionary[]>(json, _options)
            ?? throw new InvalidOperationException("Failed to deserialize JSON array.");
    }
}
```

**특징:**
- `IJsonDeserializer` 구현
- `System.Text.Json` 기반
- `JsonSerializerOptions` 설정 가능
- `DynamicDictionaryJsonConverter` 자동 추가
- 에러 처리 내장

### 3. DynamicDictionary.Create with Interface

**위치**: `OneCiel.System.Dynamics/DynamicDictionary.cs`

```csharp
/// <summary>
/// Creates a DynamicDictionary from JSON using IJsonDeserializer.
/// </summary>
public static dynamic Create(string json, IJsonDeserializer deserializer)
{
    if (json == null)
        throw new ArgumentNullException(nameof(json));
    if (string.IsNullOrWhiteSpace(json))
        throw new ArgumentException("JSON string cannot be empty", nameof(json));
    if (deserializer == null)
        throw new ArgumentNullException(nameof(deserializer));

    return deserializer.Deserialize(json);
}

/// <summary>
/// Creates an array from JSON using IJsonDeserializer.
/// </summary>
public static dynamic CreateArray(string json, IJsonDeserializer deserializer)
{
    if (json == null)
        throw new ArgumentNullException(nameof(json));
    if (string.IsNullOrWhiteSpace(json))
        throw new ArgumentException("JSON string cannot be empty", nameof(json));
    if (deserializer == null)
        throw new ArgumentNullException(nameof(deserializer));

    return deserializer.DeserializeArray(json);
}
```

**특징:**
- 인터페이스 기반 팩토리 메서드
- 의존성 주입 패턴
- 명확한 null 체크
- 완전한 XML 문서화

### 4. Extension Methods with DI Support

**위치**: `OneCiel.System.Dynamics.JsonExtension/DynamicDictionaryJsonExtensions.cs`

```csharp
// Default deserializer 설정
private static IJsonDeserializer _defaultDeserializer = new SystemTextJsonDeserializer();

// Deserializer 교체 가능
public static void SetJsonDeserializer(IJsonDeserializer deserializer)
{
    _defaultDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
}

// Extension method (default deserializer 사용)
public static DynamicDictionary ToDynamicDictionary(this string json)
{
    return _defaultDeserializer.Deserialize(json);
}

// Extension method (custom deserializer 주입)
public static DynamicDictionary ToDynamicDictionary(this string json, IJsonDeserializer deserializer)
{
    return deserializer.Deserialize(json);
}

// Convenience overload (JsonSerializerOptions)
public static DynamicDictionary ToDynamicDictionary(this string json, JsonSerializerOptions options)
{
    var deserializer = new SystemTextJsonDeserializer(options);
    return deserializer.Deserialize(json);
}
```

**특징:**
- 다양한 오버로드 제공
- Default deserializer 전역 설정 가능
- Fluent API 유지
- 옵션 편의 메서드 제공

---

## 💡 사용 예제

### 1. 기본 사용 (Default Deserializer)

```csharp
// Extension method - 가장 간단
var data = json.ToDynamicDictionary();
var array = json.ToDynamicArray();
```

### 2. 인터페이스 기반 DI (권장)

```csharp
// Create 메서드에 인터페이스 주입
var deserializer = new SystemTextJsonDeserializer();
dynamic user = DynamicDictionary.Create(json, deserializer);
dynamic users = DynamicDictionary.CreateArray(json, deserializer);
```

### 3. 커스텀 옵션

```csharp
// 방법 1: 옵션으로 생성
var options = new JsonSerializerOptions 
{ 
    PropertyNameCaseInsensitive = false 
};
var deserializer = new SystemTextJsonDeserializer(options);
dynamic data = DynamicDictionary.Create(json, deserializer);

// 방법 2: Convenience overload
dynamic data = json.ToDynamicDictionary(options);
```

### 4. 전역 Deserializer 설정

```csharp
// 앱 시작 시 전역 deserializer 설정
var customDeserializer = new SystemTextJsonDeserializer(myOptions);
DynamicDictionaryJsonExtensions.SetJsonDeserializer(customDeserializer);

// 이후 모든 호출에서 커스텀 deserializer 사용
var data = json.ToDynamicDictionary();  // customDeserializer 사용
```

### 5. Extension Method에 Deserializer 주입

```csharp
// Fluent API with custom deserializer
var deserializer = new SystemTextJsonDeserializer(options);
var data = json.ToDynamicDictionary(deserializer);
var array = json.ToDynamicArray(deserializer);
```

### 6. 테스트를 위한 Mock

```csharp
// Unit test with mock
public class MockDeserializer : IJsonDeserializer
{
    public DynamicDictionary Deserialize(string json)
    {
        return new DynamicDictionary { { "test", true } };
    }

    public DynamicDictionary[] DeserializeArray(string json)
    {
        return new[] { new DynamicDictionary { { "test", true } } };
    }
}

// Test
var mockDeserializer = new MockDeserializer();
dynamic data = DynamicDictionary.Create(json, mockDeserializer);
Assert.IsTrue(data.test);
```

---

## 🎨 아키텍처 다이어그램

```
┌─────────────────────────────────────────────────────────────┐
│                    User Code                                 │
│  - json.ToDynamicDictionary()                                │
│  - DynamicDictionary.Create(json, deserializer)              │
└───────────────────┬─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────┐
│        DynamicDictionaryJsonExtensions (Extension Layer)     │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  _defaultDeserializer: IJsonDeserializer            │    │
│  │  SetJsonDeserializer(IJsonDeserializer)             │    │
│  │  ToDynamicDictionary(this string)                   │    │
│  │  ToDynamicDictionary(this string, IJsonDeserializer)│    │
│  │  ToDynamicDictionary(this string, Options)          │    │
│  └─────────────────────────────────────────────────────┘    │
└───────────────────┬─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────┐
│            DynamicDictionary (Core Layer)                    │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  Create(string, IJsonDeserializer)                  │    │
│  │  CreateArray(string, IJsonDeserializer)             │    │
│  └─────────────────────────────────────────────────────┘    │
└───────────────────┬─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────┐
│        IJsonDeserializer (Interface - Core)                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  + Deserialize(string): DynamicDictionary           │    │
│  │  + DeserializeArray(string): DynamicDictionary[]    │    │
│  └─────────────────────────────────────────────────────┘    │
└───────────────────┬─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────┐
│   SystemTextJsonDeserializer (Implementation - Extension)    │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  - _options: JsonSerializerOptions                  │    │
│  │  + Deserialize(string): DynamicDictionary           │    │
│  │  + DeserializeArray(string): DynamicDictionary[]    │    │
│  └─────────────────────────────────────────────────────┘    │
└───────────────────┬─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────┐
│          System.Text.Json (External Library)                 │
│  - JsonSerializer.Deserialize<T>()                           │
└─────────────────────────────────────────────────────────────┘
```

---

## 📊 비교표

| 측면 | Before | After |
|------|--------|-------|
| **결합도** | 강결합 (JsonSerializer) | 약결합 (IJsonDeserializer) |
| **테스트성** | 어려움 | 쉬움 (Mock 가능) |
| **확장성** | 제한적 | 높음 (구현체 교체) |
| **DI 지원** | ❌ | ✅ |
| **관심사 분리** | 낮음 | 높음 |
| **SOLID 원칙** | 일부 위반 | 준수 |

---

## 🎯 SOLID 원칙 적용

### 1. Single Responsibility Principle (SRP)
- ✅ `IJsonDeserializer`: 역직렬화만 담당
- ✅ `SystemTextJsonDeserializer`: System.Text.Json 구현만 담당
- ✅ `DynamicDictionary`: 동적 dictionary 기능만 담당

### 2. Open/Closed Principle (OCP)
- ✅ 확장에는 열려있음: 새로운 `IJsonDeserializer` 구현 추가 가능
- ✅ 수정에는 닫혀있음: 기존 코드 수정 없이 새 구현 사용

### 3. Liskov Substitution Principle (LSP)
- ✅ 모든 `IJsonDeserializer` 구현체는 서로 교체 가능

### 4. Interface Segregation Principle (ISP)
- ✅ `IJsonDeserializer`는 필수 메서드만 포함
- ✅ `IJsonSerializer`와 분리

### 5. Dependency Inversion Principle (DIP)
- ✅ 고수준 모듈(DynamicDictionary)이 추상화(IJsonDeserializer)에 의존
- ✅ 저수준 모듈(SystemTextJsonDeserializer)도 추상화에 의존

---

## 🔄 Migration Guide

### 기존 코드는 그대로 작동

```csharp
// 기존 코드 - 여전히 작동함
var data = json.ToDynamicDictionary();
var array = json.ToDynamicArray();
```

### 새로운 인터페이스 기반 코드

```csharp
// 인터페이스 기반 - 권장
var deserializer = new SystemTextJsonDeserializer();
var data = DynamicDictionary.Create(json, deserializer);

// 또는 extension method에 주입
var data = json.ToDynamicDictionary(deserializer);
```

### 커스텀 Deserializer 구현

```csharp
public class NewtonsoftJsonDeserializer : IJsonDeserializer
{
    public DynamicDictionary Deserialize(string json)
    {
        var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
        var dict = new DynamicDictionary();
        // JObject to DynamicDictionary 변환 로직
        return dict;
    }

    public DynamicDictionary[] DeserializeArray(string json)
    {
        var jArray = Newtonsoft.Json.Linq.JArray.Parse(json);
        // JArray to DynamicDictionary[] 변환 로직
        return result;
    }
}

// 사용
var deserializer = new NewtonsoftJsonDeserializer();
DynamicDictionaryJsonExtensions.SetJsonDeserializer(deserializer);
```

---

## ✅ 빌드 및 테스트 결과

### 빌드
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 테스트 실행
```
╔════════════════════════════════════════════════════════════╗
║   All examples completed!                                   ║
║                                                            ║
║   Key Benefits of Standardized Interfaces:                 ║
║   ✓ Clean separation of concerns                           ║
║   ✓ Testable and mockable JSON operations                  ║
║   ✓ Flexible custom implementation support                 ║
║   ✓ No code duplication                                    ║
║   ✓ Easy to extend and maintain                            ║
╚════════════════════════════════════════════════════════════╝
```

### Example 3 출력
```
=== Example 3: Standardized IJsonDeserializer Interface ===

Method 1 (Interface): ID = 1
Method 2 (Fluent):    ID = 1
Method 3 (Options):   ID = 1
```

---

## 📚 API Reference

### IJsonDeserializer Interface

```csharp
public interface IJsonDeserializer
{
    DynamicDictionary Deserialize(string json);
    DynamicDictionary[] DeserializeArray(string json);
}
```

### SystemTextJsonDeserializer Class

```csharp
public sealed class SystemTextJsonDeserializer : IJsonDeserializer
{
    public SystemTextJsonDeserializer(JsonSerializerOptions? options = null);
    public DynamicDictionary Deserialize(string json);
    public DynamicDictionary[] DeserializeArray(string json);
}
```

### DynamicDictionary Factory Methods

```csharp
public static class DynamicDictionary
{
    public static dynamic Create(string json, IJsonDeserializer deserializer);
    public static dynamic CreateArray(string json, IJsonDeserializer deserializer);
}
```

### Extension Methods

```csharp
public static class DynamicDictionaryJsonExtensions
{
    // Global configuration
    public static void SetJsonDeserializer(IJsonDeserializer deserializer);
    
    // Extension methods
    public static DynamicDictionary ToDynamicDictionary(this string json);
    public static DynamicDictionary ToDynamicDictionary(this string json, IJsonDeserializer deserializer);
    public static DynamicDictionary ToDynamicDictionary(this string json, JsonSerializerOptions options);
    
    public static DynamicDictionary[] ToDynamicArray(this string json);
    public static DynamicDictionary[] ToDynamicArray(this string json, IJsonDeserializer deserializer);
    public static DynamicDictionary[] ToDynamicArray(this string json, JsonSerializerOptions options);
}
```

---

## 🎯 장점 요약

### 1. 깨끗한 아키텍처
- ✅ 명확한 계층 분리
- ✅ 인터페이스 기반 설계
- ✅ 의존성 역전 원칙

### 2. 테스트 용이성
- ✅ Mock 객체 사용 가능
- ✅ 단위 테스트 격리
- ✅ 통합 테스트 간소화

### 3. 확장성
- ✅ 새로운 deserializer 추가 용이
- ✅ 기존 코드 수정 불필요
- ✅ 다양한 JSON 라이브러리 지원 가능

### 4. 유지보수성
- ✅ 코드 가독성 향상
- ✅ 버그 수정 용이
- ✅ 기능 추가 간편

### 5. 호환성
- ✅ 기존 API 완전 호환
- ✅ Breaking change 없음
- ✅ 점진적 마이그레이션 가능

---

## 📂 변경된 파일

1. ✅ **OneCiel.System.Dynamics.JsonExtension/SystemTextJsonImplementations.cs**
   - `SystemTextJsonDeserializer` 클래스 추가

2. ✅ **OneCiel.System.Dynamics/DynamicDictionary.cs**
   - `Create(string, IJsonDeserializer)` 추가
   - `CreateArray(string, IJsonDeserializer)` 추가

3. ✅ **OneCiel.System.Dynamics.JsonExtension/DynamicDictionaryJsonExtensions.cs**
   - `_defaultDeserializer` 필드 추가
   - `SetJsonDeserializer()` 메서드 추가
   - 모든 extension method를 인터페이스 기반으로 리팩토링
   - Deserializer 오버로드 추가

4. ✅ **Examples/RestApiUsageExample.cs**
   - Example 3을 인터페이스 기반 예제로 업데이트

5. ✅ **STANDARDIZED_INTERFACE_ARCHITECTURE.md**
   - 이 문서

---

## 🎉 결론

**✅ 표준화된 인터페이스 아키텍처 구현 완료!**

### 달성한 목표
1. ✅ `JsonSerializer.Deserialize` 직접 호출 제거
2. ✅ `IJsonDeserializer` 인터페이스로 추상화
3. ✅ `DynamicDictionary.Create`에 인터페이스 주입 지원
4. ✅ Extension method에서 인터페이스 사용
5. ✅ 전역 deserializer 설정 기능
6. ✅ SOLID 원칙 준수
7. ✅ 완전한 하위 호환성
8. ✅ 빌드 성공 (0 warnings, 0 errors)
9. ✅ 모든 예제 실행 성공

### 핵심 가치
```csharp
// 💎 Clean Architecture
var deserializer = new SystemTextJsonDeserializer();
dynamic data = DynamicDictionary.Create(json, deserializer);

// 💎 Dependency Injection
DynamicDictionaryJsonExtensions.SetJsonDeserializer(customDeserializer);

// 💎 Easy Testing
var mockDeserializer = new MockDeserializer();
var result = DynamicDictionary.Create(json, mockDeserializer);
```

---

**파일 위치**
- Interface: `OneCiel.System.Dynamics/JsonSerializationInterfaces.cs`
- Implementation: `OneCiel.System.Dynamics.JsonExtension/SystemTextJsonImplementations.cs`
- Factory: `OneCiel.System.Dynamics/DynamicDictionary.cs`
- Extensions: `OneCiel.System.Dynamics.JsonExtension/DynamicDictionaryJsonExtensions.cs`
- Examples: `Examples/RestApiUsageExample.cs`
- Documentation: `STANDARDIZED_INTERFACE_ARCHITECTURE.md`

