---
sidebar_label: ReplaceNodeAsync
---

# WorkspaceExtensions\.ReplaceNodeAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceNodeAsync(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken)](#2800034700) | Creates a new document with the specified old node replaced with new nodes\. |
| [ReplaceNodeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](#2769549058) | Creates a new document with the specified old node replaced with a new node\. |
| [ReplaceNodeAsync&lt;TNode&gt;(Solution, TNode, TNode, CancellationToken)](#726832148) | Creates a new solution with the specified old node replaced with a new node\. |

<a id="2800034700"></a>

## ReplaceNodeAsync\(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken\) 

  
Creates a new document with the specified old node replaced with new nodes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceNodeAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.SyntaxNode oldNode, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxNode> newNodes, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**oldNode** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

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

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="726832148"></a>

## ReplaceNodeAsync&lt;TNode&gt;\(Solution, TNode, TNode, CancellationToken\) 

  
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

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Solution](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.solution)&gt;

