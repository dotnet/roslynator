# SyntaxExtensions\.TrimTrivia Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [TrimTrivia(SyntaxToken)](../TrimTrivia/README.md#Roslynator_CSharp_SyntaxExtensions_TrimTrivia_Microsoft_CodeAnalysis_SyntaxToken_) | Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new token with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same token if there is nothing to trim\. |
| [TrimTrivia\<TNode>(SeparatedSyntaxList\<TNode>)](#Roslynator_CSharp_SyntaxExtensions_TrimTrivia__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [TrimTrivia\<TNode>(SyntaxList\<TNode>)](#Roslynator_CSharp_SyntaxExtensions_TrimTrivia__1_Microsoft_CodeAnalysis_SyntaxList___0__) | Removes all leading whitespace from the leading trivia of the first node in a list and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. |
| [TrimTrivia\<TNode>(TNode)](#Roslynator_CSharp_SyntaxExtensions_TrimTrivia__1___0_) | Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new node with the new trivia\. [SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\. Returns the same node if there is nothing to trim\. |

## TrimTrivia\(SyntaxToken\) <a id="Roslynator_CSharp_SyntaxExtensions_TrimTrivia_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new token with the new trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.
Returns the same token if there is nothing to trim\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken TrimTrivia(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## TrimTrivia\<TNode>\(SeparatedSyntaxList\<TNode>\) <a id="Roslynator_CSharp_SyntaxExtensions_TrimTrivia__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__"></a>

\
Removes all leading whitespace from the leading trivia of the first node in a list
and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> TrimTrivia<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

## TrimTrivia\<TNode>\(SyntaxList\<TNode>\) <a id="Roslynator_CSharp_SyntaxExtensions_TrimTrivia__1_Microsoft_CodeAnalysis_SyntaxList___0__"></a>

\
Removes all leading whitespace from the leading trivia of the first node in a list
and all trailing whitespace from the trailing trivia of the last node in a list and returns a new list with the new trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> TrimTrivia<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

## TrimTrivia\<TNode>\(TNode\) <a id="Roslynator_CSharp_SyntaxExtensions_TrimTrivia__1___0_"></a>

\
Removes all leading whitespace from the leading trivia and all trailing whitespace from the trailing trivia and returns a new node with the new trivia\.
[SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) and [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia) is considered to be a whitespace\.
Returns the same node if there is nothing to trim\.

```csharp
public static TNode TrimTrivia<TNode>(this TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

### Returns

TNode

