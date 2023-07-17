---
sidebar_label: LocalFunctionStatement
---

# CSharpFactory\.LocalFunctionStatement Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LocalFunctionStatement(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, ArrowExpressionClauseSyntax)](#1441796136) | |
| [LocalFunctionStatement(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, BlockSyntax)](#351293307) | |

<a id="1441796136"></a>

## LocalFunctionStatement\(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, ArrowExpressionClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**returnType** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**parameterList** &ensp; [ParameterListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parameterlistsyntax)

**expressionBody** &ensp; [ArrowExpressionClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.arrowexpressionclausesyntax)

### Returns

[LocalFunctionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localfunctionstatementsyntax)

<a id="351293307"></a>

## LocalFunctionStatement\(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax, BlockSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax LocalFunctionStatement(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**returnType** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**parameterList** &ensp; [ParameterListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parameterlistsyntax)

**body** &ensp; [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

### Returns

[LocalFunctionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localfunctionstatementsyntax)

