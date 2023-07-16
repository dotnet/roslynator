---
sidebar_label: ReplaceRange
---

# SyntaxExtensions\.ReplaceRange Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceRange(SyntaxTokenList, Int32, Int32, IEnumerable&lt;SyntaxToken&gt;)](#4257224275) | Creates a new list with the tokens in the specified range replaced with new tokens\. |
| [ReplaceRange(SyntaxTriviaList, Int32, Int32, IEnumerable&lt;SyntaxTrivia&gt;)](#4063342571) | Creates a new list with the trivia in the specified range replaced with new trivia\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](#607003656) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](#2148171151) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](#3814604200) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](#3682382942) | Creates a new list with the elements in the specified range replaced with new node\. |

<a id="4257224275"></a>

## ReplaceRange\(SyntaxTokenList, Int32, Int32, IEnumerable&lt;SyntaxToken&gt;\) 

  
Creates a new list with the tokens in the specified range replaced with new tokens\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList ReplaceRange(this Microsoft.CodeAnalysis.SyntaxTokenList list, int index, int count, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> newTokens)
```

### Parameters

**list** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newTokens** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)&gt;

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="4063342571"></a>

## ReplaceRange\(SyntaxTriviaList, Int32, Int32, IEnumerable&lt;SyntaxTrivia&gt;\) 

  
Creates a new list with the trivia in the specified range replaced with new trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTriviaList ReplaceRange(this Microsoft.CodeAnalysis.SyntaxTriviaList list, int index, int count, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> newTrivia)
```

### Parameters

**list** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newTrivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

### Returns

[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

<a id="607003656"></a>

## ReplaceRange&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;\) 

  
Creates a new list with the elements in the specified range replaced with new nodes\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, int count, System.Collections.Generic.IEnumerable<TNode> newNodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;TNode&gt;

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

<a id="2148171151"></a>

## ReplaceRange&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, TNode\) 

  
Creates a new list with the elements in the specified range replaced with new node\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, int count, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

<a id="3814604200"></a>

## ReplaceRange&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;\) 

  
Creates a new list with the elements in the specified range replaced with new nodes\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, int count, System.Collections.Generic.IEnumerable<TNode> newNodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;TNode&gt;

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

<a id="3682382942"></a>

## ReplaceRange&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Int32, Int32, TNode\) 

  
Creates a new list with the elements in the specified range replaced with new node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, int count, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

