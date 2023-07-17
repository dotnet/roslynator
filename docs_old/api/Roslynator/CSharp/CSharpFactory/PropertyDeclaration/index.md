---
sidebar_label: PropertyDeclaration
---

# CSharpFactory\.PropertyDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [PropertyDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, AccessorListSyntax, ExpressionSyntax)](#951058213) | |
| [PropertyDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, ArrowExpressionClauseSyntax)](#1337449262) | |

<a id="951058213"></a>

## PropertyDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, AccessorListSyntax, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax PropertyDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**accessorList** &ensp; [AccessorListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessorlistsyntax)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[PropertyDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.propertydeclarationsyntax)

<a id="1337449262"></a>

## PropertyDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, ArrowExpressionClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax PropertyDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**expressionBody** &ensp; [ArrowExpressionClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.arrowexpressionclausesyntax)

### Returns

[PropertyDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.propertydeclarationsyntax)

