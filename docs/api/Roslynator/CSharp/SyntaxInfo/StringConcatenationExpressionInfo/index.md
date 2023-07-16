---
sidebar_label: StringConcatenationExpressionInfo
---

# SyntaxInfo\.StringConcatenationExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [StringConcatenationExpressionInfo(BinaryExpressionSyntax, SemanticModel, CancellationToken)](#1743355382) | Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md) from the specified node\. |
| [StringConcatenationExpressionInfo(ExpressionChain, SemanticModel, CancellationToken)](#560252336) | Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md) from the specified expression chain\. |
| [StringConcatenationExpressionInfo(SyntaxNode, SemanticModel, Boolean, CancellationToken)](#868850232) | Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md) from the specified node\. |

<a id="1743355382"></a>

## StringConcatenationExpressionInfo\(BinaryExpressionSyntax, SemanticModel, CancellationToken\) 

  
Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.StringConcatenationExpressionInfo StringConcatenationExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax binaryExpression, Microsoft.CodeAnalysis.SemanticModel semanticModel, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**binaryExpression** &ensp; [BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md)

<a id="560252336"></a>

## StringConcatenationExpressionInfo\(ExpressionChain, SemanticModel, CancellationToken\) 

  
Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md) from the specified expression chain\.

```csharp
public static Roslynator.CSharp.Syntax.StringConcatenationExpressionInfo StringConcatenationExpressionInfo(in Roslynator.CSharp.ExpressionChain expressionChain, Microsoft.CodeAnalysis.SemanticModel semanticModel, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**expressionChain** &ensp; [ExpressionChain](../../ExpressionChain/index.md)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md)

<a id="868850232"></a>

## StringConcatenationExpressionInfo\(SyntaxNode, SemanticModel, Boolean, CancellationToken\) 

  
Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.StringConcatenationExpressionInfo StringConcatenationExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.SemanticModel semanticModel, bool walkDownParentheses = true, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/index.md)

