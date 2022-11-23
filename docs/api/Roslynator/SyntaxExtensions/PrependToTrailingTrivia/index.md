---
sidebar_label: PrependToTrailingTrivia
---

# SyntaxExtensions\.PrependToTrailingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/index.md
| [PrependToTrailingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_PrependToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, SyntaxTrivia)](#Roslynator_SyntaxExtensions_PrependToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](#Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |

## PrependToTrailingTrivia\(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_PrependToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
| [PrependToTrailingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](#3817969325) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, SyntaxTrivia)](#1356374860) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](#1111873538) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia\<TNode\>(TNode, SyntaxTrivia)](#3683468027) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |

<a id="3817969325"></a>

## PrependToTrailingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/README.md
  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/README.md

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="1356374860"></a>

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/index.md
========
## PrependToTrailingTrivia\(SyntaxToken, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/README.md
  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/index.md
## PrependToTrailingTrivia&lt;TNode&gt;\(TNode, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
<a id="1111873538"></a>

## PrependToTrailingTrivia\<TNode\>\(TNode, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/README.md
  
Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\.

```csharp
public static TNode PrependToTrailingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/README.md

### Returns

TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/index.md
## PrependToTrailingTrivia&lt;TNode&gt;\(TNode, SyntaxTrivia\) <a id="Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

========
<a id="3683468027"></a>

## PrependToTrailingTrivia\<TNode\>\(TNode, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/PrependToTrailingTrivia/README.md
  
Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\.

```csharp
public static TNode PrependToTrailingTrivia<TNode>(this TNode node, Microsoft.CodeAnalysis.SyntaxTrivia trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

TNode

