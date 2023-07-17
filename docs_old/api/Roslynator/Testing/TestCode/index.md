---
sidebar_label: TestCode
---

# TestCode Struct

**Namespace**: [Roslynator.Testing](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Represents a source code to be tested\.

```csharp
public readonly struct TestCode
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; TestCode

## Properties

| Property | Summary |
| -------- | ------- |
| [AdditionalSpans](AdditionalSpans/index.md) | Gets a collection of spans that represent additional selected text\. |
| [ExpectedValue](ExpectedValue/index.md) | Gets a source code after a code fix or a refactoring was applied\. |
| [Spans](Spans/index.md) | Gets a collection of spans that represent selected text\. |
| [Value](Value/index.md) | Gets a source code that should be tested\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Parse(String, String, String)](Parse/index.md#3209459912) | Finds and replace span that is marked with `[\|\|]` token\. |
| [Parse(String)](Parse/index.md#2022869111) | Finds and removes spans that are marked with `[\|` and `\|]` tokens\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |

