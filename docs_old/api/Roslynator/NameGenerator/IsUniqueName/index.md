---
sidebar_label: IsUniqueName
---

# NameGenerator\.IsUniqueName Method

**Containing Type**: [NameGenerator](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsUniqueName(String, IEnumerable&lt;String&gt;, Boolean)](#2992007639) | Returns true if the name is not contained in the specified list\. |
| [IsUniqueName(String, ImmutableArray&lt;ISymbol&gt;, Boolean)](#2911018138) | Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\. |

<a id="2992007639"></a>

## IsUniqueName\(String, IEnumerable&lt;String&gt;, Boolean\) 

  
Returns true if the name is not contained in the specified list\.

```csharp
public static bool IsUniqueName(string name, System.Collections.Generic.IEnumerable<string> reservedNames, bool isCaseSensitive = true)
```

### Parameters

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**reservedNames** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)&gt;

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2911018138"></a>

## IsUniqueName\(String, ImmutableArray&lt;ISymbol&gt;, Boolean\) 

  
Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\.

```csharp
public static bool IsUniqueName(string name, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ISymbol> symbols, bool isCaseSensitive = true)
```

### Parameters

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**symbols** &ensp; [ImmutableArray](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablearray-1)&lt;[ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)&gt;

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

