# SyntaxExtensions\.AppendToTrailingTrivia Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AppendToTrailingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia\>)](#682978820) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, SyntaxTrivia)](#2809312657) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode\>(TNode, IEnumerable\<SyntaxTrivia\>)](#782693212) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode\>(TNode, SyntaxTrivia)](#3044430369) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |

<a id="682978820"></a>

## AppendToTrailingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia\>\) 

  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="2809312657"></a>

## AppendToTrailingTrivia\(SyntaxToken, SyntaxTrivia\) 

  
Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken AppendToTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.SyntaxTrivia trivia)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<a id="782693212"></a>

## AppendToTrailingTrivia\<TNode\>\(TNode, IEnumerable\<SyntaxTrivia\>\) 

  
Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static TNode AppendToTrailingTrivia<TNode>(this TNode node, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**trivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>

### Returns

TNode

<a id="3044430369"></a>

## AppendToTrailingTrivia\<TNode\>\(TNode, SyntaxTrivia\) 

  
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

