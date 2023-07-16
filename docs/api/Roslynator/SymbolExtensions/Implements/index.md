---
sidebar_label: Implements
---

# SymbolExtensions\.Implements Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Implements(ITypeSymbol, INamedTypeSymbol, Boolean)](#1804500735) | Returns true if the type implements specified interface\. |
| [Implements(ITypeSymbol, MetadataName, Boolean)](#3538366426) | Returns true if the type implements specified interface name\. |
| [Implements(ITypeSymbol, SpecialType, Boolean)](#2161671967) | Returns true if the type implements specified interface\. |

<a id="1804500735"></a>

## Implements\(ITypeSymbol, INamedTypeSymbol, Boolean\) 

  
Returns true if the type implements specified interface\.

```csharp
public static bool Implements(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.INamedTypeSymbol interfaceSymbol, bool allInterfaces = false)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**interfaceSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="3538366426"></a>

## Implements\(ITypeSymbol, MetadataName, Boolean\) 

  
Returns true if the type implements specified interface name\.

```csharp
public static bool Implements(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, in Roslynator.MetadataName interfaceName, bool allInterfaces = false)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**interfaceName** &ensp; [MetadataName](../../MetadataName/index.md)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2161671967"></a>

## Implements\(ITypeSymbol, SpecialType, Boolean\) 

  
Returns true if the type implements specified interface\.

```csharp
public static bool Implements(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.SpecialType interfaceType, bool allInterfaces = false)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**interfaceType** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

