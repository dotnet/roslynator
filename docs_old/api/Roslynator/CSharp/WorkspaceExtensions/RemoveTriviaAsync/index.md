---
sidebar_label: RemoveTriviaAsync
---

# WorkspaceExtensions\.RemoveTriviaAsync\(Document, TextSpan, CancellationToken\) Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
Creates a new document with trivia inside the specified span removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemoveTriviaAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.Text.TextSpan span, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

