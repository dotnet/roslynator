---
sidebar_label: EqualsOrInheritsFrom
---

# SymbolExtensions\.EqualsOrInheritsFrom Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [EqualsOrInheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](#3013860381) | Returns true if the type is equal or inherits from a specified base type\. |
| [EqualsOrInheritsFrom(ITypeSymbol, MetadataName, Boolean)](#1803936331) | Returns true if the type is equal or inherits from a type with the specified name\. |

<a id="3013860381"></a>

## EqualsOrInheritsFrom\(ITypeSymbol, ITypeSymbol, Boolean\) 

  
Returns true if the type is equal or inherits from a specified base type\.

```csharp
public static bool EqualsOrInheritsFrom(this Microsoft.CodeAnalysis.ITypeSymbol type, Microsoft.CodeAnalysis.ITypeSymbol baseType, bool includeInterfaces = false)
```

### Parameters

**type** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**baseType** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**includeInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1803936331"></a>

## EqualsOrInheritsFrom\(ITypeSymbol, MetadataName, Boolean\) 

  
Returns true if the type is equal or inherits from a type with the specified name\.

```csharp
public static bool EqualsOrInheritsFrom(this Microsoft.CodeAnalysis.ITypeSymbol type, in Roslynator.MetadataName baseTypeName, bool includeInterfaces = false)
```

### Parameters

**type** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**baseTypeName** &ensp; [MetadataName](../../MetadataName/index.md)

**includeInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

