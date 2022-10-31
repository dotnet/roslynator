# SyntaxExtensions\.GetTopmostIf Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetTopmostIf(ElseClauseSyntax)](#Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax_) | Returns topmost if statement of the if\-else cascade the specified else clause is part of\. |
| [GetTopmostIf(IfStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax_) | Returns topmost if statement of the if\-else cascade the specified if statement is part of\. |

## GetTopmostIf\(ElseClauseSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax_"></a>

\
Returns topmost if statement of the if\-else cascade the specified else clause is part of\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax GetTopmostIf(this Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax elseClause)
```

### Parameters

**elseClause** &ensp; [ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)

### Returns

[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

## GetTopmostIf\(IfStatementSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax_"></a>

\
Returns topmost if statement of the if\-else cascade the specified if statement is part of\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax GetTopmostIf(this Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax ifStatement)
```

### Parameters

**ifStatement** &ensp; [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

### Returns

[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

