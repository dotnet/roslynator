---
sidebar_label: PrependToTrailingTrivia
---

# SyntaxExtensions\.PrependToTrailingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [PrependToTrailingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](#3817969325) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, SyntaxTrivia)](#1356374860) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](#1111873538) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |
| [PrependToTrailingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](#3683468027) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\. |

<a id="3817969325"></a>

## PrependToTrailingTrivia\(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;\) 

  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="1356374860"></a>

## PrependToTrailingTrivia\(SyntaxToken, SyntaxTrivia\) 

  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="1111873538"></a>

## PrependToTrailingTrivia&lt;TNode&gt;\(TNode, IEnumerable&lt;SyntaxTrivia&gt;\) 

  
Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the trailing trivia\.

```csharp
public static TNode PrependToTrailingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

### Returns

TNode

<a id="3683468027"></a>

## PrependToTrailingTrivia&lt;TNode&gt;\(TNode, SyntaxTrivia\) 

  
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

