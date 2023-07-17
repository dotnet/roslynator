---
sidebar_label: ReplaceTokenAsync
---

# WorkspaceExtensions\.ReplaceTokenAsync Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.Workspaces\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceTokenAsync(Document, SyntaxToken, IEnumerable&lt;SyntaxToken&gt;, CancellationToken)](#2405049151) | Creates a new document with the specified old token replaced with new tokens\. |
| [ReplaceTokenAsync(Document, SyntaxToken, SyntaxToken, CancellationToken)](#2782180799) | Creates a new document with the specified old token replaced with a new token\. |

<a id="2405049151"></a>

## ReplaceTokenAsync\(Document, SyntaxToken, IEnumerable&lt;SyntaxToken&gt;, CancellationToken\) 

  
Creates a new document with the specified old token replaced with new tokens\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceTokenAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.SyntaxToken oldToken, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> newTokens, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**oldToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**newTokens** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)&gt;

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

<a id="2782180799"></a>

## ReplaceTokenAsync\(Document, SyntaxToken, SyntaxToken, CancellationToken\) 

  
Creates a new document with the specified old token replaced with a new token\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> ReplaceTokenAsync(this Microsoft.CodeAnalysis.Document document, Microsoft.CodeAnalysis.SyntaxToken oldToken, Microsoft.CodeAnalysis.SyntaxToken newToken, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**oldToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**newToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

