---
sidebar_label: SimpleAssignmentExpressionInfo
---

# SyntaxInfo\.SimpleAssignmentExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleAssignmentExpressionInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#3198633426) | Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/index.md) from the specified assignment expression\. |
| [SimpleAssignmentExpressionInfo(SyntaxNode, Boolean, Boolean)](#991779826) | Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/index.md) from the specified node\. |

<a id="3198633426"></a>

## SimpleAssignmentExpressionInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) 

  
Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/index.md) from the specified assignment expression\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression** &ensp; [AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/index.md)

<a id="991779826"></a>

## SimpleAssignmentExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/index.md)

