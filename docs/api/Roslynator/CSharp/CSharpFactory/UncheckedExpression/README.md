# CSharpFactory\.UncheckedExpression Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [UncheckedExpression(ExpressionSyntax)](#Roslynator_CSharp_CSharpFactory_UncheckedExpression_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_) | |
| [UncheckedExpression(SyntaxToken, ExpressionSyntax, SyntaxToken)](#Roslynator_CSharp_CSharpFactory_UncheckedExpression_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_Microsoft_CodeAnalysis_SyntaxToken_) | |

## UncheckedExpression\(ExpressionSyntax\) <a id="Roslynator_CSharp_CSharpFactory_UncheckedExpression_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax UncheckedExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[CheckedExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.checkedexpressionsyntax)

## UncheckedExpression\(SyntaxToken, ExpressionSyntax, SyntaxToken\) <a id="Roslynator_CSharp_CSharpFactory_UncheckedExpression_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_Microsoft_CodeAnalysis_SyntaxToken_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax UncheckedExpression(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken)
```

### Parameters

**openParenToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**closeParenToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[CheckedExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.checkedexpressionsyntax)

