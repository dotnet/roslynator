---
sidebar_label: DiagnosticTestData
---

# DiagnosticTestData Class

**Namespace**: [Roslynator.Testing](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Represents test data for a diagnostic and its fix\.

```csharp
public sealed class DiagnosticTestData
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; DiagnosticTestData

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [DiagnosticTestData(DiagnosticDescriptor, String, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;AdditionalFile&gt;, String, IFormatProvider, String, Boolean)](-ctor/index.md) | Initializes a new instance of [DiagnosticTestData](./index.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [AdditionalFiles](AdditionalFiles/index.md) | Gets additional source files\. |
| [AdditionalSpans](AdditionalSpans/index.md) | Gets diagnostic's additional locations\. |
| [AlwaysVerifyAdditionalLocations](AlwaysVerifyAdditionalLocations/index.md) | True if additional locations should be always verified\. |
| [Descriptor](Descriptor/index.md) | Gets diagnostic's descriptor\. |
| [DiagnosticMessage](DiagnosticMessage/index.md) | Gets diagnostic's message |
| [EquivalenceKey](EquivalenceKey/index.md) | Gets code action's equivalence key\. |
| [FormatProvider](FormatProvider/index.md) | Gets format provider to be used to format diagnostic's message\. |
| [Source](Source/index.md) | Gets source that will report specified diagnostic\. |
| [Spans](Spans/index.md) | Gets diagnostic's locations\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Update(DiagnosticDescriptor, String, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;AdditionalFile&gt;, String, IFormatProvider, String, Boolean)](Update/index.md) | Creates and return new instance of [DiagnosticTestData](./index.md) updated with specified values\. |

