# WorkspaceExtensions\.ReplaceNodesAsync Method

[Home](../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceNodesAsync\<TNode\>(Document, IEnumerable\<TNode\>, Func\<TNode, TNode, SyntaxNode\>, CancellationToken)](#3390405393) | Creates a new document with the specified old nodes replaced with new nodes\. |
| [ReplaceNodesAsync\<TNode\>(Solution, IEnumerable\<TNode\>, Func\<TNode, TNode, SyntaxNode\>, CancellationToken)](#3829645159) | Creates a new solution with the specified old nodes replaced with new nodes\. |

<a id="3390405393"></a>

## ReplaceNodesAsync\<TNode\>\(Document, IEnumerable\<TNode\>, Func\<TNode, TNode, SyntaxNode\>, CancellationToken\) 

  
Creates a new document with the specified old nodes replaced with new nodes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceNodesAsync<TNode>(this Microsoft.CodeAnalysis.Document document, System.Collections.Generic.IEnumerable<TNode> nodes, Func<TNode, TNode, Microsoft.CodeAnalysis.SyntaxNode> computeReplacementNode, System.Threading.CancellationToken cancellationToken = default) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**nodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<TNode\>

**computeReplacementNode** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-3)\<TNode, TNode, [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)\>

<a id="3829645159"></a>

## ReplaceNodesAsync\<TNode\>\(Solution, IEnumerable\<TNode\>, Func\<TNode, TNode, SyntaxNode\>, CancellationToken\) 

  
Creates a new solution with the specified old nodes replaced with new nodes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Solution> ReplaceNodesAsync<TNode>(this Microsoft.CodeAnalysis.Solution solution, System.Collections.Generic.IEnumerable<TNode> nodes, Func<TNode, TNode, Microsoft.CodeAnalysis.SyntaxNode> computeReplacementNodes, System.Threading.CancellationToken cancellationToken = default) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**solution** &ensp; [Solution](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.solution)

**nodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<TNode\>

**computeReplacementNodes** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-3)\<TNode, TNode, [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Solution](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.solution)\>

