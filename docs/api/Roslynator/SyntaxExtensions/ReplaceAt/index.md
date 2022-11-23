---
sidebar_label: ReplaceAt
---

# SyntaxExtensions\.ReplaceAt Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
| [ReplaceAt(SyntaxTokenList, Int32, SyntaxToken)](#Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\. |
| [ReplaceAt(SyntaxTriviaList, Int32, SyntaxTrivia)](#Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\. |
| [ReplaceAt&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, TNode)](#Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32___0_) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceAt&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, TNode)](#Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32___0_) | Creates a new list with the node at the specified index replaced with a new node\. |
========
| [ReplaceAt(SyntaxTokenList, Int32, SyntaxToken)](#2421566925) | Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\. |
| [ReplaceAt(SyntaxTriviaList, Int32, SyntaxTrivia)](#3526169581) | Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\. |
| [ReplaceAt\<TNode\>(SeparatedSyntaxList\<TNode\>, Int32, TNode)](#3499086875) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceAt\<TNode\>(SyntaxList\<TNode\>, Int32, TNode)](#2512119344) | Creates a new list with the node at the specified index replaced with a new node\. |
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md

<a id="2421566925"></a>

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
========
## ReplaceAt\(SyntaxTokenList, Int32, SyntaxToken\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md
  
Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList ReplaceAt(this Microsoft.CodeAnalysis.SyntaxTokenList tokenList, int index, Microsoft.CodeAnalysis.SyntaxToken newToken)
```

### Parameters

**tokenList** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="3526169581"></a>

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
========
## ReplaceAt\(SyntaxTriviaList, Int32, SyntaxTrivia\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md
  
Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTriviaList ReplaceAt(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, int index, Microsoft.CodeAnalysis.SyntaxTrivia newTrivia)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newTrivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
## ReplaceAt&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Int32, TNode\) <a id="Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32___0_"></a>

========
<a id="3499086875"></a>

## ReplaceAt\<TNode\>\(SeparatedSyntaxList\<TNode\>, Int32, TNode\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md
  
Creates a new list with a node at the specified index replaced with a new node\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceAt<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

## ReplaceAt&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Int32, TNode\) <a id="Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32___0_"></a>

========
[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>

<a id="2512119344"></a>

## ReplaceAt\<TNode\>\(SyntaxList\<TNode\>, Int32, TNode\) 

>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md
  
Creates a new list with the node at the specified index replaced with a new node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceAt<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

<<<<<<<< HEAD:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/index.md
[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/SyntaxExtensions/ReplaceAt/README.md

