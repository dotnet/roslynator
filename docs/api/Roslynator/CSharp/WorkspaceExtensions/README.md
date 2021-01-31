# WorkspaceExtensions Class

[Home](../../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

\
A set of extension methods for the workspace layer\.

```csharp
public static class WorkspaceExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [RemoveCommentsAsync(Document, CommentFilter, CancellationToken)](RemoveCommentsAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemoveCommentsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_CommentFilter_System_Threading_CancellationToken_) | Creates a new document with comments of the specified kind removed\. |
| [RemoveCommentsAsync(Document, TextSpan, CommentFilter, CancellationToken)](RemoveCommentsAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemoveCommentsAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_CommentFilter_System_Threading_CancellationToken_) | Creates a new document with comments of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemoveRegionAsync(Document, RegionInfo, CancellationToken)](RemoveRegionAsync/README.md) | Creates a new document with the specified region removed\. |
| [RemoveTriviaAsync(Document, TextSpan, CancellationToken)](RemoveTriviaAsync/README.md) | Creates a new document with trivia inside the specified span removed\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, IEnumerable\<MemberDeclarationSyntax>, CancellationToken)](ReplaceMembersAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, SyntaxList\<MemberDeclarationSyntax>, CancellationToken)](ReplaceMembersAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, IEnumerable\<SyntaxToken>, CancellationToken)](ReplaceModifiersAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__System_Threading_CancellationToken_) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, SyntaxTokenList, CancellationToken)](ReplaceModifiersAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_Microsoft_CodeAnalysis_SyntaxTokenList_System_Threading_CancellationToken_) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, IEnumerable\<StatementSyntax>, CancellationToken)](ReplaceStatementsAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, SyntaxList\<StatementSyntax>, CancellationToken)](ReplaceStatementsAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified statements replaced with new statements\. |

