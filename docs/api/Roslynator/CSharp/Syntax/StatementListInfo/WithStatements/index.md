---
sidebar_label: WithStatements
---

# StatementListInfo\.WithStatements Method

**Containing Type**: [StatementListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithStatements(IEnumerable&lt;StatementSyntax&gt;)](#3677217916) | Creates a new [StatementListInfo](../index.md) with the statements updated\. |
| [WithStatements(SyntaxList&lt;StatementSyntax&gt;)](#1765041563) | Creates a new [StatementListInfo](../index.md) with the statements updated\. |

<a id="3677217916"></a>

## WithStatements\(IEnumerable&lt;StatementSyntax&gt;\) 

  
Creates a new [StatementListInfo](../index.md) with the statements updated\.

```csharp
public Roslynator.CSharp.Syntax.StatementListInfo WithStatements(System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements)
```

### Parameters

**statements** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

### Returns

[StatementListInfo](../index.md)

<a id="1765041563"></a>

## WithStatements\(SyntaxList&lt;StatementSyntax&gt;\) 

  
Creates a new [StatementListInfo](../index.md) with the statements updated\.

```csharp
public Roslynator.CSharp.Syntax.StatementListInfo WithStatements(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements)
```

### Parameters

**statements** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

### Returns

[StatementListInfo](../index.md)

