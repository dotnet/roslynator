---
sidebar_label: OrAssignmentExpression
---

# CSharpFactory\.OrAssignmentExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [OrAssignmentExpression(ExpressionSyntax, ExpressionSyntax)](#614755045) | |
| [OrAssignmentExpression(ExpressionSyntax, SyntaxToken, ExpressionSyntax)](#442053808) | |

<a id="614755045"></a>

## OrAssignmentExpression\(ExpressionSyntax, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax OrAssignmentExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

<a id="442053808"></a>

## OrAssignmentExpression\(ExpressionSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax OrAssignmentExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**operatorToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

