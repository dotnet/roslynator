---
sidebar_label: RemoveRange
---

# SyntaxExtensions\.RemoveRange Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveRange(SyntaxTokenList, Int32, Int32)](#560377099) | Creates a new list with tokens in the specified range removed\. |
| [RemoveRange(SyntaxTriviaList, Int32, Int32)](#2543741306) | Creates a new list with trivia in the specified range removed\. |
| [RemoveRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32)](#1305034856) | Creates a new list with elements in the specified range removed\. |
| [RemoveRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32)](#3807495140) | Creates a new list with elements in the specified range removed\. |

<a id="560377099"></a>

## RemoveRange\(SyntaxTokenList, Int32, Int32\) 

  
Creates a new list with tokens in the specified range removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList RemoveRange(this Microsoft.CodeAnalysis.SyntaxTokenList list, int index, int count)
```

### Parameters

**list** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

An index of the first element to remove\.

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

A number of elements to remove\.

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="2543741306"></a>

## RemoveRange\(SyntaxTriviaList, Int32, Int32\) 

  
Creates a new list with trivia in the specified range removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTriviaList RemoveRange(this Microsoft.CodeAnalysis.SyntaxTriviaList list, int index, int count)
```

### Parameters

**list** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

An index of the first element to remove\.

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

A number of elements to remove\.

### Returns

[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

<a id="1305034856"></a>

## RemoveRange&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32\) 

  
Creates a new list with elements in the specified range removed\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> RemoveRange<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, int count) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

An index of the first element to remove\.

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

A number of elements to remove\.

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

<a id="3807495140"></a>

## RemoveRange&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Int32, Int32\) 

  
Creates a new list with elements in the specified range removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> RemoveRange<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, int count) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

An index of the first element to remove\.

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

A number of elements to remove\.

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

