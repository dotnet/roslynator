---
sidebar_label: AppendToLeadingTrivia
---

# SyntaxExtensions\.AppendToLeadingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/index.md
| [AppendToLeadingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_AppendToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia(SyntaxToken, SyntaxTrivia)](#Roslynator_SyntaxExtensions_AppendToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](#Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |

## AppendToLeadingTrivia\(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_AppendToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
| [AppendToLeadingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](#2690812841) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia(SyntaxToken, SyntaxTrivia)](#2537170499) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](#3159857401) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia\<TNode\>(TNode, SyntaxTrivia)](#161505572) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |

<a id="2690812841"></a>

## AppendToLeadingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/README.md
  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/README.md

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="2537170499"></a>

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/index.md
========
## AppendToLeadingTrivia\(SyntaxToken, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/README.md
  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/index.md
## AppendToLeadingTrivia&lt;TNode&gt;\(TNode, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
<a id="3159857401"></a>

## AppendToLeadingTrivia\<TNode\>\(TNode, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/README.md
  
Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static TNode AppendToLeadingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/index.md
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/README.md

### Returns

TNode

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/index.md
## AppendToLeadingTrivia&lt;TNode&gt;\(TNode, SyntaxTrivia\) <a id="Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

========
<a id="161505572"></a>

## AppendToLeadingTrivia\<TNode\>\(TNode, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/AppendToLeadingTrivia/README.md
  
Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static TNode AppendToLeadingTrivia<TNode>(this TNode node, Microsoft.CodeAnalysis.SyntaxTrivia trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

TNode

