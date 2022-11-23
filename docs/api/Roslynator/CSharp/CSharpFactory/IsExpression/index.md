---
sidebar_label: IsExpression
---

# CSharpFactory\.IsExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsExpression(ExpressionSyntax, ExpressionSyntax)](#3604210358) | |
| [IsExpression(ExpressionSyntax, SyntaxToken, ExpressionSyntax)](#3748522859) | |

<a id="3604210358"></a>

## IsExpression\(ExpressionSyntax, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax IsExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

<a id="3748522859"></a>

## IsExpression\(ExpressionSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax IsExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**operatorToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

