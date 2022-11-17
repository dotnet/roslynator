# WorkspaceExtensions Class

[Home](../../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
A set of extension methods for the workspace layer\.

```csharp
public static class WorkspaceExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [RemoveCommentsAsync(Document, CommentFilter, CancellationToken)](RemoveCommentsAsync/README.md#1785144339) | Creates a new document with comments of the specified kind removed\. |
| [RemoveCommentsAsync(Document, TextSpan, CommentFilter, CancellationToken)](RemoveCommentsAsync/README.md#2147765751) | Creates a new document with comments of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/README.md#4209258380) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/README.md#1657920506) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemoveRegionAsync(Document, RegionInfo, CancellationToken)](RemoveRegionAsync/README.md) | Creates a new document with the specified region removed\. |
| [RemoveTriviaAsync(Document, TextSpan, CancellationToken)](RemoveTriviaAsync/README.md) | Creates a new document with trivia inside the specified span removed\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, IEnumerable\<MemberDeclarationSyntax\>, CancellationToken)](ReplaceMembersAsync/README.md#3720048427) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, SyntaxList\<MemberDeclarationSyntax\>, CancellationToken)](ReplaceMembersAsync/README.md#2694444151) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, IEnumerable\<SyntaxToken\>, CancellationToken)](ReplaceModifiersAsync/README.md#2100445257) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, SyntaxTokenList, CancellationToken)](ReplaceModifiersAsync/README.md#624135533) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, IEnumerable\<StatementSyntax\>, CancellationToken)](ReplaceStatementsAsync/README.md#1112725449) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, SyntaxList\<StatementSyntax\>, CancellationToken)](ReplaceStatementsAsync/README.md#1837521881) | Creates a new document with the specified statements replaced with new statements\. |

