---
sidebar_label: RemoveAll
---

# ModifierList\.RemoveAll Method

**Containing Type**: [ModifierList&lt;TNode&gt;](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveAll(TNode, Func&lt;SyntaxToken, Boolean&gt;)](#1892225288) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAll(TNode)](#1375848717) | Creates a new node with all modifiers removed\. |

<a id="1892225288"></a>

## RemoveAll\(TNode, Func&lt;SyntaxToken, Boolean&gt;\) 

  
Creates a new node with modifiers that matches the predicate removed\.

```csharp
public TNode RemoveAll(TNode node, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate)
```

### Parameters

**node** &ensp; TNode

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

TNode

<a id="1375848717"></a>

## RemoveAll\(TNode\) 

  
Creates a new node with all modifiers removed\.

```csharp
public TNode RemoveAll(TNode node)
```

### Parameters

**node** &ensp; TNode

### Returns

TNode

