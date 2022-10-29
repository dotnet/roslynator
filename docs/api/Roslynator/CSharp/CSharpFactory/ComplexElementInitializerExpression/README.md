# CSharpFactory\.ComplexElementInitializerExpression Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ComplexElementInitializerExpression(SeparatedSyntaxList\<ExpressionSyntax\>)](#561848386) | |
| [ComplexElementInitializerExpression(SyntaxToken, SeparatedSyntaxList\<ExpressionSyntax\>, SyntaxToken)](#1369186474) | |

<a id="561848386"></a>

## ComplexElementInitializerExpression\(SeparatedSyntaxList\<ExpressionSyntax\>\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax ComplexElementInitializerExpression(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions = default)
```

### Parameters

**expressions** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)\>

### Returns

[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

<a id="1369186474"></a>

## ComplexElementInitializerExpression\(SyntaxToken, SeparatedSyntaxList\<ExpressionSyntax\>, SyntaxToken\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax ComplexElementInitializerExpression(Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken)
```

### Parameters

**openBraceToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**expressions** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)\>

**closeBraceToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

