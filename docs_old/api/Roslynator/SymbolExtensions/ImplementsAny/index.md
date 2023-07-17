---
sidebar_label: ImplementsAny
---

# SymbolExtensions\.ImplementsAny Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, Boolean)](#1018184594) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean)](#196953422) | Returns true if the type implements any of specified interfaces\. |

<a id="1018184594"></a>

## ImplementsAny\(ITypeSymbol, SpecialType, SpecialType, Boolean\) 

  
Returns true if the type implements any of specified interfaces\.

```csharp
public static bool ImplementsAny(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.SpecialType interfaceType1, Microsoft.CodeAnalysis.SpecialType interfaceType2, bool allInterfaces = false)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**interfaceType1** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**interfaceType2** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="196953422"></a>

## ImplementsAny\(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean\) 

  
Returns true if the type implements any of specified interfaces\.

```csharp
public static bool ImplementsAny(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.SpecialType interfaceType1, Microsoft.CodeAnalysis.SpecialType interfaceType2, Microsoft.CodeAnalysis.SpecialType interfaceType3, bool allInterfaces = false)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**interfaceType1** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**interfaceType2** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**interfaceType3** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

