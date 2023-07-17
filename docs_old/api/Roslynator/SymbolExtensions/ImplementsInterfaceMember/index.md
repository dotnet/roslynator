---
sidebar_label: ImplementsInterfaceMember
---

# SymbolExtensions\.ImplementsInterfaceMember Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ImplementsInterfaceMember(ISymbol, Boolean)](#1947636977) | Returns true if the symbol implements any interface member\. |
| [ImplementsInterfaceMember(ISymbol, INamedTypeSymbol, Boolean)](#1539225690) | Returns true if the symbol implements any member of the specified interface\. |
| [ImplementsInterfaceMember&lt;TSymbol&gt;(ISymbol, Boolean)](#270427832) | Returns true if the symbol implements any interface member\. |
| [ImplementsInterfaceMember&lt;TSymbol&gt;(ISymbol, INamedTypeSymbol, Boolean)](#2598799324) | Returns true if the symbol implements any member of the specified interface\. |

<a id="1947636977"></a>

## ImplementsInterfaceMember\(ISymbol, Boolean\) 

  
Returns true if the symbol implements any interface member\.

```csharp
public static bool ImplementsInterfaceMember(this Microsoft.CodeAnalysis.ISymbol symbol, bool allInterfaces = false)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1539225690"></a>

## ImplementsInterfaceMember\(ISymbol, INamedTypeSymbol, Boolean\) 

  
Returns true if the symbol implements any member of the specified interface\.

```csharp
public static bool ImplementsInterfaceMember(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.INamedTypeSymbol interfaceSymbol, bool allInterfaces = false)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**interfaceSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="270427832"></a>

## ImplementsInterfaceMember&lt;TSymbol&gt;\(ISymbol, Boolean\) 

  
Returns true if the symbol implements any interface member\.

```csharp
public static bool ImplementsInterfaceMember<TSymbol>(this Microsoft.CodeAnalysis.ISymbol symbol, bool allInterfaces = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2598799324"></a>

## ImplementsInterfaceMember&lt;TSymbol&gt;\(ISymbol, INamedTypeSymbol, Boolean\) 

  
Returns true if the symbol implements any member of the specified interface\.

```csharp
public static bool ImplementsInterfaceMember<TSymbol>(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.INamedTypeSymbol interfaceSymbol, bool allInterfaces = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**interfaceSymbol** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**allInterfaces** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

