# SyntaxInfo\.SimpleAssignmentStatementInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleAssignmentStatementInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleAssignmentStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md) from the specified assignment expression\. |
| [SimpleAssignmentStatementInfo(ExpressionStatementSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleAssignmentStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionStatementSyntax_System_Boolean_System_Boolean_) | Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md) from the specified expression statement\. |
| [SimpleAssignmentStatementInfo(StatementSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleAssignmentStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_System_Boolean_System_Boolean_) | Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md) from the specified statement\. |

## SimpleAssignmentStatementInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleAssignmentStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md) from the specified assignment expression\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression** &ensp; [AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md)

## SimpleAssignmentStatementInfo\(ExpressionStatementSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleAssignmentStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionStatementSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md) from the specified expression statement\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax expressionStatement, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**expressionStatement** &ensp; [ExpressionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionstatementsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md)

## SimpleAssignmentStatementInfo\(StatementSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleAssignmentStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md) from the specified statement\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/README.md)

