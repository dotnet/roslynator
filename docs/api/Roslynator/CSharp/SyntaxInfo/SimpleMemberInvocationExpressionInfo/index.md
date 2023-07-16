---
sidebar_label: SimpleMemberInvocationExpressionInfo
---

# SyntaxInfo\.SimpleMemberInvocationExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleMemberInvocationExpressionInfo(InvocationExpressionSyntax, Boolean)](#3355621655) | Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/index.md) from the specified invocation expression\. |
| [SimpleMemberInvocationExpressionInfo(SyntaxNode, Boolean, Boolean)](#2278576838) | Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/index.md) from the specified node\. |

<a id="3355621655"></a>

## SimpleMemberInvocationExpressionInfo\(InvocationExpressionSyntax, Boolean\) 

  
Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/index.md) from the specified invocation expression\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleMemberInvocationExpressionInfo SimpleMemberInvocationExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax invocationExpression, bool allowMissing = false)
```

### Parameters

**invocationExpression** &ensp; [InvocationExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.invocationexpressionsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/index.md)

<a id="2278576838"></a>

## SimpleMemberInvocationExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleMemberInvocationExpressionInfo SimpleMemberInvocationExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/index.md)

