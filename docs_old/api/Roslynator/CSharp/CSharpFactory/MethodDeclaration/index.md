---
sidebar_label: MethodDeclaration
---

# CSharpFactory\.MethodDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [MethodDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, ArrowExpressionClauseSyntax)](#1245730861) | |
| [MethodDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, BlockSyntax)](#3794712067) | |

<a id="1245730861"></a>

## MethodDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, ArrowExpressionClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax MethodDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**returnType** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**parameterList** &ensp; [ParameterListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parameterlistsyntax)

**expressionBody** &ensp; [ArrowExpressionClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.arrowexpressionclausesyntax)

### Returns

[MethodDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.methoddeclarationsyntax)

<a id="3794712067"></a>

## MethodDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, BlockSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax MethodDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**returnType** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**parameterList** &ensp; [ParameterListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parameterlistsyntax)

**body** &ensp; [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

### Returns

[MethodDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.methoddeclarationsyntax)

