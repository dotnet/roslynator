---
sidebar_label: ConditionalExpressionInfo
---

# SyntaxInfo\.ConditionalExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ConditionalExpressionInfo(ConditionalExpressionSyntax, Boolean, Boolean)](#4291540972) | Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/index.md) from the specified conditional expression\. |
| [ConditionalExpressionInfo(SyntaxNode, Boolean, Boolean)](#2909060166) | Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/index.md) from the specified node\. |

<a id="4291540972"></a>

## ConditionalExpressionInfo\(ConditionalExpressionSyntax, Boolean, Boolean\) 

  
Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/index.md) from the specified conditional expression\.

```csharp
public static Roslynator.CSharp.Syntax.ConditionalExpressionInfo ConditionalExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalExpressionSyntax conditionalExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**conditionalExpression** &ensp; [ConditionalExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.conditionalexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/index.md)

<a id="2909060166"></a>

## ConditionalExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.ConditionalExpressionInfo ConditionalExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[ConditionalExpressionInfo](../../Syntax/ConditionalExpressionInfo/index.md)

