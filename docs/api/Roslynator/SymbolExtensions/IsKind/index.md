---
sidebar_label: IsKind
---

# SymbolExtensions\.IsKind Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind, SymbolKind)](#2562543075) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind)](#3941599818) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind)](#144279932) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind)](#2288796010) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind)](#2241854371) | Returns true if the symbol is the specified kind\. |

<a id="2562543075"></a>

## IsKind\(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind, SymbolKind\) 

  
Returns true if the symbol is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.SymbolKind kind1, Microsoft.CodeAnalysis.SymbolKind kind2, Microsoft.CodeAnalysis.SymbolKind kind3, Microsoft.CodeAnalysis.SymbolKind kind4, Microsoft.CodeAnalysis.SymbolKind kind5)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**kind1** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind2** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind3** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind4** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind5** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="3941599818"></a>

## IsKind\(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind\) 

  
Returns true if the symbol is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.SymbolKind kind1, Microsoft.CodeAnalysis.SymbolKind kind2, Microsoft.CodeAnalysis.SymbolKind kind3, Microsoft.CodeAnalysis.SymbolKind kind4)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**kind1** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind2** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind3** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind4** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="144279932"></a>

## IsKind\(ISymbol, SymbolKind, SymbolKind, SymbolKind\) 

  
Returns true if the symbol is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.SymbolKind kind1, Microsoft.CodeAnalysis.SymbolKind kind2, Microsoft.CodeAnalysis.SymbolKind kind3)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**kind1** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind2** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind3** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2288796010"></a>

## IsKind\(ISymbol, SymbolKind, SymbolKind\) 

  
Returns true if the symbol is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.SymbolKind kind1, Microsoft.CodeAnalysis.SymbolKind kind2)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**kind1** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

**kind2** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2241854371"></a>

## IsKind\(ISymbol, SymbolKind\) 

  
Returns true if the symbol is the specified kind\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.SymbolKind kind)
```

### Parameters

**symbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**kind** &ensp; [SymbolKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symbolkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

