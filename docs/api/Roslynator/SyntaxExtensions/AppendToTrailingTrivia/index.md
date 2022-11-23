---
sidebar_label: AppendToTrailingTrivia
---

# SyntaxExtensions\.AppendToTrailingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/index.md
| [AppendToTrailingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_AppendToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, SyntaxTrivia)](#Roslynator_SyntaxExtensions_AppendToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](#Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |

## AppendToTrailingTrivia\(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_AppendToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
| [AppendToTrailingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](#682978820) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, SyntaxTrivia)](#2809312657) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](#782693212) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode\>(TNode, SyntaxTrivia)](#3044430369) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |

<a id="682978820"></a>

## AppendToTrailingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/README.md
  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/README.md

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="2809312657"></a>

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/index.md
========
## AppendToTrailingTrivia\(SyntaxToken, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/README.md
  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/index.md
## AppendToTrailingTrivia&lt;TNode&gt;\(TNode, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
<a id="782693212"></a>

## AppendToTrailingTrivia\<TNode\>\(TNode, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/README.md
  
Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static TNode AppendToTrailingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/README.md

### Returns

TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/index.md
## AppendToTrailingTrivia&lt;TNode&gt;\(TNode, SyntaxTrivia\) <a id="Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

========
<a id="3044430369"></a>

## AppendToTrailingTrivia\<TNode\>\(TNode, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToTrailingTrivia/README.md
  
Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static TNode AppendToTrailingTrivia<TNode>(this TNode node, Microsoft.CodeAnalysis.SyntaxTrivia trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

TNode

