---
sidebar_label: StringLiteralExpressionInfo
---

# SyntaxInfo\.StringLiteralExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [StringLiteralExpressionInfo(LiteralExpressionSyntax)](#2861805613) | Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/index.md) from the specified literal expression\. |
| [StringLiteralExpressionInfo(SyntaxNode, Boolean)](#3487857671) | Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/index.md) from the specified node\. |

<a id="2861805613"></a>

## StringLiteralExpressionInfo\(LiteralExpressionSyntax\) 

  
Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/index.md) from the specified literal expression\.

```csharp
public static Roslynator.CSharp.Syntax.StringLiteralExpressionInfo StringLiteralExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax literalExpression)
```

### Parameters

**literalExpression** &ensp; [LiteralExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.literalexpressionsyntax)

### Returns

[StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/index.md)

<a id="3487857671"></a>

## StringLiteralExpressionInfo\(SyntaxNode, Boolean\) 

  
Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.StringLiteralExpressionInfo StringLiteralExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/index.md)

