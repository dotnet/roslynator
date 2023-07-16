---
sidebar_label: WorkspaceExtensions
---

# WorkspaceExtensions Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
A set of extension methods for the workspace layer\.

```csharp
public static class WorkspaceExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [RemoveCommentsAsync(Document, CommentFilter, CancellationToken)](RemoveCommentsAsync/index.md#1785144339) | Creates a new document with comments of the specified kind removed\. |
| [RemoveCommentsAsync(Document, TextSpan, CommentFilter, CancellationToken)](RemoveCommentsAsync/index.md#2147765751) | Creates a new document with comments of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/index.md#4209258380) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/index.md#1657920506) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemoveRegionAsync(Document, RegionInfo, CancellationToken)](RemoveRegionAsync/index.md) | Creates a new document with the specified region removed\. |
| [RemoveTriviaAsync(Document, TextSpan, CancellationToken)](RemoveTriviaAsync/index.md) | Creates a new document with trivia inside the specified span removed\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, IEnumerable&lt;MemberDeclarationSyntax&gt;, CancellationToken)](ReplaceMembersAsync/index.md#3720048427) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, SyntaxList&lt;MemberDeclarationSyntax&gt;, CancellationToken)](ReplaceMembersAsync/index.md#2694444151) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, IEnumerable&lt;SyntaxToken&gt;, CancellationToken)](ReplaceModifiersAsync/index.md#2100445257) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, SyntaxTokenList, CancellationToken)](ReplaceModifiersAsync/index.md#624135533) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, IEnumerable&lt;StatementSyntax&gt;, CancellationToken)](ReplaceStatementsAsync/index.md#1112725449) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, SyntaxList&lt;StatementSyntax&gt;, CancellationToken)](ReplaceStatementsAsync/index.md#1837521881) | Creates a new document with the specified statements replaced with new statements\. |

