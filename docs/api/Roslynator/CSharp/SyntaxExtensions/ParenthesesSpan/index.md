---
sidebar_label: ParenthesesSpan
---

# SyntaxExtensions\.ParenthesesSpan Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ParenthesesSpan(CastExpressionSyntax)](#1201715952) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(CommonForEachStatementSyntax)](#1008516473) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(ForStatementSyntax)](#3518600528) | Absolute span of the parentheses, not including the leading and trailing trivia\. |

<a id="1201715952"></a>

## ParenthesesSpan\(CastExpressionSyntax\) 

  
The absolute span of the parentheses, not including its leading and trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.Text.TextSpan ParenthesesSpan(this Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax castExpression)
```

### Parameters

**castExpression** &ensp; [CastExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.castexpressionsyntax)

### Returns

[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

<a id="1008516473"></a>

## ParenthesesSpan\(CommonForEachStatementSyntax\) 

  
The absolute span of the parentheses, not including its leading and trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.Text.TextSpan ParenthesesSpan(this Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax forEachStatement)
```

### Parameters

**forEachStatement** &ensp; [CommonForEachStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.commonforeachstatementsyntax)

### Returns

[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

<a id="3518600528"></a>

## ParenthesesSpan\(ForStatementSyntax\) 

  
Absolute span of the parentheses, not including the leading and trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.Text.TextSpan ParenthesesSpan(this Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax forStatement)
```

### Parameters

**forStatement** &ensp; [ForStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.forstatementsyntax)

### Returns

[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

