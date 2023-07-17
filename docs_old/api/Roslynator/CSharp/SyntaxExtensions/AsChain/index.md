---
sidebar_label: AsChain
---

# SyntaxExtensions\.AsChain\(BinaryExpressionSyntax, TextSpan?\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns [ExpressionChain](../../ExpressionChain/index.md) that enables to enumerate expressions of a binary expression\.

```csharp
public static Roslynator.CSharp.ExpressionChain AsChain(this Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax binaryExpression, Microsoft.CodeAnalysis.Text.TextSpan? span = null)
```

### Parameters

**binaryExpression** &ensp; [BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)?

### Returns

[ExpressionChain](../../ExpressionChain/index.md)

