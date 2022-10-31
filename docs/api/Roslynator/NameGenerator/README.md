# NameGenerator Class

[Home](../../README.md) &#x2022; [Constructors](#constructors) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator](../README.md)

**Assembly**: Roslynator\.Core\.dll

\
Provides methods to obtain an unique identifier\.

```csharp
public abstract class NameGenerator
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; NameGenerator

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [NameGenerator()](-ctor/README.md) | |

## Properties

| Property | Summary |
| -------- | ------- |
| [Default](Default/README.md) | Default implementation of [NameGenerator](./README.md) that adds number suffix to ensure uniqueness\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [CreateName(ITypeSymbol, Boolean)](CreateName/README.md) | Creates a syntax identifier from the specified type symbol\. |
| [EnsureUniqueEnumMemberName(String, INamedTypeSymbol, Boolean)](EnsureUniqueEnumMemberName/README.md) | Returns unique enum member name for a specified enum type\. |
| [EnsureUniqueLocalName(String, SemanticModel, Int32, Boolean, CancellationToken)](EnsureUniqueLocalName/README.md) | Return a local name that will be unique at the specified position\. |
| [EnsureUniqueLocalNames(String, SemanticModel, Int32, Int32, Boolean, CancellationToken)](EnsureUniqueLocalNames/README.md) | Return a local names that will be unique at the specified position\. |
| [EnsureUniqueName(String, IEnumerable\<String>, Boolean)](EnsureUniqueName/README.md#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_) | Returns an unique name using the specified list of reserved names\. |
| [EnsureUniqueName(String, ImmutableArray\<ISymbol>, Boolean)](EnsureUniqueName/README.md#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_) | Returns an unique name using the specified list of symbols\. |
| [EnsureUniqueName(String, SemanticModel, Int32, Boolean)](EnsureUniqueName/README.md#Roslynator_NameGenerator_EnsureUniqueName_System_String_Microsoft_CodeAnalysis_SemanticModel_System_Int32_System_Boolean_) | Returns a name that will be unique at the specified position\. |
| [EnsureUniqueParameterName(String, ISymbol, SemanticModel, Boolean, CancellationToken)](EnsureUniqueParameterName/README.md) | Return a parameter name that will be unique at the specified position\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [IsUniqueName(String, IEnumerable\<String>, Boolean)](IsUniqueName/README.md#Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_) | Returns true if the name is not contained in the specified list\. |
| [IsUniqueName(String, ImmutableArray\<ISymbol>, Boolean)](IsUniqueName/README.md#Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_) | Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

