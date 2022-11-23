---
sidebar_label: WithTriviaFrom
---

# SyntaxExtensions\.WithTriviaFrom Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/index.md
| [WithTriviaFrom(SyntaxToken, SyntaxNode)](#Roslynator_SyntaxExtensions_WithTriviaFrom_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new token from this token with both the leading and trailing trivia of the specified node\. |
| [WithTriviaFrom&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxNode)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new separated list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxNode)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom&lt;TNode&gt;(TNode, SyntaxToken)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1___0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node from this node with both the leading and trailing trivia of the specified token\. |
========
| [WithTriviaFrom(SyntaxToken, SyntaxNode)](#3436644061) | Creates a new token from this token with both the leading and trailing trivia of the specified node\. |
| [WithTriviaFrom\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxNode)](#2087578213) | Creates a new separated list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode\>(SyntaxList\<TNode\>, SyntaxNode)](#301376900) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode\>(TNode, SyntaxToken)](#441639473) | Creates a new node from this node with both the leading and trailing trivia of the specified token\. |
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/README.md

<a id="3436644061"></a>

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/index.md
========
## WithTriviaFrom\(SyntaxToken, SyntaxNode\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/README.md
  
Creates a new token from this token with both the leading and trailing trivia of the specified node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithTriviaFrom(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxNode node)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/index.md
## WithTriviaFrom&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, SyntaxNode\) <a id="Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_"></a>

========
<a id="2087578213"></a>

## WithTriviaFrom\<TNode\>\(SeparatedSyntaxList\<TNode\>, SyntaxNode\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/README.md
  
Creates a new separated list with both leading and trailing trivia of the specified node\.
If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> WithTriviaFrom<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.SyntaxNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/index.md
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/README.md

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/index.md
[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

## WithTriviaFrom&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, SyntaxNode\) <a id="Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_"></a>

========
[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>

<a id="301376900"></a>

## WithTriviaFrom\<TNode\>\(SyntaxList\<TNode\>, SyntaxNode\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/README.md
  
Creates a new list with both leading and trailing trivia of the specified node\.
If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> WithTriviaFrom<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.SyntaxNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/index.md
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/README.md

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/index.md
[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

## WithTriviaFrom&lt;TNode&gt;\(TNode, SyntaxToken\) <a id="Roslynator_SyntaxExtensions_WithTriviaFrom__1___0_Microsoft_CodeAnalysis_SyntaxToken_"></a>

========
[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>

<a id="441639473"></a>

## WithTriviaFrom\<TNode\>\(TNode, SyntaxToken\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/WithTriviaFrom/README.md
  
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

