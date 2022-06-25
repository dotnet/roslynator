---
sidebar_label: NameGenerator
---

# NameGenerator Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Provides methods to obtain an unique identifier\.

```csharp
public abstract class NameGenerator
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; NameGenerator

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [NameGenerator()](-ctor/index.md) | |

## Properties

| Property | Summary |
| -------- | ------- |
| [Default](Default/index.md) | Default implementation of [NameGenerator](./index.md) that adds number suffix to ensure uniqueness\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [CreateName(ITypeSymbol, Boolean)](CreateName/index.md) | Creates a syntax identifier from the specified type symbol\. |
| [EnsureUniqueEnumMemberName(String, INamedTypeSymbol, Boolean)](EnsureUniqueEnumMemberName/index.md) | Returns unique enum member name for a specified enum type\. |
| [EnsureUniqueLocalName(String, SemanticModel, Int32, Boolean, CancellationToken)](EnsureUniqueLocalName/index.md) | Return a local name that will be unique at the specified position\. |
| [EnsureUniqueLocalNames(String, SemanticModel, Int32, Int32, Boolean, CancellationToken)](EnsureUniqueLocalNames/index.md) | Return a local names that will be unique at the specified position\. |
| [EnsureUniqueName(String, IEnumerable&lt;String&gt;, Boolean)](EnsureUniqueName/index.md#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_) | Returns an unique name using the specified list of reserved names\. |
| [EnsureUniqueName(String, ImmutableArray&lt;ISymbol&gt;, Boolean)](EnsureUniqueName/index.md#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_) | Returns an unique name using the specified list of symbols\. |
| [EnsureUniqueName(String, SemanticModel, Int32, Boolean)](EnsureUniqueName/index.md#Roslynator_NameGenerator_EnsureUniqueName_System_String_Microsoft_CodeAnalysis_SemanticModel_System_Int32_System_Boolean_) | Returns a name that will be unique at the specified position\. |
| [EnsureUniqueParameterName(String, ISymbol, SemanticModel, Boolean, CancellationToken)](EnsureUniqueParameterName/index.md) | Return a parameter name that will be unique at the specified position\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [IsUniqueName(String, IEnumerable&lt;String&gt;, Boolean)](IsUniqueName/index.md#Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_) | Returns true if the name is not contained in the specified list\. |
| [IsUniqueName(String, ImmutableArray&lt;ISymbol&gt;, Boolean)](IsUniqueName/index.md#Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_) | Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

