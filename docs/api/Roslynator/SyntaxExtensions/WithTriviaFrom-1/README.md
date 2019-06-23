# SyntaxExtensions\.WithTriviaFrom Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithTriviaFrom(SyntaxToken, SyntaxNode)](../WithTriviaFrom/README.md#Roslynator_SyntaxExtensions_WithTriviaFrom_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new token from this token with both the leading and trailing trivia of the specified node\. |
| [WithTriviaFrom\<TNode>(SeparatedSyntaxList\<TNode>, SyntaxNode)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new separated list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode>(SyntaxList\<TNode>, SyntaxNode)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode>(TNode, SyntaxToken)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1___0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node from this node with both the leading and trailing trivia of the specified token\. |

## WithTriviaFrom\(SyntaxToken, SyntaxNode\) <a id="Roslynator_SyntaxExtensions_WithTriviaFrom_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxNode_"></a>

\
Creates a new token from this token with both the leading and trailing trivia of the specified node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithTriviaFrom(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxNode node)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## WithTriviaFrom\<TNode>\(SeparatedSyntaxList\<TNode>, SyntaxNode\) <a id="Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_"></a>

\
Creates a new separated list with both leading and trailing trivia of the specified node\.
If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> WithTriviaFrom<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.SyntaxNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

## WithTriviaFrom\<TNode>\(SyntaxList\<TNode>, SyntaxNode\) <a id="Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_"></a>

\
Creates a new list with both leading and trailing trivia of the specified node\.
If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> WithTriviaFrom<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.SyntaxNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

## WithTriviaFrom\<TNode>\(TNode, SyntaxToken\) <a id="Roslynator_SyntaxExtensions_WithTriviaFrom__1___0_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Creates a new node from this node with both the leading and trailing trivia of the specified token\.

```csharp
public static TNode WithTriviaFrom<TNode>(this TNode node, Microsoft.CodeAnalysis.SyntaxToken token) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

TNode

