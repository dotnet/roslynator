---
sidebar_label: SyntaxListSelection<TNode>
---

# SyntaxListSelection&lt;TNode&gt; Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Represents selected nodes in a [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public class SyntaxListSelection<TNode> : Roslynator.ISelection<TNode> where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; SyntaxListSelection&lt;TNode&gt;

### Derived

* [MemberDeclarationListSelection](../CSharp/MemberDeclarationListSelection/index.md)
* [StatementListSelection](../CSharp/StatementListSelection/index.md)

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;TNode&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;TNode&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;TNode&gt;
* [ISelection](../ISelection-1/index.md)&lt;TNode&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [SyntaxListSelection(SyntaxList&lt;TNode&gt;, TextSpan, Int32, Int32)](-ctor/index.md) | Initializes a new instance of the [SyntaxListSelection&lt;TNode&gt;](./index.md)\. |

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/index.md) | Gets the selected node at the specified index\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/index.md) | Gets a number of selected nodes\. |
| [FirstIndex](FirstIndex/index.md) | Gets an index of the first selected node\. \(Implements [ISelection&lt;TNode&gt;.FirstIndex](../ISelection-1/FirstIndex/index.md)\) |
| [LastIndex](LastIndex/index.md) | Gets an index of the last selected node\. \(Implements [ISelection&lt;TNode&gt;.LastIndex](../ISelection-1/LastIndex/index.md)\) |
| [OriginalSpan](OriginalSpan/index.md) | Gets the original span that was used to determine selected nodes\. |
| [UnderlyingList](UnderlyingList/index.md) | Gets an underlying list that contains selected nodes\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(SyntaxList&lt;TNode&gt;, TextSpan)](Create/index.md) | Creates a new [SyntaxListSelection&lt;TNode&gt;](./index.md) based on the specified list and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](First/index.md) | Gets the first selected node\. \(Implements [ISelection&lt;TNode&gt;.First](../ISelection-1/First/index.md)\) |
| [GetEnumerator()](GetEnumerator/index.md) | Returns an enumerator that iterates through selected nodes\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](Last/index.md) | Gets the last selected node\. \(Implements [ISelection&lt;TNode&gt;.Last](../ISelection-1/Last/index.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(SyntaxList&lt;TNode&gt;, TextSpan, SyntaxListSelection&lt;TNode&gt;)](TryCreate/index.md) | Creates a new [SyntaxListSelection&lt;TNode&gt;](./index.md) based on the specified list and span\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;TNode&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-TNode--GetEnumerator/index.md) | |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](Enumerator/index.md) | |

