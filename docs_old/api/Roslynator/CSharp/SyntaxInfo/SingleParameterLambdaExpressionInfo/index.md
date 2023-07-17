---
sidebar_label: SingleParameterLambdaExpressionInfo
---

# SyntaxInfo\.SingleParameterLambdaExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SingleParameterLambdaExpressionInfo(LambdaExpressionSyntax, Boolean)](#2354507096) | Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/index.md) from the specified lambda expression\. |
| [SingleParameterLambdaExpressionInfo(SyntaxNode, Boolean, Boolean)](#2023294591) | Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/index.md) from the specified node\. |

<a id="2354507096"></a>

## SingleParameterLambdaExpressionInfo\(LambdaExpressionSyntax, Boolean\) 

  
Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/index.md) from the specified lambda expression\.

```csharp
public static Roslynator.CSharp.Syntax.SingleParameterLambdaExpressionInfo SingleParameterLambdaExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax lambdaExpression, bool allowMissing = false)
```

### Parameters

**lambdaExpression** &ensp; [LambdaExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.lambdaexpressionsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/index.md)

<a id="2023294591"></a>

## SingleParameterLambdaExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SingleParameterLambdaExpressionInfo SingleParameterLambdaExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/index.md)

