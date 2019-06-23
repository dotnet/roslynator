# IfStatementOrElseClause\.Implicit Operator

[Home](../../../../README.md)

**Containing Type**: [IfStatementOrElseClause](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Operator | Summary |
| -------- | ------- |
| [Implicit(ElseClauseSyntax to IfStatementOrElseClause)](#Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax__Roslynator_CSharp_IfStatementOrElseClause) | |
| [Implicit(IfStatementOrElseClause to ElseClauseSyntax)](#Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Roslynator_CSharp_IfStatementOrElseClause___Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax) | |
| [Implicit(IfStatementOrElseClause to IfStatementSyntax)](#Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Roslynator_CSharp_IfStatementOrElseClause___Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax) | |
| [Implicit(IfStatementSyntax to IfStatementOrElseClause)](#Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax__Roslynator_CSharp_IfStatementOrElseClause) | |

## Implicit\(ElseClauseSyntax to IfStatementOrElseClause\) <a id="Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax__Roslynator_CSharp_IfStatementOrElseClause"></a>

```csharp
public static implicit operator Roslynator.CSharp.IfStatementOrElseClause(Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax elseClause)
```

### Parameters

**elseClause** &ensp; [ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)

### Returns

[IfStatementOrElseClause](../README.md)

## Implicit\(IfStatementOrElseClause to ElseClauseSyntax\) <a id="Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Roslynator_CSharp_IfStatementOrElseClause___Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax"></a>

```csharp
public static implicit operator Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax(in Roslynator.CSharp.IfStatementOrElseClause ifOrElse)
```

### Parameters

**ifOrElse** &ensp; [IfStatementOrElseClause](../README.md)

### Returns

[ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)

## Implicit\(IfStatementOrElseClause to IfStatementSyntax\) <a id="Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Roslynator_CSharp_IfStatementOrElseClause___Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax"></a>

```csharp
public static implicit operator Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax(in Roslynator.CSharp.IfStatementOrElseClause ifOrElse)
```

### Parameters

**ifOrElse** &ensp; [IfStatementOrElseClause](../README.md)

### Returns

[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

## Implicit\(IfStatementSyntax to IfStatementOrElseClause\) <a id="Roslynator_CSharp_IfStatementOrElseClause_op_Implicit_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax__Roslynator_CSharp_IfStatementOrElseClause"></a>

```csharp
public static implicit operator Roslynator.CSharp.IfStatementOrElseClause(Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax ifStatement)
```

### Parameters

**ifStatement** &ensp; [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

### Returns

[IfStatementOrElseClause](../README.md)

