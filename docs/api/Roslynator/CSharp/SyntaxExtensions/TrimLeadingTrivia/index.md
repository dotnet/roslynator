---
sidebar_label: TrimLeadingTrivia
---

# SyntaxExtensions\.TrimLeadingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimLeadingTrivia/index.md
| [TrimLeadingTrivia(SyntaxToken)](#Roslynator_CSharp_SyntaxExtensions_TrimLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_) | Removes all leading whitespace from the leading trivia and returns a new token with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimLeadingTrivia&lt;TNode&gt;(TNode)](#Roslynator_CSharp_SyntaxExtensions_TrimLeadingTrivia__1___0_) | Removes all leading whitespace from the leading trivia and returns a new node with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
========
| [TrimLeadingTrivia(SyntaxToken)](#1084780771) | Removes all leading whitespace from the leading trivia and returns a new token with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimLeadingTrivia\<TNode\>(TNode)](#1018285907) | Removes all leading whitespace from the leading trivia and returns a new node with the new leading trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimLeadingTrivia/README.md

<a id="1084780771"></a>

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimLeadingTrivia/index.md
========
## TrimLeadingTrivia\(SyntaxToken\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimLeadingTrivia/README.md
  
Removes all leading whitespace from the leading trivia and returns a new token with the new leading trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.
Returns the same token if there is nothing to trim\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken TrimLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimLeadingTrivia/index.md
## TrimLeadingTrivia&lt;TNode&gt;\(TNode\) <a id="Roslynator_CSharp_SyntaxExtensions_TrimLeadingTrivia__1___0_"></a>

========
<a id="1018285907"></a>

## TrimLeadingTrivia\<TNode\>\(TNode\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/TrimLeadingTrivia/README.md
  
Removes all leading whitespace from the leading trivia and returns a new node with the new leading trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.
Returns the same node if there is nothing to trim\.

```csharp
public static TNode TrimLeadingTrivia<TNode>(this TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

### Returns

TNode

