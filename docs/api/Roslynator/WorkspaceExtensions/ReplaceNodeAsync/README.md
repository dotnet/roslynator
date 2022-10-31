# WorkspaceExtensions\.ReplaceNodeAsync Method

[Home](../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceNodeAsync(Document, SyntaxNode, IEnumerable\<SyntaxNode\>, CancellationToken)](#2800034700) | Creates a new document with the specified old node replaced with new nodes\. |
| [ReplaceNodeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](#2769549058) | Creates a new document with the specified old node replaced with a new node\. |
| [ReplaceNodeAsync\<TNode\>(Solution, TNode, TNode, CancellationToken)](#726832148) | Creates a new solution with the specified old node replaced with a new node\. |

<a id="2800034700"></a>

## ReplaceNodeAsync\(Document, SyntaxNode, IEnumerable\<SyntaxNode\>, CancellationToken\) 

  
Creates a new document with the specified old node replaced with new nodes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceNodeAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.SyntaxNode oldNode, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxNode> newNodes, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**oldNode** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)\>

<a id="2769549058"></a>

## ReplaceNodeAsync\(Document, SyntaxNode, SyntaxNode, CancellationToken\) 

  
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

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)\>

<a id="726832148"></a>

## ReplaceNodeAsync\<TNode\>\(Solution, TNode, TNode, CancellationToken\) 

  
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

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Solution](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.solution)\>

