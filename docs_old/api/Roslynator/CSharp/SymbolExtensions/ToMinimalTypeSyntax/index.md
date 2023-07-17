---
sidebar_label: ToMinimalTypeSyntax
---

# SymbolExtensions\.ToMinimalTypeSyntax Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ToMinimalTypeSyntax(INamespaceOrTypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](#3588425153) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol |
| [ToMinimalTypeSyntax(INamespaceSymbol, SemanticModel, Int32, SymbolDisplayFormat)](#4080868940) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\. |
| [ToMinimalTypeSyntax(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](#2161128311) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |

<a id="3588425153"></a>

## ToMinimalTypeSyntax\(INamespaceOrTypeSymbol, SemanticModel, Int32, SymbolDisplayFormat\) 

  
Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ToMinimalTypeSyntax(this Microsoft.CodeAnalysis.INamespaceOrTypeSymbol namespaceOrTypeSymbol, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null)
```

### Parameters

**namespaceOrTypeSymbol** &ensp; [INamespaceOrTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamespaceortypesymbol)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**format** &ensp; [SymbolDisplayFormat](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symboldisplayformat)

### Returns

[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

<a id="4080868940"></a>

## ToMinimalTypeSyntax\(INamespaceSymbol, SemanticModel, Int32, SymbolDisplayFormat\) 

  
Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ToMinimalTypeSyntax(this Microsoft.CodeAnalysis.INamespaceSymbol namespaceSymbol, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null)
```

### Parameters

**namespaceSymbol** &ensp; [INamespaceSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamespacesymbol)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**format** &ensp; [SymbolDisplayFormat](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symboldisplayformat)

### Returns

[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

<a id="2161128311"></a>

## ToMinimalTypeSyntax\(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat\) 

  
Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ToMinimalTypeSyntax(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**format** &ensp; [SymbolDisplayFormat](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symboldisplayformat)

### Returns

[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

