# WorkspaceExtensions\.WithTextChangesAsync Method

[Home](../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithTextChangesAsync(Document, IEnumerable\<TextChange>, CancellationToken)](#Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Text_TextChange__System_Threading_CancellationToken_) | Creates a new document updated with the specified text changes\. |
| [WithTextChangesAsync(Document, TextChange\[\], CancellationToken)](#Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextChange___System_Threading_CancellationToken_) | Creates a new document updated with the specified text changes\. |

## WithTextChangesAsync\(Document, IEnumerable\<TextChange>, CancellationToken\) <a id="Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Text_TextChange__System_Threading_CancellationToken_"></a>

\
Creates a new document updated with the specified text changes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> WithTextChangesAsync(this Microsoft.CodeAnalysis.Document document, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Text.TextChange> textChanges, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**textChanges** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[TextChange](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textchange)>

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

## WithTextChangesAsync\(Document, TextChange\[\], CancellationToken\) <a id="Roslynator_WorkspaceExtensions_WithTextChangesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextChange___System_Threading_CancellationToken_"></a>

\
Creates a new document updated with the specified text changes\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> WithTextChangesAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.Text.TextChange[] textChanges, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**textChanges** &ensp; [TextChange](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textchange)\[\]

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

