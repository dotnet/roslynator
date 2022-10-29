# ModifierList Class

[Home](../../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of static methods that allows manipulation with modifiers\.

```csharp
public static class ModifierList
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetInsertIndex(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind\>)](GetInsertIndex/README.md#4042759183) | Returns an index a token with the specified kind should be inserted at\. |
| [GetInsertIndex(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken\>)](GetInsertIndex/README.md#2289002607) | Returns an index the specified token should be inserted at\. |
| [Insert(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind\>)](Insert/README.md#3030337277) | Creates a new list of modifiers with the modifier of the specified kind inserted\. |
| [Insert(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken\>)](Insert/README.md#3626674845) | Creates a new list of modifiers with a specified modifier inserted\. |
| [Insert\<TNode\>(TNode, SyntaxKind, IComparer\<SyntaxKind\>)](Insert/README.md#571500578) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert\<TNode\>(TNode, SyntaxToken, IComparer\<SyntaxToken\>)](Insert/README.md#2775814333) | Creates a new node with the specified modifier inserted\. |
| [Remove\<TNode\>(TNode, SyntaxKind)](Remove/README.md#2255088376) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove\<TNode\>(TNode, SyntaxToken)](Remove/README.md#2345772034) | Creates a new node with the specified modifier removed\. |
| [RemoveAll\<TNode\>(TNode, Func\<SyntaxToken, Boolean\>)](RemoveAll/README.md#704560686) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAll\<TNode\>(TNode)](RemoveAll/README.md#672470665) | Creates a new node with all modifiers removed\. |
| [RemoveAt\<TNode\>(TNode, Int32)](RemoveAt/README.md) | Creates a new node with a modifier at the specified index removed\. |

