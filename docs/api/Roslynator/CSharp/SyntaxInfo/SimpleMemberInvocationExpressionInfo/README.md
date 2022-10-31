# SyntaxInfo\.SimpleMemberInvocationExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleMemberInvocationExpressionInfo(InvocationExpressionSyntax, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleMemberInvocationExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_InvocationExpressionSyntax_System_Boolean_) | Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/README.md) from the specified invocation expression\. |
| [SimpleMemberInvocationExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleMemberInvocationExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/README.md) from the specified node\. |

## SimpleMemberInvocationExpressionInfo\(InvocationExpressionSyntax, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleMemberInvocationExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_InvocationExpressionSyntax_System_Boolean_"></a>

\
Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/README.md) from the specified invocation expression\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleMemberInvocationExpressionInfo SimpleMemberInvocationExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax invocationExpression, bool allowMissing = false)
```

### Parameters

**invocationExpression** &ensp; [InvocationExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.invocationexpressionsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/README.md)

## SimpleMemberInvocationExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleMemberInvocationExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleMemberInvocationExpressionInfo SimpleMemberInvocationExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleMemberInvocationExpressionInfo](../../Syntax/SimpleMemberInvocationExpressionInfo/README.md)

