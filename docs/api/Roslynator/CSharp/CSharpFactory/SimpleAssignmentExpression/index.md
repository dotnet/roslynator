---
sidebar_label: SimpleAssignmentExpression
---

# CSharpFactory\.SimpleAssignmentExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleAssignmentExpression(ExpressionSyntax, ExpressionSyntax)](#3358121271) | |
| [SimpleAssignmentExpression(ExpressionSyntax, SyntaxToken, ExpressionSyntax)](#455189239) | |

<a id="3358121271"></a>

## SimpleAssignmentExpression\(ExpressionSyntax, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax SimpleAssignmentExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

<a id="455189239"></a>

## SimpleAssignmentExpression\(ExpressionSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax SimpleAssignmentExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax left, Microsoft.CodeAnalysis.SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax right)
```

### Parameters

**left** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**operatorToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**right** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[AssignmentExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.assignmentexpressionsyntax)

