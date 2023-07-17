---
sidebar_label: FindTypeMember
---

# SymbolExtensions\.FindTypeMember Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FindTypeMember(INamedTypeSymbol, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean)](#931525377) | Searches for a type member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, String, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean)](#4255324844) | Searches for a type member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, String, Int32, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean)](#3885424205) | Searches for a type member that has the specified name, arity and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |

<a id="931525377"></a>

## FindTypeMember\(INamedTypeSymbol, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean\) 

  
Searches for a type member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\.

```csharp
public static Microsoft.CodeAnalysis.INamedTypeSymbol FindTypeMember(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, Func<Microsoft.CodeAnalysis.INamedTypeSymbol, bool> predicate, bool includeBaseTypes = false)
```

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

<a id="4255324844"></a>

## FindTypeMember\(INamedTypeSymbol, String, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean\) 

  
Searches for a type member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static Microsoft.CodeAnalysis.INamedTypeSymbol FindTypeMember(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, string name, Func<Microsoft.CodeAnalysis.INamedTypeSymbol, bool> predicate = null, bool includeBaseTypes = false)
```

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

<a id="3885424205"></a>

## FindTypeMember\(INamedTypeSymbol, String, Int32, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean\) 

  
Searches for a type member that has the specified name, arity and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static Microsoft.CodeAnalysis.INamedTypeSymbol FindTypeMember(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, string name, int arity, Func<Microsoft.CodeAnalysis.INamedTypeSymbol, bool> predicate = null, bool includeBaseTypes = false)
```

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**arity** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

