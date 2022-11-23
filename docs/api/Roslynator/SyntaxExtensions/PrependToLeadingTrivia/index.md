---
sidebar_label: PrependToLeadingTrivia
---

# SyntaxExtensions\.PrependToLeadingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/index.md
| [PrependToLeadingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_PrependToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia(SyntaxToken, SyntaxTrivia)](#Roslynator_SyntaxExtensions_PrependToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_PrependToLeadingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](#Roslynator_SyntaxExtensions_PrependToLeadingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |

## PrependToLeadingTrivia\(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_PrependToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
| [PrependToLeadingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](#640281292) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia(SyntaxToken, SyntaxTrivia)](#2404824921) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](#3915245611) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia\<TNode\>(TNode, SyntaxTrivia)](#3276186521) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |

<a id="640281292"></a>

## PrependToLeadingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/README.md
  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/README.md

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="2404824921"></a>

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/index.md
========
## PrependToLeadingTrivia\(SyntaxToken, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/README.md
  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/index.md
## PrependToLeadingTrivia&lt;TNode&gt;\(TNode, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_PrependToLeadingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
<a id="3915245611"></a>

## PrependToLeadingTrivia\<TNode\>\(TNode, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/README.md
  
Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\.

```csharp
public static TNode PrependToLeadingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/README.md

### Returns

TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/index.md
## PrependToLeadingTrivia&lt;TNode&gt;\(TNode, SyntaxTrivia\) <a id="Roslynator_SyntaxExtensions_PrependToLeadingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

========
<a id="3276186521"></a>

## PrependToLeadingTrivia\<TNode\>\(TNode, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToLeadingTrivia/README.md
  
Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\.

```csharp
public static TNode PrependToLeadingTrivia<TNode>(this TNode node, Microsoft.CodeAnalysis.SyntaxTrivia trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

TNode

