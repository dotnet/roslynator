---
sidebar_label: HasAttribute
---

# SymbolExtensions\.HasAttribute Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [HasAttribute(ISymbol, INamedTypeSymbol)](#3062983091) | Returns true if the symbol has the specified attribute\. |
| [HasAttribute(ISymbol, MetadataName)](#151999350) | Returns true if the symbol has attribute with the specified name\. |
| [HasAttribute(ITypeSymbol, INamedTypeSymbol, Boolean)](#289352201) | Returns true if the type symbol has the specified attribute\. |
| [HasAttribute(ITypeSymbol, MetadataName, Boolean)](#1814378823) | Returns true if the type symbol has attribute with the specified name\. |

<a id="3062983091"></a>

## HasAttribute\(ISymbol, INamedTypeSymbol\) 

  
Returns true if the symbol has the specified attribute\.

```csharp
public static bool HasAttribute(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.INamedTypeSymbol attributeClass)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**attributeClass** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="151999350"></a>

## HasAttribute\(ISymbol, MetadataName\) 

  
Returns true if the symbol has attribute with the specified name\.

```csharp
public static bool HasAttribute(this Microsoft.CodeAnalysis.ISymbol symbol, in Roslynator.MetadataName attributeName)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**attributeName** &ensp; [MetadataName](../../MetadataName/index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="289352201"></a>

## HasAttribute\(ITypeSymbol, INamedTypeSymbol, Boolean\) 

  
Returns true if the type symbol has the specified attribute\.

```csharp
public static bool HasAttribute(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.INamedTypeSymbol attributeClass, bool includeBaseTypes)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**attributeClass** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1814378823"></a>

## HasAttribute\(ITypeSymbol, MetadataName, Boolean\) 

  
Returns true if the type symbol has attribute with the specified name\.

```csharp
public static bool HasAttribute(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, in Roslynator.MetadataName attributeName, bool includeBaseTypes)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**attributeName** &ensp; [MetadataName](../../MetadataName/index.md)

**includeBaseTypes** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

