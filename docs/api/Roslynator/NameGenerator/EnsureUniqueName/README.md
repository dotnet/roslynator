# NameGenerator\.EnsureUniqueName Method

[Home](../../../README.md)

**Containing Type**: [NameGenerator](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [EnsureUniqueName(String, IEnumerable\<String>, Boolean)](#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_) | Returns an unique name using the specified list of reserved names\. |
| [EnsureUniqueName(String, ImmutableArray\<ISymbol>, Boolean)](#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_) | Returns an unique name using the specified list of symbols\. |
| [EnsureUniqueName(String, SemanticModel, Int32, Boolean)](#Roslynator_NameGenerator_EnsureUniqueName_System_String_Microsoft_CodeAnalysis_SemanticModel_System_Int32_System_Boolean_) | Returns a name that will be unique at the specified position\. |

## EnsureUniqueName\(String, IEnumerable\<String>, Boolean\) <a id="Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_"></a>

\
Returns an unique name using the specified list of reserved names\.

```csharp
public abstract string EnsureUniqueName(string baseName, System.Collections.Generic.IEnumerable<string> reservedNames, bool isCaseSensitive = true)
```

### Parameters

**baseName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**reservedNames** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)>

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

## EnsureUniqueName\(String, ImmutableArray\<ISymbol>, Boolean\) <a id="Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_"></a>

\
Returns an unique name using the specified list of symbols\.

```csharp
public abstract string EnsureUniqueName(string baseName, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ISymbol> symbols, bool isCaseSensitive = true)
```

### Parameters

**baseName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**symbols** &ensp; [ImmutableArray](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablearray-1)\<[ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)>

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

## EnsureUniqueName\(String, SemanticModel, Int32, Boolean\) <a id="Roslynator_NameGenerator_EnsureUniqueName_System_String_Microsoft_CodeAnalysis_SemanticModel_System_Int32_System_Boolean_"></a>

\
Returns a name that will be unique at the specified position\.

```csharp
public string EnsureUniqueName(string baseName, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, bool isCaseSensitive = true)
```

### Parameters

**baseName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

