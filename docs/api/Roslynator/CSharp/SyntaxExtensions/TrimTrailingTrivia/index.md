---
sidebar_label: TrimTrailingTrivia
---

# SyntaxExtensions\.TrimTrailingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimTrailingTrivia/index.md
| [TrimTrailingTrivia(SyntaxToken)](#Roslynator_CSharp_SyntaxExtensions_TrimTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_) | Removes all trailing whitespace from the trailing trivia and returns a new token with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimTrailingTrivia&lt;TNode&gt;(TNode)](#Roslynator_CSharp_SyntaxExtensions_TrimTrailingTrivia__1___0_) | Removes all trailing whitespace from the trailing trivia and returns a new node with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
========
| [TrimTrailingTrivia(SyntaxToken)](#3749666890) | Removes all trailing whitespace from the trailing trivia and returns a new token with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimTrailingTrivia\<TNode\>(TNode)](#2450702114) | Removes all trailing whitespace from the trailing trivia and returns a new node with the new trailing trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimTrailingTrivia/README.md

<a id="3749666890"></a>

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimTrailingTrivia/index.md
========
## TrimTrailingTrivia\(SyntaxToken\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimTrailingTrivia/README.md
  
Removes all trailing whitespace from the trailing trivia and returns a new token with the new trailing trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.
Returns the same token if there is nothing to trim\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken TrimTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimTrailingTrivia/index.md
## TrimTrailingTrivia&lt;TNode&gt;\(TNode\) <a id="Roslynator_CSharp_SyntaxExtensions_TrimTrailingTrivia__1___0_"></a>

========
<a id="2450702114"></a>

## TrimTrailingTrivia\<TNode\>\(TNode\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimTrailingTrivia/README.md
  
Removes all trailing whitespace from the trailing trivia and returns a new node with the new trailing trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.
Returns the same node if there is nothing to trim\.

```csharp
public static TNode TrimTrailingTrivia<TNode>(this TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

### Returns

TNode

