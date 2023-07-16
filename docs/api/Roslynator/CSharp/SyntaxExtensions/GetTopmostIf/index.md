---
sidebar_label: GetTopmostIf
---

# SyntaxExtensions\.GetTopmostIf Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetTopmostIf(ElseClauseSyntax)](#2176362029) | Returns topmost if statement of the if\-else cascade the specified else clause is part of\. |
| [GetTopmostIf(IfStatementSyntax)](#210946778) | Returns topmost if statement of the if\-else cascade the specified if statement is part of\. |

<a id="2176362029"></a>

## GetTopmostIf\(ElseClauseSyntax\) 

  
Returns topmost if statement of the if\-else cascade the specified else clause is part of\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax GetTopmostIf(this Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax elseClause)
```

### Parameters

**elseClause** &ensp; [ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)

### Returns

[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

<a id="210946778"></a>

## GetTopmostIf\(IfStatementSyntax\) 

  
Returns topmost if statement of the if\-else cascade the specified if statement is part of\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax GetTopmostIf(this Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax ifStatement)
```

### Parameters

**ifStatement** &ensp; [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

### Returns

[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

