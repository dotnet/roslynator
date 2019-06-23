# SeparatedSyntaxListSelection\<TNode> Class

[Home](../../README.md) &#x2022; [Constructors](#constructors) &#x2022; [Indexers](#indexers) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Explicit Interface Implementations](#explicit-interface-implementations) &#x2022; [Structs](#structs)

**Namespace**: [Roslynator](../README.md)

**Assembly**: Roslynator\.Core\.dll

\
Represents selected nodes in a [SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public class SeparatedSyntaxListSelection<TNode> : Roslynator.ISelection<TNode> where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; SeparatedSyntaxListSelection\<TNode>

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<TNode>
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)\<TNode>
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)\<TNode>
* [ISelection](../ISelection-1/README.md)\<TNode>

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [SeparatedSyntaxListSelection(SeparatedSyntaxList\<TNode>, TextSpan, Int32, Int32)](-ctor/README.md) | Initializes a new instance of the [SeparatedSyntaxListSelection\<TNode>](./README.md)\. |

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/README.md) | Gets the selected node at the specified index\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/README.md) | Gets a number of selected nodes\. |
| [FirstIndex](FirstIndex/README.md) | Gets an index of the first selected node\. \(Implements [ISelection\<TNode>.FirstIndex](../ISelection-1/FirstIndex/README.md)\) |
| [LastIndex](LastIndex/README.md) | Gets an index of the last selected node\. \(Implements [ISelection\<TNode>.LastIndex](../ISelection-1/LastIndex/README.md)\) |
| [OriginalSpan](OriginalSpan/README.md) | Gets the original span that was used to determine selected nodes\. |
| [UnderlyingList](UnderlyingList/README.md) | Gets an underlying list that contains selected nodes\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(SeparatedSyntaxList\<TNode>, TextSpan)](Create/README.md) | Creates a new [SeparatedSyntaxListSelection\<TNode>](./README.md) based on the specified list and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](First/README.md) | Gets the first selected node\. \(Implements [ISelection\<TNode>.First](../ISelection-1/First/README.md)\) |
| [GetEnumerator()](GetEnumerator/README.md) | Returns an enumerator that iterates through selected nodes\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](Last/README.md) | Gets the last selected node\. \(Implements [ISelection\<TNode>.Last](../ISelection-1/Last/README.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(SeparatedSyntaxList\<TNode>, TextSpan, SeparatedSyntaxListSelection\<TNode>)](TryCreate/README.md) | Creates a new [SeparatedSyntaxListSelection\<TNode>](./README.md) based on the specified list and span\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/README.md) | |
| [IEnumerable\<TNode>.GetEnumerator()](System-Collections-Generic-IEnumerable-TNode--GetEnumerator/README.md) | |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](Enumerator/README.md) | |

