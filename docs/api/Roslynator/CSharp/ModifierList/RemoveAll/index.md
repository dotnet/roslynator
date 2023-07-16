---
sidebar_label: RemoveAll
---

# ModifierList\.RemoveAll Method

**Containing Type**: [ModifierList](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveAll&lt;TNode&gt;(TNode, Func&lt;SyntaxToken, Boolean&gt;)](#704560686) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAll&lt;TNode&gt;(TNode)](#672470665) | Creates a new node with all modifiers removed\. |

<a id="704560686"></a>

## RemoveAll&lt;TNode&gt;\(TNode, Func&lt;SyntaxToken, Boolean&gt;\) 

  
Creates a new node with modifiers that matches the predicate removed\.

```csharp
public static TNode RemoveAll<TNode>(TNode node, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

TNode

<a id="672470665"></a>

## RemoveAll&lt;TNode&gt;\(TNode\) 

  
Creates a new node with all modifiers removed\.

```csharp
public static TNode RemoveAll<TNode>(TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

### Returns

TNode

