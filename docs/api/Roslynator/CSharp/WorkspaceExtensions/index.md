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
| [RemoveCommentsAsync(Document, CommentFilter, CancellationToken)](RemoveCommentsAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_RemoveCommentsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_CommentFilter_System_Threading_CancellationToken_) | Creates a new document with comments of the specified kind removed\. |
| [RemoveCommentsAsync(Document, TextSpan, CommentFilter, CancellationToken)](RemoveCommentsAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_RemoveCommentsAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_CommentFilter_System_Threading_CancellationToken_) | Creates a new document with comments of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken)](RemovePreprocessorDirectivesAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemoveRegionAsync(Document, RegionInfo, CancellationToken)](RemoveRegionAsync/index.md) | Creates a new document with the specified region removed\. |
| [RemoveTriviaAsync(Document, TextSpan, CancellationToken)](RemoveTriviaAsync/index.md) | Creates a new document with trivia inside the specified span removed\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, IEnumerable&lt;MemberDeclarationSyntax&gt;, CancellationToken)](ReplaceMembersAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, SyntaxList&lt;MemberDeclarationSyntax&gt;, CancellationToken)](ReplaceMembersAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, IEnumerable&lt;SyntaxToken&gt;, CancellationToken)](ReplaceModifiersAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__System_Threading_CancellationToken_) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, SyntaxTokenList, CancellationToken)](ReplaceModifiersAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_Microsoft_CodeAnalysis_SyntaxTokenList_System_Threading_CancellationToken_) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, IEnumerable&lt;StatementSyntax&gt;, CancellationToken)](ReplaceStatementsAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified statements replaced with new statements\. |
| [ReplaceStatementsAsync(Document, StatementListInfo, SyntaxList&lt;StatementSyntax&gt;, CancellationToken)](ReplaceStatementsAsync/index.md#Roslynator_CSharp_WorkspaceExtensions_ReplaceStatementsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_StatementListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified statements replaced with new statements\. |

