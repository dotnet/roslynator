---
sidebar_label: InheritsFrom
---

# SymbolExtensions\.InheritsFrom Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [InheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](#2746876473) | Returns true if the type inherits from a specified base type\. |
| [InheritsFrom(ITypeSymbol, MetadataName, Boolean)](#3951984790) | Returns true if the type inherits from a type with the specified name\. |

<a id="2746876473"></a>

## InheritsFrom\(ITypeSymbol, ITypeSymbol, Boolean\) 

  
Returns true if the type inherits from a specified base type\.

```csharp
public static bool InheritsFrom(this Microsoft.CodeAnalysis.ITypeSymbol type, Microsoft.CodeAnalysis.ITypeSymbol baseType, bool includeInterfaces = false)
```

### Parameters

**type** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**baseType** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**includeInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="3951984790"></a>

## InheritsFrom\(ITypeSymbol, MetadataName, Boolean\) 

  
Returns true if the type inherits from a type with the specified name\.

```csharp
public static bool InheritsFrom(this Microsoft.CodeAnalysis.ITypeSymbol type, in Roslynator.MetadataName baseTypeName, bool includeInterfaces = false)
```

### Parameters

**type** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**baseTypeName** &ensp; [MetadataName](../../MetadataName/index.md)

**includeInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

