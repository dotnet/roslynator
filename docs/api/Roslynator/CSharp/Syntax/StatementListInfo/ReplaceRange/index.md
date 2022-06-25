---
sidebar_label: ReplaceRange
---

# StatementListInfo\.ReplaceRange\(StatementSyntax, IEnumerable&lt;StatementSyntax&gt;\) Method

**Containing Type**: [StatementListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Creates a new [StatementListInfo](../index.md) with the specified statement replaced with new statements\.

```csharp
public Roslynator.CSharp.Syntax.StatementListInfo ReplaceRange(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statementInList, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> newStatements)
```

### Parameters

**statementInList** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**newStatements** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

### Returns

[StatementListInfo](../index.md)

