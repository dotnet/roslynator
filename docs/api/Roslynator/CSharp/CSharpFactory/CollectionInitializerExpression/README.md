# CSharpFactory\.CollectionInitializerExpression Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [CollectionInitializerExpression(SeparatedSyntaxList\<ExpressionSyntax\>)](#3589452669) | |
| [CollectionInitializerExpression(SyntaxToken, SeparatedSyntaxList\<ExpressionSyntax\>, SyntaxToken)](#3700811157) | |

<a id="3589452669"></a>

## CollectionInitializerExpression\(SeparatedSyntaxList\<ExpressionSyntax\>\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax CollectionInitializerExpression(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions = default)
```

### Parameters

**expressions** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)\>

### Returns

[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

<a id="3700811157"></a>

## CollectionInitializerExpression\(SyntaxToken, SeparatedSyntaxList\<ExpressionSyntax\>, SyntaxToken\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax CollectionInitializerExpression(Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax> expressions, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken)
```

### Parameters

**openBraceToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**expressions** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)\>

**closeBraceToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

