---
sidebar_label: ObjectInitializerExpression
---

# CSharpFactory\.ObjectInitializerExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ObjectInitializerExpression(SeparatedSyntaxList&lt;ExpressionSyntax&gt;)](#1011671945) | |
| [ObjectInitializerExpression(SyntaxToken, SeparatedSyntaxList&lt;ExpressionSyntax&gt;, SyntaxToken)](#358178625) | |

<a id="1011671945"></a>

## ObjectInitializerExpression\(SeparatedSyntaxList&lt;ExpressionSyntax&gt;\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax ObjectInitializerExpression(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions = default)
```

### Parameters

**expressions** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)&gt;

### Returns

[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

<a id="358178625"></a>

## ObjectInitializerExpression\(SyntaxToken, SeparatedSyntaxList&lt;ExpressionSyntax&gt;, SyntaxToken\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax ObjectInitializerExpression(Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken)
```

### Parameters

**openBraceToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**expressions** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)&gt;

**closeBraceToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

