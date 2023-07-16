---
sidebar_label: WithTextChangesAsync
---

# WorkspaceExtensions\.WithTextChangesAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithTextChangesAsync(Document, IEnumerable&lt;TextChange&gt;, CancellationToken)](#2083710782) | Creates a new document updated with the specified text changes\. |
| [WithTextChangesAsync(Document, TextChange\[\], CancellationToken)](#4270127073) | Creates a new document updated with the specified text changes\. |

<a id="2083710782"></a>

## WithTextChangesAsync\(Document, IEnumerable&lt;TextChange&gt;, CancellationToken\) 

  
Creates a new document updated with the specified text changes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> WithTextChangesAsync(this Microsoft.CodeAnalysis.Document document, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Text.TextChange> textChanges, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**textChanges** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[TextChange](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textchange)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="4270127073"></a>

## WithTextChangesAsync\(Document, TextChange\[\], CancellationToken\) 

  
Creates a new document updated with the specified text changes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> WithTextChangesAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.Text.TextChange[] textChanges, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**textChanges** &ensp; [TextChange](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textchange)\[\]

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

