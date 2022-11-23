---
sidebar_label: CheckedExpression
---

# CSharpFactory\.CheckedExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [CheckedExpression(ExpressionSyntax)](#2444239571) | |
| [CheckedExpression(SyntaxToken, ExpressionSyntax, SyntaxToken)](#3821766561) | |

<a id="2444239571"></a>

## CheckedExpression\(ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax CheckedExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[CheckedExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.checkedexpressionsyntax)

<a id="3821766561"></a>

## CheckedExpression\(SyntaxToken, ExpressionSyntax, SyntaxToken\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax CheckedExpression(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken)
```

### Parameters

**openParenToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**closeParenToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[CheckedExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.checkedexpressionsyntax)

