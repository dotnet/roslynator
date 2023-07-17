---
sidebar_label: UncheckedExpression
---

# CSharpFactory\.UncheckedExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [UncheckedExpression(ExpressionSyntax)](#581773469) | |
| [UncheckedExpression(SyntaxToken, ExpressionSyntax, SyntaxToken)](#1917840557) | |

<a id="581773469"></a>

## UncheckedExpression\(ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax UncheckedExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[CheckedExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.checkedexpressionsyntax)

<a id="1917840557"></a>

## UncheckedExpression\(SyntaxToken, ExpressionSyntax, SyntaxToken\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax UncheckedExpression(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken)
```

### Parameters

**openParenToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**closeParenToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[CheckedExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.checkedexpressionsyntax)

