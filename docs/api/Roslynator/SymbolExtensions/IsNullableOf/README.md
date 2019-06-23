# SymbolExtensions\.IsNullableOf Method

[Home](../../../README.md)

**Containing Type**: [SymbolExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsNullableOf(INamedTypeSymbol, ITypeSymbol)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(INamedTypeSymbol, SpecialType)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, ITypeSymbol)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, SpecialType)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |

## IsNullableOf\(INamedTypeSymbol, ITypeSymbol\) <a id="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_"></a>

\
Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.INamedTypeSymbol namedTypeSymbol, Microsoft.CodeAnalysis.ITypeSymbol typeArgument)
```

### Parameters

**namedTypeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**typeArgument** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsNullableOf\(INamedTypeSymbol, SpecialType\) <a id="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_SpecialType_"></a>

\
Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.INamedTypeSymbol namedTypeSymbol, Microsoft.CodeAnalysis.SpecialType specialType)
```

### Parameters

**namedTypeSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**specialType** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsNullableOf\(ITypeSymbol, ITypeSymbol\) <a id="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_"></a>

\
Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.ITypeSymbol typeArgument)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**typeArgument** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsNullableOf\(ITypeSymbol, SpecialType\) <a id="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_"></a>

\
Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.SpecialType specialType)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**specialType** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

