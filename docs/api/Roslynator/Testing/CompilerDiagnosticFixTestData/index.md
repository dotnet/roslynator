---
sidebar_label: CompilerDiagnosticFixTestData
---

# CompilerDiagnosticFixTestData Class

**Namespace**: [Roslynator.Testing](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Represents test data for a compiler diagnostic fix\.

```csharp
public sealed class CompilerDiagnosticFixTestData
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; CompilerDiagnosticFixTestData

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [CompilerDiagnosticFixTestData(String, String, IEnumerable&lt;AdditionalFile&gt;, String)](-ctor/index.md) | Initializes a new instance of [CompilerDiagnosticFixTestData](./index.md) |

## Properties

| Property | Summary |
| -------- | ------- |
| [AdditionalFiles](AdditionalFiles/index.md) | Gets additional source files\. |
| [DiagnosticId](DiagnosticId/index.md) | Gets compiler diagnostic ID to be fixed\. |
| [EquivalenceKey](EquivalenceKey/index.md) | Gets code action's equivalence key\. |
| [Source](Source/index.md) | Gets a source code that will report specified compiler diagnostic\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Update(String, String, IEnumerable&lt;AdditionalFile&gt;, String)](Update/index.md) | Creates and return new instance of [CompilerDiagnosticFixTestData](./index.md) updated with specified values\. |

