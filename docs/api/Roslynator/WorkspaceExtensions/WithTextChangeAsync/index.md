---
sidebar_label: WithTextChangeAsync
---

# WorkspaceExtensions\.WithTextChangeAsync\(Document, TextChange, CancellationToken\) Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

  
Creates a new document updated with the specified text change\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> WithTextChangeAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.Text.TextChange textChange, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**textChange** &ensp; [TextChange](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textchange)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

