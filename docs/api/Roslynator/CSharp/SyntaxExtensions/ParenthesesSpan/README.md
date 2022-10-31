# SyntaxExtensions\.ParenthesesSpan Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ParenthesesSpan(CastExpressionSyntax)](#Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CastExpressionSyntax_) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(CommonForEachStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CommonForEachStatementSyntax_) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(ForStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_ForStatementSyntax_) | Absolute span of the parentheses, not including the leading and trailing trivia\. |

## ParenthesesSpan\(CastExpressionSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CastExpressionSyntax_"></a>

\
The absolute span of the parentheses, not including its leading and trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.Text.TextSpan ParenthesesSpan(this Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax castExpression)
```

### Parameters

**castExpression** &ensp; [CastExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.castexpressionsyntax)

### Returns

[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

## ParenthesesSpan\(CommonForEachStatementSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CommonForEachStatementSyntax_"></a>

\
The absolute span of the parentheses, not including its leading and trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.Text.TextSpan ParenthesesSpan(this Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax forEachStatement)
```

### Parameters

**forEachStatement** &ensp; [CommonForEachStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.commonforeachstatementsyntax)

### Returns

[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

## ParenthesesSpan\(ForStatementSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_ForStatementSyntax_"></a>

\
Absolute span of the parentheses, not including the leading and trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.Text.TextSpan ParenthesesSpan(this Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax forStatement)
```

### Parameters

**forStatement** &ensp; [ForStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.forstatementsyntax)

### Returns

[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

