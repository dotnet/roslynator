---
sidebar_label: PrependToLeadingTrivia
---

# SyntaxExtensions\.PrependToLeadingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [PrependToLeadingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](#640281292) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia(SyntaxToken, SyntaxTrivia)](#2404824921) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](#3915245611) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |
| [PrependToLeadingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](#3276186521) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\. |

<a id="640281292"></a>

## PrependToLeadingTrivia\(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;\) 

  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="2404824921"></a>

## PrependToLeadingTrivia\(SyntaxToken, SyntaxTrivia\) 

  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken PrependToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="3915245611"></a>

## PrependToLeadingTrivia&lt;TNode&gt;\(TNode, IEnumerable&lt;SyntaxTrivia&gt;\) 

  
Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is inserted at the beginning of the leading trivia\.

```csharp
public static TNode PrependToLeadingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

### Returns

TNode

<a id="3276186521"></a>

## PrependToLeadingTrivia&lt;TNode&gt;\(TNode, SyntaxTrivia\) 

  
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

