---
sidebar_label: ModifierList
---

# ModifierList Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of static methods that allows manipulation with modifiers\.

```csharp
public static class ModifierList
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetInsertIndex(SyntaxTokenList, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](GetInsertIndex/index.md#4042759183) | Returns an index a token with the specified kind should be inserted at\. |
| [GetInsertIndex(SyntaxTokenList, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](GetInsertIndex/index.md#2289002607) | Returns an index the specified token should be inserted at\. |
| [Insert(SyntaxTokenList, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](Insert/index.md#3030337277) | Creates a new list of modifiers with the modifier of the specified kind inserted\. |
| [Insert(SyntaxTokenList, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](Insert/index.md#3626674845) | Creates a new list of modifiers with a specified modifier inserted\. |
| [Insert&lt;TNode&gt;(TNode, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](Insert/index.md#571500578) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert&lt;TNode&gt;(TNode, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](Insert/index.md#2775814333) | Creates a new node with the specified modifier inserted\. |
| [Remove&lt;TNode&gt;(TNode, SyntaxKind)](Remove/index.md#2255088376) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove&lt;TNode&gt;(TNode, SyntaxToken)](Remove/index.md#2345772034) | Creates a new node with the specified modifier removed\. |
| [RemoveAll&lt;TNode&gt;(TNode, Func&lt;SyntaxToken, Boolean&gt;)](RemoveAll/index.md#704560686) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAll&lt;TNode&gt;(TNode)](RemoveAll/index.md#672470665) | Creates a new node with all modifiers removed\. |
| [RemoveAt&lt;TNode&gt;(TNode, Int32)](RemoveAt/index.md) | Creates a new node with a modifier at the specified index removed\. |

