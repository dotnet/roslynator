---
sidebar_label: WorkspaceExtensions
---

# WorkspaceExtensions Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

  
A set of extension methods for the workspace layer\.

```csharp
public static class WorkspaceExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [InsertNodeAfterAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](InsertNodeAfterAsync/index.md) | Creates a new document with a new node inserted after the specified node\. |
| [InsertNodeBeforeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](InsertNodeBeforeAsync/index.md) | Creates a new document with a new node inserted before the specified node\. |
| [InsertNodesAfterAsync(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken)](InsertNodesAfterAsync/index.md) | Creates a new document with new nodes inserted after the specified node\. |
| [InsertNodesBeforeAsync(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken)](InsertNodesBeforeAsync/index.md) | Creates a new document with new nodes inserted before the specified node\. |
| [RemoveNodeAsync(Document, SyntaxNode, SyntaxRemoveOptions, CancellationToken)](RemoveNodeAsync/index.md) | Creates a new document with the specified node removed\. |
| [RemoveNodesAsync(Document, IEnumerable&lt;SyntaxNode&gt;, SyntaxRemoveOptions, CancellationToken)](RemoveNodesAsync/index.md) | Creates a new document with the specified nodes removed\. |
| [ReplaceNodeAsync(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken)](ReplaceNodeAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_) | Creates a new document with the specified old node replaced with new nodes\. |
| [ReplaceNodeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](ReplaceNodeAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_SyntaxNode_System_Threading_CancellationToken_) | Creates a new document with the specified old node replaced with a new node\. |
| [ReplaceNodeAsync&lt;TNode&gt;(Solution, TNode, TNode, CancellationToken)](ReplaceNodeAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceNodeAsync__1_Microsoft_CodeAnalysis_Solution___0___0_System_Threading_CancellationToken_) | Creates a new solution with the specified old node replaced with a new node\. |
| [ReplaceNodesAsync&lt;TNode&gt;(Document, IEnumerable&lt;TNode&gt;, Func&lt;TNode, TNode, SyntaxNode&gt;, CancellationToken)](ReplaceNodesAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceNodesAsync__1_Microsoft_CodeAnalysis_Document_System_Collections_Generic_IEnumerable___0__System_Func___0___0_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_) | Creates a new document with the specified old nodes replaced with new nodes\. |
| [ReplaceNodesAsync&lt;TNode&gt;(Solution, IEnumerable&lt;TNode&gt;, Func&lt;TNode, TNode, SyntaxNode&gt;, CancellationToken)](ReplaceNodesAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceNodesAsync__1_Microsoft_CodeAnalysis_Solution_System_Collections_Generic_IEnumerable___0__System_Func___0___0_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_) | Creates a new solution with the specified old nodes replaced with new nodes\. |
| [ReplaceTokenAsync(Document, SyntaxToken, IEnumerable&lt;SyntaxToken&gt;, CancellationToken)](ReplaceTokenAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceTokenAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__System_Threading_CancellationToken_) | Creates a new document with the specified old token replaced with new tokens\. |
| [ReplaceTokenAsync(Document, SyntaxToken, SyntaxToken, CancellationToken)](ReplaceTokenAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceTokenAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxToken_System_Threading_CancellationToken_) | Creates a new document with the specified old token replaced with a new token\. |
| [ReplaceTriviaAsync(Document, SyntaxTrivia, IEnumerable&lt;SyntaxTrivia&gt;, CancellationToken)](ReplaceTriviaAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceTriviaAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxTrivia_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__System_Threading_CancellationToken_) | Creates a new document with the specified old trivia replaced with a new trivia\. |
| [ReplaceTriviaAsync(Document, SyntaxTrivia, SyntaxTrivia, CancellationToken)](ReplaceTriviaAsync/index.md#Roslynator_WorkspaceExtensions_ReplaceTriviaAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_SyntaxTrivia_System_Threading_CancellationToken_) | Creates a new document with the specified old trivia replaced with a new trivia\. |
| [WithTextChangeAsync(Document, TextChange, CancellationToken)](WithTextChangeAsync/index.md) | Creates a new document updated with the specified text change\. |
| [WithTextChangesAsync(Document, IEnumerable&lt;TextChange&gt;, CancellationToken)](WithTextChangesAsync/index.md#Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Text_TextChange__System_Threading_CancellationToken_) | Creates a new document updated with the specified text changes\. |
| [WithTextChangesAsync(Document, TextChange\[\], CancellationToken)](WithTextChangesAsync/index.md#Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextChange___System_Threading_CancellationToken_) | Creates a new document updated with the specified text changes\. |
