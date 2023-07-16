---
sidebar_label: SimpleAssignmentStatementInfo
---

# SyntaxInfo\.SimpleAssignmentStatementInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleAssignmentStatementInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#3727216217) | Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md) from the specified assignment expression\. |
| [SimpleAssignmentStatementInfo(ExpressionStatementSyntax, Boolean, Boolean)](#1396493216) | Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md) from the specified expression statement\. |
| [SimpleAssignmentStatementInfo(StatementSyntax, Boolean, Boolean)](#4249106749) | Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md) from the specified statement\. |

<a id="3727216217"></a>

## SimpleAssignmentStatementInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) 

  
Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md) from the specified assignment expression\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression** &ensp; [AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md)

<a id="1396493216"></a>

## SimpleAssignmentStatementInfo\(ExpressionStatementSyntax, Boolean, Boolean\) 

  
Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md) from the specified expression statement\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax expressionStatement, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**expressionStatement** &ensp; [ExpressionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionstatementsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md)

<a id="4249106749"></a>

## SimpleAssignmentStatementInfo\(StatementSyntax, Boolean, Boolean\) 

  
Creates a new [SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md) from the specified statement\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleAssignmentStatementInfo](../../Syntax/SimpleAssignmentStatementInfo/index.md)

