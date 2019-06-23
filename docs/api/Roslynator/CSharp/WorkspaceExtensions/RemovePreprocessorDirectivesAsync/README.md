# WorkspaceExtensions\.RemovePreprocessorDirectivesAsync Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveFilter, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken)](#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |

## RemovePreprocessorDirectivesAsync\(Document, PreprocessorDirectiveFilter, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_"></a>

\
Creates a new document with preprocessor directives of the specified kind removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemovePreprocessorDirectivesAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.PreprocessorDirectiveFilter directiveFilter, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**directiveFilter** &ensp; [PreprocessorDirectiveFilter](../../PreprocessorDirectiveFilter/README.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

## RemovePreprocessorDirectivesAsync\(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken\) <a id="Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_PreprocessorDirectiveFilter_System_Threading_CancellationToken_"></a>

\
Creates a new document with preprocessor directives of the specified kind removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemovePreprocessorDirectivesAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.Text.TextSpan span, Roslynator.CSharp.PreprocessorDirectiveFilter directiveFilter, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**directiveFilter** &ensp; [PreprocessorDirectiveFilter](../../PreprocessorDirectiveFilter/README.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)\<[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)>

