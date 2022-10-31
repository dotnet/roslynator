# WorkspaceExtensions\.ReplaceMembersAsync Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, IEnumerable\<MemberDeclarationSyntax>, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, SyntaxList\<MemberDeclarationSyntax>, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_) | Creates a new document with the specified members replaced with new members\. |

## ReplaceMembersAsync\(Document, MemberDeclarationListInfo, IEnumerable\<MemberDeclarationSyntax>, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified members replaced with new members\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceMembersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.MemberDeclarationListInfo info, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> newMembers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**info** &ensp; [MemberDeclarationListInfo](../../Syntax/MemberDeclarationListInfo/README.md)

**newMembers** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

## ReplaceMembersAsync\(Document, MemberDeclarationListInfo, SyntaxList\<MemberDeclarationSyntax>, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_ReplaceMembersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_MemberDeclarationListInfo_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified members replaced with new members\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceMembersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.MemberDeclarationListInfo info, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> newMembers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**info** &ensp; [MemberDeclarationListInfo](../../Syntax/MemberDeclarationListInfo/README.md)

**newMembers** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

