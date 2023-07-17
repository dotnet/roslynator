---
sidebar_label: RemoveCommentsAsync
---

# WorkspaceExtensions\.RemoveCommentsAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveCommentsAsync(Document, CommentFilter, CancellationToken)](#1785144339) | Creates a new document with comments of the specified kind removed\. |
| [RemoveCommentsAsync(Document, TextSpan, CommentFilter, CancellationToken)](#2147765751) | Creates a new document with comments of the specified kind removed\. |

<a id="1785144339"></a>

## RemoveCommentsAsync\(Document, CommentFilter, CancellationToken\) 

  
Creates a new document with comments of the specified kind removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemoveCommentsAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.CommentFilter comments, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**comments** &ensp; [CommentFilter](../../CommentFilter/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="2147765751"></a>

## RemoveCommentsAsync\(Document, TextSpan, CommentFilter, CancellationToken\) 

  
Creates a new document with comments of the specified kind removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemoveCommentsAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.Text.TextSpan span, Roslynator.CSharp.CommentFilter comments, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**comments** &ensp; [CommentFilter](../../CommentFilter/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

