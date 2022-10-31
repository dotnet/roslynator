# WorkspaceExtensions\.ReplaceModifiersAsync Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceModifiersAsync(Document, ModifierListInfo, IEnumerable\<SyntaxToken>, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__System_Threading_CancellationToken_) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, SyntaxTokenList, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_Microsoft_CodeAnalysis_SyntaxTokenList_System_Threading_CancellationToken_) | Creates a new document with the specified modifiers replaced with new modifiers\. |

## ReplaceModifiersAsync\(Document, ModifierListInfo, IEnumerable\<SyntaxToken>, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified modifiers replaced with new modifiers\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceModifiersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.ModifierListInfo modifiersInfo, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> newModifiers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**modifiersInfo** &ensp; [ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

**newModifiers** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

## ReplaceModifiersAsync\(Document, ModifierListInfo, SyntaxTokenList, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_ReplaceModifiersAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_Syntax_ModifierListInfo_Microsoft_CodeAnalysis_SyntaxTokenList_System_Threading_CancellationToken_"></a>

\
Creates a new document with the specified modifiers replaced with new modifiers\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceModifiersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.ModifierListInfo modifiersInfo, Microsoft.CodeAnalysis.SyntaxTokenList newModifiers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**modifiersInfo** &ensp; [ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

**newModifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

