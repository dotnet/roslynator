# SyntaxInfo\.AssignmentExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AssignmentExpressionInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md) from the specified assignment expression\. |
| [AssignmentExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md) from the specified node\. |

## AssignmentExpressionInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md) from the specified assignment expression\.

```csharp
public static Roslynator.CSharp.Syntax.AssignmentExpressionInfo AssignmentExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression** &ensp; [AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md)

## AssignmentExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

\
Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.AssignmentExpressionInfo AssignmentExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md)

