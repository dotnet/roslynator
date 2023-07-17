---
sidebar_label: SimpleMemberAccessExpression
---

# CSharpFactory\.SimpleMemberAccessExpression Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleMemberAccessExpression(ExpressionSyntax, SimpleNameSyntax)](#3926974401) | |
| [SimpleMemberAccessExpression(ExpressionSyntax, SyntaxToken, SimpleNameSyntax)](#1126784979) | |

<a id="3926974401"></a>

## SimpleMemberAccessExpression\(ExpressionSyntax, SimpleNameSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax SimpleMemberAccessExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**name** &ensp; [SimpleNameSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.simplenamesyntax)

### Returns

[MemberAccessExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberaccessexpressionsyntax)

<a id="1126784979"></a>

## SimpleMemberAccessExpression\(ExpressionSyntax, SyntaxToken, SimpleNameSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax SimpleMemberAccessExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax name)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**operatorToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**name** &ensp; [SimpleNameSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.simplenamesyntax)

### Returns

[MemberAccessExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberaccessexpressionsyntax)

