---
sidebar_label: SimpleMemberInvocationStatementInfo
---

# SyntaxInfo\.SimpleMemberInvocationStatementInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleMemberInvocationStatementInfo(ExpressionStatementSyntax, Boolean)](#906050472) | Creates a new [SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md) from the specified expression statement\. |
| [SimpleMemberInvocationStatementInfo(InvocationExpressionSyntax, Boolean)](#2601273746) | Creates a new [SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md) from the specified invocation expression\. |
| [SimpleMemberInvocationStatementInfo(SyntaxNode, Boolean)](#4170800706) | Creates a new [SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md) from the specified node\. |

<a id="906050472"></a>

## SimpleMemberInvocationStatementInfo\(ExpressionStatementSyntax, Boolean\) 

  
Creates a new [SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md) from the specified expression statement\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleMemberInvocationStatementInfo SimpleMemberInvocationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax expressionStatement, bool allowMissing = false)
```

### Parameters

**expressionStatement** &ensp; [ExpressionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionstatementsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md)

<a id="2601273746"></a>

## SimpleMemberInvocationStatementInfo\(InvocationExpressionSyntax, Boolean\) 

  
Creates a new [SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md) from the specified invocation expression\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleMemberInvocationStatementInfo SimpleMemberInvocationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax invocationExpression, bool allowMissing = false)
```

### Parameters

**invocationExpression** &ensp; [InvocationExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.invocationexpressionsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md)

<a id="4170800706"></a>

## SimpleMemberInvocationStatementInfo\(SyntaxNode, Boolean\) 

  
Creates a new [SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleMemberInvocationStatementInfo SimpleMemberInvocationStatementInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleMemberInvocationStatementInfo](../../Syntax/SimpleMemberInvocationStatementInfo/index.md)

