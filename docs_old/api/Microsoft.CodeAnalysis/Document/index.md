---
sidebar_label: Document
---

# [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document) Class Extensions

| Extension Method | Summary |
| ---------------- | ------- |
| [InsertNodeAfterAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](../../Roslynator/WorkspaceExtensions/InsertNodeAfterAsync/index.md) | Creates a new document with a new node inserted after the specified node\. |
| [InsertNodeBeforeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](../../Roslynator/WorkspaceExtensions/InsertNodeBeforeAsync/index.md) | Creates a new document with a new node inserted before the specified node\. |
| [InsertNodesAfterAsync(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken)](../../Roslynator/WorkspaceExtensions/InsertNodesAfterAsync/index.md) | Creates a new document with new nodes inserted after the specified node\. |
| [InsertNodesBeforeAsync(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken)](../../Roslynator/WorkspaceExtensions/InsertNodesBeforeAsync/index.md) | Creates a new document with new nodes inserted before the specified node\. |
| [RemoveCommentsAsync(Document, CommentFilter, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/RemoveCommentsAsync/index.md#1785144339) | Creates a new document with comments of the specified kind removed\. |
| [RemoveCommentsAsync(Document, TextSpan, CommentFilter, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/RemoveCommentsAsync/index.md#2147765751) | Creates a new document with comments of the specified kind removed\. |
| [RemoveNodeAsync(Document, SyntaxNode, SyntaxRemoveOptions, CancellationToken)](../../Roslynator/WorkspaceExtensions/RemoveNodeAsync/index.md) | Creates a new document with the specified node removed\. |
| [RemoveNodesAsync(Document, IEnumerable&lt;SyntaxNode&gt;, SyntaxRemoveOptions, CancellationToken)](../../Roslynator/WorkspaceExtensions/RemoveNodesAsync/index.md) | Creates a new document with the specified nodes removed\. |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveFilter, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/RemovePreprocessorDirectivesAsync/index.md#4209258380) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/RemovePreprocessorDirectivesAsync/index.md#1657920506) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemoveRegionAsync(Document, RegionInfo, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/RemoveRegionAsync/index.md) | Creates a new document with the specified region removed\. |
| [RemoveTriviaAsync(Document, TextSpan, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/RemoveTriviaAsync/index.md) | Creates a new document with trivia inside the specified span removed\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, IEnumerable&lt;MemberDeclarationSyntax&gt;, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/ReplaceMembersAsync/index.md#3720048427) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, SyntaxList&lt;MemberDeclarationSyntax&gt;, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/ReplaceMembersAsync/index.md#2694444151) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, IEnumerable&lt;SyntaxToken&gt;, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/ReplaceModifiersAsync/index.md#2100445257) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, SyntaxTokenList, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/ReplaceModifiersAsync/index.md#624135533) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceNodeAsync(Document, SyntaxNode, IEnumerable&lt;SyntaxNode&gt;, CancellationToken)](../../Roslynator/WorkspaceExtensions/ReplaceNodeAsync/index.md#2800034700) | Creates a new document with the specified old node replaced with new nodes\. |
| [ReplaceNodeAsync(Document, SyntaxNode, SyntaxNode, CancellationToken)](../../Roslynator/WorkspaceExtensions/ReplaceNodeAsync/index.md#2769549058) | Creates a new document with the specified old node replaced with a new node\. |
| [ReplaceNodesAsync&lt;TNode&gt;(Document, IEnumerable&lt;TNode&gt;, Func&lt;TNode, TNode, SyntaxNode&gt;, CancellationToken)](../../Roslynator/WorkspaceExtensions/ReplaceNodesAsync/index.md#3390405393) | Creates a new document with the specified old nodes replaced with new nodes\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, IEnumerable&lt;StatementSyntax&gt;, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/ReplaceStatementsAsync/index.md#1112725449) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, SyntaxList&lt;StatementSyntax&gt;, CancellationToken)](../../Roslynator/CSharp/WorkspaceExtensions/ReplaceStatementsAsync/index.md#1837521881) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceTokenAsync(Document, SyntaxToken, IEnumerable&lt;SyntaxToken&gt;, CancellationToken)](../../Roslynator/WorkspaceExtensions/ReplaceTokenAsync/index.md#2405049151) | Creates a new document with the specified old token replaced with new tokens\. |
| [ReplaceTokenAsync(Document, SyntaxToken, SyntaxToken, CancellationToken)](../../Roslynator/WorkspaceExtensions/ReplaceTokenAsync/index.md#2782180799) | Creates a new document with the specified old token replaced with a new token\. |
| [ReplaceTriviaAsync(Document, SyntaxTrivia, IEnumerable&lt;SyntaxTrivia&gt;, CancellationToken)](../../Roslynator/WorkspaceExtensions/ReplaceTriviaAsync/index.md#3069294243) | Creates a new document with the specified old trivia replaced with a new trivia\. |
| [ReplaceTriviaAsync(Document, SyntaxTrivia, SyntaxTrivia, CancellationToken)](../../Roslynator/WorkspaceExtensions/ReplaceTriviaAsync/index.md#4172355089) | Creates a new document with the specified old trivia replaced with a new trivia\. |
| [WithTextChangeAsync(Document, TextChange, CancellationToken)](../../Roslynator/WorkspaceExtensions/WithTextChangeAsync/index.md) | Creates a new document updated with the specified text change\. |
| [WithTextChangesAsync(Document, IEnumerable&lt;TextChange&gt;, CancellationToken)](../../Roslynator/WorkspaceExtensions/WithTextChangesAsync/index.md#2083710782) | Creates a new document updated with the specified text changes\. |
| [WithTextChangesAsync(Document, TextChange\[\], CancellationToken)](../../Roslynator/WorkspaceExtensions/WithTextChangesAsync/index.md#4270127073) | Creates a new document updated with the specified text changes\. |

