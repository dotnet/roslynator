---
sidebar_label: ModifierList<TNode>
---

# ModifierList&lt;TNode&gt; Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Represents a list of modifiers\.

```csharp
public abstract class ModifierList<TNode> where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; ModifierList&lt;TNode&gt;

## Properties

| Property | Summary |
| -------- | ------- |
| [Instance](Instance/index.md) | Gets an instance of the [ModifierList&lt;TNode&gt;](./index.md) for a syntax specified by the generic argument\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Insert(TNode, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](Insert/index.md#4255247645) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert(TNode, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](Insert/index.md#2540511869) | Creates a new node with the specified modifier inserted\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(TNode, SyntaxKind)](Remove/index.md#981244679) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove(TNode, SyntaxToken)](Remove/index.md#1001668605) | Creates a new node with the specified modifier removed\. |
| [RemoveAll(TNode, Func&lt;SyntaxToken, Boolean&gt;)](RemoveAll/index.md#1892225288) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAll(TNode)](RemoveAll/index.md#1375848717) | Creates a new node with all modifiers removed\. |
| [RemoveAt(TNode, Int32)](RemoveAt/index.md) | Creates a new node with a modifier at the specified index removed\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

