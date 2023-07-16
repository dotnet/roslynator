---
sidebar_label: ReplaceStatementsAsync
---

# WorkspaceExtensions\.ReplaceStatementsAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceStatementsAsync(Document, StatementListInfo, IEnumerable&lt;StatementSyntax&gt;, CancellationToken)](#1112725449) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, SyntaxList&lt;StatementSyntax&gt;, CancellationToken)](#1837521881) | Creates a new document with the specified statements replaced with new statements\. |

<a id="1112725449"></a>

## ReplaceStatementsAsync\(Document, StatementListInfo, IEnumerable&lt;StatementSyntax&gt;, CancellationToken\) 

  
Creates a new document with the specified statements replaced with new statements\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceStatementsAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.StatementListInfo statementsInfo, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> newStatements, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**statementsInfo** &ensp; [StatementListInfo](../../Syntax/StatementListInfo/index.md)

**newStatements** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="1837521881"></a>

## ReplaceStatementsAsync\(Document, StatementListInfo, SyntaxList&lt;StatementSyntax&gt;, CancellationToken\) 

  
Creates a new document with the specified statements replaced with new statements\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceStatementsAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.StatementListInfo statementsInfo, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> newStatements, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**statementsInfo** &ensp; [StatementListInfo](../../Syntax/StatementListInfo/index.md)

**newStatements** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

