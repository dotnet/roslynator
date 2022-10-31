# WorkspaceExtensions\.ReplaceNodeAsync Method

[Home](../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceNodeAsync(Document, SyntaxNode, IEnumerable\<SyntaxNode>, CancellationToken)](#Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_) | Creates a new document with the specified old node replaced with new nodes\. |
| [ReplaceNodeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](#Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_SyntaxNode_System_Threading_CancellationToken_) | Creates a new document with the specified old node replaced with a new node\. |
| [ReplaceNodeAsync\<TNode>(Solution, TNode, TNode, CancellationToken)](../ReplaceNodeAsync-1/README.md#Roslynator_WorkspaceExtensions_ReplaceNodeAsync__1_Microsoft_CodeAnalysis_Solution___0___0_System_Threading_CancellationToken_) | Creates a new solution with the specified old node replaced with a new node\. |

## ReplaceNodeAsync\(Document, SyntaxNode, IEnumerable\<SyntaxNode>, CancellationToken\) <a id="Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified old node replaced with new nodes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceNodeAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.SyntaxNode oldNode, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxNode> newNodes, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**oldNode** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

## ReplaceNodeAsync\(Document, SyntaxNode, SyntaxNode, CancellationToken\) <a id="Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_SyntaxNode_System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified old node replaced with a new node\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceNodeAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.SyntaxNode oldNode, Microsoft.CodeAnalysis.SyntaxNode newNode, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**oldNode** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**newNode** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

## ReplaceNodeAsync\<TNode>\(Solution, TNode, TNode, CancellationToken\) <a id="Roslynator_WorkspaceExtensions_ReplaceNodeAsync__1_Microsoft_CodeAnalysis_Solution___0___0_System_Threading_CancellationToken_"></a>

\
Creates a new solution with the specified old node replaced with a new node\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Solution> ReplaceNodeAsync<TNode>(this Microsoft.CodeAnalysis.Solution solution, TNode oldNode, TNode newNode, System.Threading.CancellationToken cancellationToken = default) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**solution** &ensp; [Solution](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.solution)

**oldNode** &ensp; TNode

**newNode** &ensp; TNode

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Solution](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.solution)>

