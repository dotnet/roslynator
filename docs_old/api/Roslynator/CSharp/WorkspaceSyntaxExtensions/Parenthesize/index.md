---
sidebar_label: Parenthesize
---

# WorkspaceSyntaxExtensions\.Parenthesize\(ExpressionSyntax, Boolean, Boolean\) Method

**Containing Type**: [WorkspaceSyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
Creates parenthesized expression that is parenthesizing the specified expression\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedExpressionSyntax Parenthesize(this Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, bool includeElasticTrivia = true, bool simplifiable = true)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**includeElasticTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, add elastic trivia\.

**simplifiable** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, attach [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the parenthesized expression\.

### Returns

[ParenthesizedExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parenthesizedexpressionsyntax)

