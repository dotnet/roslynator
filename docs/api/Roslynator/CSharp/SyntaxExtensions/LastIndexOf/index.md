---
sidebar_label: LastIndexOf
---

# SyntaxExtensions\.LastIndexOf Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md
| [LastIndexOf(SyntaxTriviaList, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_LastIndexOf_Microsoft_CodeAnalysis_SyntaxTriviaList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [LastIndexOf&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [LastIndexOf&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
========
| [LastIndexOf(SyntaxTriviaList, SyntaxKind)](#2989371063) | Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [LastIndexOf\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxKind)](#1073548081) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [LastIndexOf\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](#2386444843) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/README.md

<a id="2989371063"></a>

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md
========
## LastIndexOf\(SyntaxTriviaList, SyntaxKind\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/README.md
  
Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\.

```csharp
public static int LastIndexOf(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md
## LastIndexOf&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.
========
<a id="1073548081"></a>

## LastIndexOf\<TNode\>\(SeparatedSyntaxList\<TNode\>, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/README.md

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/README.md

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md
## LastIndexOf&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.
========
<a id="2386444843"></a>

## LastIndexOf\<TNode\>\(SyntaxList\<TNode\>, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/README.md

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/index.md
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/LastIndexOf/README.md

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

