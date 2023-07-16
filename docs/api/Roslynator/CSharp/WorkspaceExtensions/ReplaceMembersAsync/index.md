---
sidebar_label: ReplaceMembersAsync
---

# WorkspaceExtensions\.ReplaceMembersAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, IEnumerable&lt;MemberDeclarationSyntax&gt;, CancellationToken)](#3720048427) | Creates a new document with the specified members replaced with new members\. |
| [ReplaceMembersAsync(Document, MemberDeclarationListInfo, SyntaxList&lt;MemberDeclarationSyntax&gt;, CancellationToken)](#2694444151) | Creates a new document with the specified members replaced with new members\. |

<a id="3720048427"></a>

## ReplaceMembersAsync\(Document, MemberDeclarationListInfo, IEnumerable&lt;MemberDeclarationSyntax&gt;, CancellationToken\) 

  
Creates a new document with the specified members replaced with new members\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceMembersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.MemberDeclarationListInfo info, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> newMembers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**info** &ensp; [MemberDeclarationListInfo](../../Syntax/MemberDeclarationListInfo/index.md)

**newMembers** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="2694444151"></a>

## ReplaceMembersAsync\(Document, MemberDeclarationListInfo, SyntaxList&lt;MemberDeclarationSyntax&gt;, CancellationToken\) 

  
Creates a new document with the specified members replaced with new members\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceMembersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.MemberDeclarationListInfo info, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> newMembers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**info** &ensp; [MemberDeclarationListInfo](../../Syntax/MemberDeclarationListInfo/index.md)

**newMembers** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

