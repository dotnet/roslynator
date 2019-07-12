# SyntaxInfo\.SimpleAssignmentExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleAssignmentExpressionInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md) from the specified assignment expression\. |
| [SimpleAssignmentExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md) from the specified node\. |

## SimpleAssignmentExpressionInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md) from the specified assignment expression\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression** &ensp; [AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md)

## SimpleAssignmentExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md)

