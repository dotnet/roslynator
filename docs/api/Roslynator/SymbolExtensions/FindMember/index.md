---
sidebar_label: FindMember
---

# SymbolExtensions\.FindMember Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FindMember&lt;TSymbol&gt;(INamedTypeSymbol, Func&lt;TSymbol, Boolean&gt;, Boolean)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Func___0_System_Boolean__System_Boolean_) | Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(INamedTypeSymbol, String, Func&lt;TSymbol, Boolean&gt;, Boolean)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_String_System_Func___0_System_Boolean__System_Boolean_) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, Func&lt;TSymbol, Boolean&gt;)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_Func___0_System_Boolean__) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, String, Func&lt;TSymbol, Boolean&gt;)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_String_System_Func___0_System_Boolean__) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |

## FindMember&lt;TSymbol&gt;\(INamedTypeSymbol, Func&lt;TSymbol, Boolean&gt;, Boolean\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Func___0_System_Boolean__System_Boolean_"></a>

  
Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, Func<TSymbol, bool> predicate, bool includeBaseTypes = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TSymbol

## FindMember&lt;TSymbol&gt;\(INamedTypeSymbol, String, Func&lt;TSymbol, Boolean&gt;, Boolean\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_String_System_Func___0_System_Boolean__System_Boolean_"></a>

  
Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null, bool includeBaseTypes = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TSymbol

## FindMember&lt;TSymbol&gt;\(ITypeSymbol, Func&lt;TSymbol, Boolean&gt;\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_Func___0_System_Boolean__"></a>

  
Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

TSymbol

## FindMember&lt;TSymbol&gt;\(ITypeSymbol, String, Func&lt;TSymbol, Boolean&gt;\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_String_System_Func___0_System_Boolean__"></a>

  
Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

TSymbol

