---
sidebar_label: FindMember
---

# SymbolExtensions\.FindMember Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
| [FindMember&lt;TSymbol&gt;(INamedTypeSymbol, Func&lt;TSymbol, Boolean&gt;, Boolean)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Func___0_System_Boolean__System_Boolean_) | Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(INamedTypeSymbol, String, Func&lt;TSymbol, Boolean&gt;, Boolean)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_String_System_Func___0_System_Boolean__System_Boolean_) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, Func&lt;TSymbol, Boolean&gt;)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_Func___0_System_Boolean__) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, String, Func&lt;TSymbol, Boolean&gt;)](#Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_String_System_Func___0_System_Boolean__) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |

## FindMember&lt;TSymbol&gt;\(INamedTypeSymbol, Func&lt;TSymbol, Boolean&gt;, Boolean\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Func___0_System_Boolean__System_Boolean_"></a>

========
| [FindMember\<TSymbol\>(INamedTypeSymbol, Func\<TSymbol, Boolean\>, Boolean)](#996682075) | Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(INamedTypeSymbol, String, Func\<TSymbol, Boolean\>, Boolean)](#358208601) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(ITypeSymbol, Func\<TSymbol, Boolean\>)](#2854901772) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(ITypeSymbol, String, Func\<TSymbol, Boolean\>)](#3171999706) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |

<a id="996682075"></a>

## FindMember\<TSymbol\>\(INamedTypeSymbol, Func\<TSymbol, Boolean\>, Boolean\) 

>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md
  
Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, Func<TSymbol, bool> predicate, bool includeBaseTypes = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;
========
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>
>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TSymbol

<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
## FindMember&lt;TSymbol&gt;\(INamedTypeSymbol, String, Func&lt;TSymbol, Boolean&gt;, Boolean\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_INamedTypeSymbol_System_String_System_Func___0_System_Boolean__System_Boolean_"></a>

========
<a id="358208601"></a>

## FindMember\<TSymbol\>\(INamedTypeSymbol, String, Func\<TSymbol, Boolean\>, Boolean\) 

>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md
  
Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.INamedTypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null, bool includeBaseTypes = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;
========
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>
>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TSymbol

<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
## FindMember&lt;TSymbol&gt;\(ITypeSymbol, Func&lt;TSymbol, Boolean&gt;\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_Func___0_System_Boolean__"></a>

========
<a id="2854901772"></a>

## FindMember\<TSymbol\>\(ITypeSymbol, Func\<TSymbol, Boolean\>\) 

>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md
  
Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;
========
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>
>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md

### Returns

TSymbol

<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
## FindMember&lt;TSymbol&gt;\(ITypeSymbol, String, Func&lt;TSymbol, Boolean&gt;\) <a id="Roslynator_SymbolExtensions_FindMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_String_System_Func___0_System_Boolean__"></a>

========
<a id="3171999706"></a>

## FindMember\<TSymbol\>\(ITypeSymbol, String, Func\<TSymbol, Boolean\>\) 

>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md
  
Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\.

```csharp
public static TSymbol FindMember<TSymbol>(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

<<<<<<<< HEAD:docs/api/Roslynator/SymbolExtensions/FindMember/index.md
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;
========
**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<TSymbol, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>
>>>>>>>> main:docs/api/Roslynator/SymbolExtensions/FindMember/README.md

### Returns

TSymbol

