---
sidebar_label: Insert
---

# ModifierList\.Insert Method

**Containing Type**: [ModifierList](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Insert(SyntaxTokenList, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](#3030337277) | Creates a new list of modifiers with the modifier of the specified kind inserted\. |
| [Insert(SyntaxTokenList, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](#3626674845) | Creates a new list of modifiers with a specified modifier inserted\. |
| [Insert&lt;TNode&gt;(TNode, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](#571500578) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert&lt;TNode&gt;(TNode, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](#2775814333) | Creates a new node with the specified modifier inserted\. |

<a id="3030337277"></a>

## Insert\(SyntaxTokenList, SyntaxKind, IComparer&lt;SyntaxKind&gt;\) 

  
Creates a new list of modifiers with the modifier of the specified kind inserted\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList Insert(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)&lt;[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)&gt;

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="3626674845"></a>

## Insert\(SyntaxTokenList, SyntaxToken, IComparer&lt;SyntaxToken&gt;\) 

  
Creates a new list of modifiers with a specified modifier inserted\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList Insert(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken modifier, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.SyntaxToken> comparer = null)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)&gt;

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="571500578"></a>

## Insert&lt;TNode&gt;\(TNode, SyntaxKind, IComparer&lt;SyntaxKind&gt;\) 

  
Creates a new node with a modifier of the specified kind inserted\.

```csharp
public static TNode Insert<TNode>(TNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)&lt;[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)&gt;

### Returns

TNode

<a id="2775814333"></a>

## Insert&lt;TNode&gt;\(TNode, SyntaxToken, IComparer&lt;SyntaxToken&gt;\) 

  
Creates a new node with the specified modifier inserted\.

```csharp
public static TNode Insert<TNode>(TNode node, Microsoft.CodeAnalysis.SyntaxToken modifier, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.SyntaxToken> comparer = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)&gt;

### Returns

TNode

