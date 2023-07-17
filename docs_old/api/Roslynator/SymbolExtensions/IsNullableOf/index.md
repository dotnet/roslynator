---
sidebar_label: IsNullableOf
---

# SymbolExtensions\.IsNullableOf Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsNullableOf(INamedTypeSymbol, ITypeSymbol)](#831430666) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(INamedTypeSymbol, SpecialType)](#1928104294) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, ITypeSymbol)](#2277729142) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, SpecialType)](#467484347) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |

<a id="831430666"></a>

## IsNullableOf\(INamedTypeSymbol, ITypeSymbol\) 

  
Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.INamedTypeSymbol namedTypeSymbol, Microsoft.CodeAnalysis.ITypeSymbol typeArgument)
```

### Parameters

**namedTypeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**typeArgument** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1928104294"></a>

## IsNullableOf\(INamedTypeSymbol, SpecialType\) 

  
Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.INamedTypeSymbol namedTypeSymbol, Microsoft.CodeAnalysis.SpecialType specialType)
```

### Parameters

**namedTypeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**specialType** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2277729142"></a>

## IsNullableOf\(ITypeSymbol, ITypeSymbol\) 

  
Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.ITypeSymbol typeArgument)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**typeArgument** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="467484347"></a>

## IsNullableOf\(ITypeSymbol, SpecialType\) 

  
Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.SpecialType specialType)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**specialType** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

