---
sidebar_label: ToTypeSyntax
---

# SymbolExtensions\.ToTypeSyntax Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ToTypeSyntax(INamespaceOrTypeSymbol, SymbolDisplayFormat)](#3148432103) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol\. |
| [ToTypeSyntax(INamespaceSymbol, SymbolDisplayFormat)](#3888206216) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\. |
| [ToTypeSyntax(ITypeSymbol, SymbolDisplayFormat)](#3779029411) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |

<a id="3148432103"></a>

## ToTypeSyntax\(INamespaceOrTypeSymbol, SymbolDisplayFormat\) 

  
Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ToTypeSyntax(this Microsoft.CodeAnalysis.INamespaceOrTypeSymbol namespaceOrTypeSymbol, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null)
```

### Parameters

**namespaceOrTypeSymbol** &ensp; [INamespaceOrTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamespaceortypesymbol)

**format** &ensp; [SymbolDisplayFormat](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symboldisplayformat)

### Returns

[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

<a id="3888206216"></a>

## ToTypeSyntax\(INamespaceSymbol, SymbolDisplayFormat\) 

  
Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ToTypeSyntax(this Microsoft.CodeAnalysis.INamespaceSymbol namespaceSymbol, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null)
```

### Parameters

**namespaceSymbol** &ensp; [INamespaceSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamespacesymbol)

**format** &ensp; [SymbolDisplayFormat](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symboldisplayformat)

### Returns

[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

<a id="3779029411"></a>

## ToTypeSyntax\(ITypeSymbol, SymbolDisplayFormat\) 

  
Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ToTypeSyntax(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**format** &ensp; [SymbolDisplayFormat](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symboldisplayformat)

### Returns

[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

