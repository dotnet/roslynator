# WorkspaceExtensions Class

[Home](../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator](../README.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

\
A set of extension methods for the workspace layer\.

```csharp
public static class WorkspaceExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [InsertNodeAfterAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](InsertNodeAfterAsync/README.md) | Creates a new document with a new node inserted after the specified node\. |
| [InsertNodeBeforeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](InsertNodeBeforeAsync/README.md) | Creates a new document with a new node inserted before the specified node\. |
| [InsertNodesAfterAsync(Document, SyntaxNode, IEnumerable\<SyntaxNode>, CancellationToken)](InsertNodesAfterAsync/README.md) | Creates a new document with new nodes inserted after the specified node\. |
| [InsertNodesBeforeAsync(Document, SyntaxNode, IEnumerable\<SyntaxNode>, CancellationToken)](InsertNodesBeforeAsync/README.md) | Creates a new document with new nodes inserted before the specified node\. |
| [RemoveNodeAsync(Document, SyntaxNode, SyntaxRemoveOptions, CancellationToken)](RemoveNodeAsync/README.md) | Creates a new document with the specified node removed\. |
| [RemoveNodesAsync(Document, IEnumerable\<SyntaxNode>, SyntaxRemoveOptions, CancellationToken)](RemoveNodesAsync/README.md) | Creates a new document with the specified nodes removed\. |
| [ReplaceNodeAsync(Document, SyntaxNode, IEnumerable\<SyntaxNode>, CancellationToken)](ReplaceNodeAsync/README.md#Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_) | Creates a new document with the specified old node replaced with new nodes\. |
| [ReplaceNodeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](ReplaceNodeAsync/README.md#Roslynator_WorkspaceExtensions_ReplaceNodeAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_SyntaxNode_System_Threading_CancellationToken_) | Creates a new document with the specified old node replaced with a new node\. |
| [ReplaceNodeAsync\<TNode>(Solution, TNode, TNode, CancellationToken)](ReplaceNodeAsync-1/README.md#Roslynator_WorkspaceExtensions_ReplaceNodeAsync__1_Microsoft_CodeAnalysis_Solution___0___0_System_Threading_CancellationToken_) | Creates a new solution with the specified old node replaced with a new node\. |
| [ReplaceNodesAsync\<TNode>(Document, IEnumerable\<TNode>, Func\<TNode, TNode, SyntaxNode>, CancellationToken)](ReplaceNodesAsync-1/README.md#Roslynator_WorkspaceExtensions_ReplaceNodesAsync__1_Microsoft_CodeAnalysis_Document_System_Collections_Generic_IEnumerable___0__System_Func___0___0_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_) | Creates a new document with the specified old nodes replaced with new nodes\. |
| [ReplaceNodesAsync\<TNode>(Solution, IEnumerable\<TNode>, Func\<TNode, TNode, SyntaxNode>, CancellationToken)](ReplaceNodesAsync-1/README.md#Roslynator_WorkspaceExtensions_ReplaceNodesAsync__1_Microsoft_CodeAnalysis_Solution_System_Collections_Generic_IEnumerable___0__System_Func___0___0_Microsoft_CodeAnalysis_SyntaxNode__System_Threading_CancellationToken_) | Creates a new solution with the specified old nodes replaced with new nodes\. |
| [ReplaceTokenAsync(Document, SyntaxToken, IEnumerable\<SyntaxToken>, CancellationToken)](ReplaceTokenAsync/README.md#Roslynator_WorkspaceExtensions_ReplaceTokenAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__System_Threading_CancellationToken_) | Creates a new document with the specified old token replaced with new tokens\. |
| [ReplaceTokenAsync(Document, SyntaxToken, SyntaxToken, CancellationToken)](ReplaceTokenAsync/README.md#Roslynator_WorkspaceExtensions_ReplaceTokenAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxToken_System_Threading_CancellationToken_) | Creates a new document with the specified old token replaced with a new token\. |
| [ReplaceTriviaAsync(Document, SyntaxTrivia, IEnumerable\<SyntaxTrivia>, CancellationToken)](ReplaceTriviaAsync/README.md#Roslynator_WorkspaceExtensions_ReplaceTriviaAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxTrivia_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__System_Threading_CancellationToken_) | Creates a new document with the specified old trivia replaced with a new trivia\. |
| [ReplaceTriviaAsync(Document, SyntaxTrivia, SyntaxTrivia, CancellationToken)](ReplaceTriviaAsync/README.md#Roslynator_WorkspaceExtensions_ReplaceTriviaAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_SyntaxTrivia_System_Threading_CancellationToken_) | Creates a new document with the specified old trivia replaced with a new trivia\. |
| [WithTextChangeAsync(Document, TextChange, CancellationToken)](WithTextChangeAsync/README.md) | Creates a new document updated with the specified text change\. |
| [WithTextChangesAsync(Document, IEnumerable\<TextChange>, CancellationToken)](WithTextChangesAsync/README.md#Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Text_TextChange__System_Threading_CancellationToken_) | Creates a new document updated with the specified text changes\. |
| [WithTextChangesAsync(Document, TextChange\[\], CancellationToken)](WithTextChangesAsync/README.md#Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextChange___System_Threading_CancellationToken_) | Creates a new document updated with the specified text changes\. |

