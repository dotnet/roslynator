---
sidebar_label: AddExpression
---

# CSharpFactory\.AddExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AddExpression(ExpressionSyntax, ExpressionSyntax)](#4039649307) | |
| [AddExpression(ExpressionSyntax, SyntaxToken, ExpressionSyntax)](#2149100472) | |

<a id="4039649307"></a>

## AddExpression\(ExpressionSyntax, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax AddExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

<a id="2149100472"></a>

## AddExpression\(ExpressionSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax AddExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**operatorToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

