# SyntaxInfo\.ConditionalExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ConditionalExpressionInfo(ConditionalExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_ConditionalExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ConditionalExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/README.md) from the specified conditional expression\. |
| [ConditionalExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_ConditionalExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/README.md) from the specified node\. |

## ConditionalExpressionInfo\(ConditionalExpressionSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_ConditionalExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ConditionalExpressionSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/README.md) from the specified conditional expression\.

```csharp
public static Roslynator.CSharp.Syntax.ConditionalExpressionInfo ConditionalExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalExpressionSyntax conditionalExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**conditionalExpression** &ensp; [ConditionalExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.conditionalexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/README.md)

## ConditionalExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_ConditionalExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

\
Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.ConditionalExpressionInfo ConditionalExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/README.md)

