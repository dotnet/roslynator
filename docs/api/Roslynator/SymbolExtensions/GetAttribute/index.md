---
sidebar_label: GetAttribute
---

# SymbolExtensions\.GetAttribute Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetAttribute(ISymbol, INamedTypeSymbol)](#1998351864) | Returns the attribute for the symbol that matches the specified attribute class, or null if the symbol does not have the specified attribute\. |
| [GetAttribute(ISymbol, MetadataName)](#596707890) | Returns the attribute for the symbol that matches the specified name, or null if the symbol does not have the specified attribute\. |

<a id="1998351864"></a>

## GetAttribute\(ISymbol, INamedTypeSymbol\) 

  
Returns the attribute for the symbol that matches the specified attribute class, or null if the symbol does not have the specified attribute\.

```csharp
public static Microsoft.CodeAnalysis.AttributeData GetAttribute(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.INamedTypeSymbol attributeClass)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**attributeClass** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

### Returns

[AttributeData](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.attributedata)

<a id="596707890"></a>

## GetAttribute\(ISymbol, MetadataName\) 

  
Returns the attribute for the symbol that matches the specified name, or null if the symbol does not have the specified attribute\.

```csharp
public static Microsoft.CodeAnalysis.AttributeData GetAttribute(this Microsoft.CodeAnalysis.ISymbol symbol, in Roslynator.MetadataName attributeName)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**attributeName** &ensp; [MetadataName](../../MetadataName/index.md)

### Returns

[AttributeData](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.attributedata)

