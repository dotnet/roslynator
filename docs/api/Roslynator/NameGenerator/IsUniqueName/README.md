# NameGenerator\.IsUniqueName Method

[Home](../../../README.md)

**Containing Type**: [NameGenerator](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsUniqueName(String, IEnumerable\<String\>, Boolean)](#2992007639) | Returns true if the name is not contained in the specified list\. |
| [IsUniqueName(String, ImmutableArray\<ISymbol\>, Boolean)](#2911018138) | Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\. |

<a id="2992007639"></a>

## IsUniqueName\(String, IEnumerable\<String\>, Boolean\) 

  
Returns true if the name is not contained in the specified list\.

```csharp
public static bool IsUniqueName(string name, System.Collections.Generic.IEnumerable<string> reservedNames, bool isCaseSensitive = true)
```

### Parameters

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**reservedNames** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\>

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2911018138"></a>

## IsUniqueName\(String, ImmutableArray\<ISymbol\>, Boolean\) 

  
Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\.

```csharp
public static bool IsUniqueName(string name, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ISymbol> symbols, bool isCaseSensitive = true)
```

### Parameters

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**symbols** &ensp; [ImmutableArray](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablearray-1)\<[ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\>

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

