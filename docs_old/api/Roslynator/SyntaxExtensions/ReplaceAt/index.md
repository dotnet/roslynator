---
sidebar_label: ReplaceAt
---

# SyntaxExtensions\.ReplaceAt Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceAt(SyntaxTokenList, Int32, SyntaxToken)](#2421566925) | Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\. |
| [ReplaceAt(SyntaxTriviaList, Int32, SyntaxTrivia)](#3526169581) | Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\. |
| [ReplaceAt&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, TNode)](#3499086875) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceAt&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, TNode)](#2512119344) | Creates a new list with the node at the specified index replaced with a new node\. |

<a id="2421566925"></a>

## ReplaceAt\(SyntaxTokenList, Int32, SyntaxToken\) 

  
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

## ReplaceAt\(SyntaxTriviaList, Int32, SyntaxTrivia\) 

  
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

<a id="3499086875"></a>

## ReplaceAt&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Int32, TNode\) 

  
Creates a new list with a node at the specified index replaced with a new node\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceAt<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

<a id="2512119344"></a>

## ReplaceAt&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Int32, TNode\) 

  
Creates a new list with the node at the specified index replaced with a new node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceAt<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

