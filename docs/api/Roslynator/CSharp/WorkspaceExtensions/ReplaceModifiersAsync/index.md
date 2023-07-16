---
sidebar_label: ReplaceModifiersAsync
---

# WorkspaceExtensions\.ReplaceModifiersAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceModifiersAsync(Document, ModifierListInfo, IEnumerable&lt;SyntaxToken&gt;, CancellationToken)](#2100445257) | Creates a new document with the specified modifiers replaced with new modifiers\. |
| [ReplaceModifiersAsync(Document, ModifierListInfo, SyntaxTokenList, CancellationToken)](#624135533) | Creates a new document with the specified modifiers replaced with new modifiers\. |

<a id="2100445257"></a>

## ReplaceModifiersAsync\(Document, ModifierListInfo, IEnumerable&lt;SyntaxToken&gt;, CancellationToken\) 

  
Creates a new document with the specified modifiers replaced with new modifiers\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceModifiersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.ModifierListInfo modifiersInfo, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> newModifiers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**modifiersInfo** &ensp; [ModifierListInfo](../../Syntax/ModifierListInfo/index.md)

**newModifiers** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="624135533"></a>

## ReplaceModifiersAsync\(Document, ModifierListInfo, SyntaxTokenList, CancellationToken\) 

  
Creates a new document with the specified modifiers replaced with new modifiers\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceModifiersAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.ModifierListInfo modifiersInfo, Microsoft.CodeAnalysis.SyntaxTokenList newModifiers, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**modifiersInfo** &ensp; [ModifierListInfo](../../Syntax/ModifierListInfo/index.md)

**newModifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

