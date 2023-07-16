---
sidebar_label: AssignmentExpressionInfo
---

# SyntaxInfo\.AssignmentExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AssignmentExpressionInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#991774791) | Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/index.md) from the specified assignment expression\. |
| [AssignmentExpressionInfo(SyntaxNode, Boolean, Boolean)](#1140170368) | Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/index.md) from the specified node\. |

<a id="991774791"></a>

## AssignmentExpressionInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) 

  
Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/index.md) from the specified assignment expression\.

```csharp
public static Roslynator.CSharp.Syntax.AssignmentExpressionInfo AssignmentExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression** &ensp; [AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/index.md)

<a id="1140170368"></a>

## AssignmentExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.AssignmentExpressionInfo AssignmentExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/index.md)

