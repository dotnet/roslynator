---
sidebar_label: AppendToLeadingTrivia
---

# SyntaxExtensions\.AppendToLeadingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AppendToLeadingTrivia(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;)](#2690812841) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia(SyntaxToken, SyntaxTrivia)](#2537170499) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia&lt;TNode&gt;(TNode, IEnumerable&lt;SyntaxTrivia&gt;)](#3159857401) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia&lt;TNode&gt;(TNode, SyntaxTrivia)](#161505572) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |

<a id="2690812841"></a>

## AppendToLeadingTrivia\(SyntaxToken, IEnumerable&lt;SyntaxTrivia&gt;\) 

  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="2537170499"></a>

## AppendToLeadingTrivia\(SyntaxToken, SyntaxTrivia\) 

  
Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="3159857401"></a>

## AppendToLeadingTrivia&lt;TNode&gt;\(TNode, IEnumerable&lt;SyntaxTrivia&gt;\) 

  
Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static TNode AppendToLeadingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

### Returns

TNode

<a id="161505572"></a>

## AppendToLeadingTrivia&lt;TNode&gt;\(TNode, SyntaxTrivia\) 

  
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

