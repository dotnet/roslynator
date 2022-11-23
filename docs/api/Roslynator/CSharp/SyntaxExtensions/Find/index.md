---
sidebar_label: Find
---

# SyntaxExtensions\.Find Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/index.md
| [Find(SyntaxTokenList, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_Find_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a token of the specified kind and returns the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Find(SyntaxTriviaList, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_Find_Microsoft_CodeAnalysis_SyntaxTriviaList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Find&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_Find__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Find&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_Find__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
========
| [Find(SyntaxTokenList, SyntaxKind)](#849057854) | Searches for a token of the specified kind and returns the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Find(SyntaxTriviaList, SyntaxKind)](#972702330) | Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Find\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxKind)](#3431504454) | Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Find\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](#2610293853) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/README.md

<a id="849057854"></a>

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/index.md
========
## Find\(SyntaxTokenList, SyntaxKind\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/README.md
  
Searches for a token of the specified kind and returns the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken Find(this Microsoft.CodeAnalysis.SyntaxTokenList tokenList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**tokenList** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="972702330"></a>

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/index.md
  
Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.
========
## Find\(SyntaxTriviaList, SyntaxKind\) 

  
Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/README.md

```csharp
public static Microsoft.CodeAnalysis.SyntaxTrivia Find(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/index.md
## Find&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_Find__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

  
Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.
========
<a id="3431504454"></a>

## Find\<TNode\>\(SeparatedSyntaxList\<TNode\>, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/README.md

```csharp
public static TNode Find<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/index.md
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/README.md

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

TNode

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/index.md
## Find&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_Find__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

  
Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.
========
<a id="2610293853"></a>

## Find\<TNode\>\(SyntaxList\<TNode\>, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/README.md

```csharp
public static TNode Find<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/index.md
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/Find/README.md

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

TNode

