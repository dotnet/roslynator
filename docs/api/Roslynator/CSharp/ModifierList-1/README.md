# ModifierList\<TNode\> Class

[Home](../../../README.md) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Represents a list of modifiers\.

```csharp
public abstract class ModifierList<TNode> where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; ModifierList\<TNode\>

## Properties

| Property | Summary |
| -------- | ------- |
| [Instance](Instance/README.md) | Gets an instance of the [ModifierList\<TNode\>](./README.md) for a syntax specified by the generic argument\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Insert(TNode, SyntaxKind, IComparer\<SyntaxKind\>)](Insert/README.md#4255247645) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert(TNode, SyntaxToken, IComparer\<SyntaxToken\>)](Insert/README.md#2540511869) | Creates a new node with the specified modifier inserted\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(TNode, SyntaxKind)](Remove/README.md#981244679) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove(TNode, SyntaxToken)](Remove/README.md#1001668605) | Creates a new node with the specified modifier removed\. |
| [RemoveAll(TNode, Func\<SyntaxToken, Boolean\>)](RemoveAll/README.md#1892225288) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAll(TNode)](RemoveAll/README.md#1375848717) | Creates a new node with all modifiers removed\. |
| [RemoveAt(TNode, Int32)](RemoveAt/README.md) | Creates a new node with a modifier at the specified index removed\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

