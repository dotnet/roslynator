---
sidebar_label: Find
---

# SyntaxExtensions\.Find Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Find(SyntaxTokenList, SyntaxKind)](#849057854) | Searches for a token of the specified kind and returns the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Find(SyntaxTriviaList, SyntaxKind)](#972702330) | Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |
| [Find&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind)](#3431504454) | Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Find&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, SyntaxKind)](#2610293853) | Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |

<a id="849057854"></a>

## Find\(SyntaxTokenList, SyntaxKind\) 

  
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

## Find\(SyntaxTriviaList, SyntaxKind\) 

  
Searches for a trivia of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTrivia Find(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

<a id="3431504454"></a>

## Find&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the first occurrence within the entire [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public static TNode Find<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

TNode

<a id="2610293853"></a>

## Find&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the first occurrence within the entire [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static TNode Find<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

TNode

