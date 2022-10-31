# SyntaxInfo\.StringLiteralExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [StringLiteralExpressionInfo(LiteralExpressionSyntax)](#Roslynator_CSharp_SyntaxInfo_StringLiteralExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LiteralExpressionSyntax_) | Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/README.md) from the specified literal expression\. |
| [StringLiteralExpressionInfo(SyntaxNode, Boolean)](#Roslynator_CSharp_SyntaxInfo_StringLiteralExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_) | Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/README.md) from the specified node\. |

## StringLiteralExpressionInfo\(LiteralExpressionSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_StringLiteralExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LiteralExpressionSyntax_"></a>

\
Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/README.md) from the specified literal expression\.

```csharp
public static Roslynator.CSharp.Syntax.StringLiteralExpressionInfo StringLiteralExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax literalExpression)
```

### Parameters

**literalExpression** &ensp; [LiteralExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.literalexpressionsyntax)

### Returns

[StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/README.md)

## StringLiteralExpressionInfo\(SyntaxNode, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_StringLiteralExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_"></a>

\
Creates a new [StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.StringLiteralExpressionInfo StringLiteralExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[StringLiteralExpressionInfo](../../Syntax/StringLiteralExpressionInfo/README.md)

