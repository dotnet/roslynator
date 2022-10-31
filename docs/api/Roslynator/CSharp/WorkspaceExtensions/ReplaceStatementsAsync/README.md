# WorkspaceExtensions\.ReplaceStatementsAsync Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceStatementsAsync(Document, StatementListInfo, IEnumerable\<StatementSyntax>, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, SyntaxList\<StatementSyntax>, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified statements replaced with new statements\. |

## ReplaceStatementsAsync\(Document, StatementListInfo, IEnumerable\<StatementSyntax>, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified statements replaced with new statements\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceStatementsAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.StatementListInfo statementsInfo, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> newStatements, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**statementsInfo** &ensp; [StatementListInfo](../../Syntax/StatementListInfo/README.md)

**newStatements** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

## ReplaceStatementsAsync\(Document, StatementListInfo, SyntaxList\<StatementSyntax>, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified statements replaced with new statements\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceStatementsAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.StatementListInfo statementsInfo, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> newStatements, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**statementsInfo** &ensp; [StatementListInfo](../../Syntax/StatementListInfo/README.md)

**newStatements** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

