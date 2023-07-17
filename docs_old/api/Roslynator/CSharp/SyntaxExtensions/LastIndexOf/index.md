---
sidebar_label: LastIndexOf
---

# SyntaxExtensions\.LastIndexOf Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(SyntaxTriviaList, SyntaxKind)](#2989371063) | Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [LastIndexOf&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](#1073548081) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [LastIndexOf&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](#2386444843) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |

<a id="2989371063"></a>

## LastIndexOf\(SyntaxTriviaList, SyntaxKind\) 

  
Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\.

```csharp
public static int LastIndexOf(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="1073548081"></a>

## LastIndexOf&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="2386444843"></a>

## LastIndexOf&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

