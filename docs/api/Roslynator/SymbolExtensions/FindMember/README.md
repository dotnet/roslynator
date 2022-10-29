# SymbolExtensions\.FindMember Method

[Home](../../../README.md)

**Containing Type**: [SymbolExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FindMember\<TSymbol\>(INamedTypeSymbol, Func\<TSymbol, Boolean\>, Boolean)](#996682075) | Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(INamedTypeSymbol, String, Func\<TSymbol, Boolean\>, Boolean)](#358208601) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(ITypeSymbol, Func\<TSymbol, Boolean\>)](#2854901772) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(ITypeSymbol, String, Func\<TSymbol, Boolean\>)](#3171999706) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |

<a id="996682075"></a>

## FindMember\<TSymbol\>\(INamedTypeSymbol, Func\<TSymbol, Boolean\>, Boolean\) 

  
Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, Func<TSymbol, bool> predicate, bool includeBaseTypes = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TSymbol

<a id="358208601"></a>

## FindMember\<TSymbol\>\(INamedTypeSymbol, String, Func\<TSymbol, Boolean\>, Boolean\) 

  
Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null, bool includeBaseTypes = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TSymbol

<a id="2854901772"></a>

## FindMember\<TSymbol\>\(ITypeSymbol, Func\<TSymbol, Boolean\>\) 

  
Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>

### Returns

TSymbol

<a id="3171999706"></a>

## FindMember\<TSymbol\>\(ITypeSymbol, String, Func\<TSymbol, Boolean\>\) 

  
Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>

### Returns

TSymbol

