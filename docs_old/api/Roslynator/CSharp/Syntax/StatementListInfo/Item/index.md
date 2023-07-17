---
sidebar_label: Item[]
---

# StatementListInfo\.Item\[Int32\] Indexer

**Containing Type**: [StatementListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Gets the statement at the specified index in the list\.

```csharp
public Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax this[int index] { get; }
```

### Parameters

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

The zero\-based index of the statement to get\. 

### Property Value

[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

The statement at the specified index in the list\.

### Implements

* [IReadOnlyList&lt;StatementSyntax&gt;.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)
