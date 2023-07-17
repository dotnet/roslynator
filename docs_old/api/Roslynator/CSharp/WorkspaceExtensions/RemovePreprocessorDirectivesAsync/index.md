---
sidebar_label: RemovePreprocessorDirectivesAsync
---

# WorkspaceExtensions\.RemovePreprocessorDirectivesAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveFilter, CancellationToken)](#4209258380) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken)](#1657920506) | Creates a new document with preprocessor directives of the specified kind removed\. |

<a id="4209258380"></a>

## RemovePreprocessorDirectivesAsync\(Document, PreprocessorDirectiveFilter, CancellationToken\) 

  
Creates a new document with preprocessor directives of the specified kind removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemovePreprocessorDirectivesAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.PreprocessorDirectiveFilter directiveFilter, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**directiveFilter** &ensp; [PreprocessorDirectiveFilter](../../PreprocessorDirectiveFilter/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="1657920506"></a>

## RemovePreprocessorDirectivesAsync\(Document, TextSpan, PreprocessorDirectiveFilter, CancellationToken\) 

  
Creates a new document with preprocessor directives of the specified kind removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemovePreprocessorDirectivesAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.Text.TextSpan span, Roslynator.CSharp.PreprocessorDirectiveFilter directiveFilter, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**directiveFilter** &ensp; [PreprocessorDirectiveFilter](../../PreprocessorDirectiveFilter/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

