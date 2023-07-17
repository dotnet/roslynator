---
sidebar_label: RefactoringTestData
---

# RefactoringTestData Class

**Namespace**: [Roslynator.Testing](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Gets test data for a code refactoring\.

```csharp
public sealed class RefactoringTestData
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; RefactoringTestData

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [RefactoringTestData(String, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;AdditionalFile&gt;, String)](-ctor/index.md) | Initializes a new instance of [RefactoringTestData](./index.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [AdditionalFiles](AdditionalFiles/index.md) | Gets additional source files\. |
| [EquivalenceKey](EquivalenceKey/index.md) | Gets code action's equivalence key\. |
| [Source](Source/index.md) | Gets a source code to be refactored\. |
| [Spans](Spans/index.md) | Gets text spans on which a code refactoring will be applied\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Update(String, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;AdditionalFile&gt;, String)](Update/index.md) | Creates and return new instance of [RefactoringTestData](./index.md) updated with specified values\. |

