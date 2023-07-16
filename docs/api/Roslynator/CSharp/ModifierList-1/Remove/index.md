---
sidebar_label: Remove
---

# ModifierList\.Remove Method

**Containing Type**: [ModifierList&lt;TNode&gt;](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Remove(TNode, SyntaxKind)](#981244679) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove(TNode, SyntaxToken)](#1001668605) | Creates a new node with the specified modifier removed\. |

<a id="981244679"></a>

## Remove\(TNode, SyntaxKind\) 

  
Creates a new node with a modifier of the specified kind removed\.

```csharp
public TNode Remove(TNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**node** &ensp; TNode

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

TNode

<a id="1001668605"></a>

## Remove\(TNode, SyntaxToken\) 

  
Creates a new node with the specified modifier removed\.

```csharp
public TNode Remove(TNode node, Microsoft.CodeAnalysis.SyntaxToken modifier)
```

### Parameters

**node** &ensp; TNode

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

TNode

