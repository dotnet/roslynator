---
sidebar_label: op_Implicit
---

# IfStatementOrElseClause\.Implicit Operator

**Containing Type**: [IfStatementOrElseClause](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Operator | Summary |
| -------- | ------- |
| [Implicit(ElseClauseSyntax to IfStatementOrElseClause)](#2383111912) | |
| [Implicit(IfStatementOrElseClause to ElseClauseSyntax)](#832248303) | |
| [Implicit(IfStatementOrElseClause to IfStatementSyntax)](#1165473507) | |
| [Implicit(IfStatementSyntax to IfStatementOrElseClause)](#3428183196) | |

<a id="2383111912"></a>

## Implicit\(ElseClauseSyntax to IfStatementOrElseClause\) 

```csharp
public static implicit operator Roslynator.CSharp.IfStatementOrElseClause(Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax elseClause)
```

### Parameters

**elseClause** &ensp; [ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)

### Returns

[IfStatementOrElseClause](../index.md)

<a id="832248303"></a>

## Implicit\(IfStatementOrElseClause to ElseClauseSyntax\) 

```csharp
public static implicit operator Microsoft.CodeAnalysis.CSharp.Syntax.ElseClauseSyntax(in Roslynator.CSharp.IfStatementOrElseClause ifOrElse)
```

### Parameters

**ifOrElse** &ensp; [IfStatementOrElseClause](../index.md)

### Returns

[ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)

<a id="1165473507"></a>

## Implicit\(IfStatementOrElseClause to IfStatementSyntax\) 

```csharp
public static implicit operator Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax(in Roslynator.CSharp.IfStatementOrElseClause ifOrElse)
```

### Parameters

**ifOrElse** &ensp; [IfStatementOrElseClause](../index.md)

### Returns

[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

<a id="3428183196"></a>

## Implicit\(IfStatementSyntax to IfStatementOrElseClause\) 

```csharp
public static implicit operator Roslynator.CSharp.IfStatementOrElseClause(Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax ifStatement)
```

### Parameters

**ifStatement** &ensp; [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

### Returns

[IfStatementOrElseClause](../index.md)

