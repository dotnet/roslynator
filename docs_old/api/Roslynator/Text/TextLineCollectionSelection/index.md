---
sidebar_label: TextLineCollectionSelection
---

# TextLineCollectionSelection Class

**Namespace**: [Roslynator.Text](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Represents selected lines in a [TextLineCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textlinecollection)\.

```csharp
public class TextLineCollectionSelection : Roslynator.ISelection<Microsoft.CodeAnalysis.Text.TextLine>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; TextLineCollectionSelection

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)&gt;
* [ISelection](../../ISelection-1/index.md)&lt;[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [TextLineCollectionSelection(TextLineCollection, TextSpan, Int32, Int32)](-ctor/index.md) | Initializes a new instance of [TextLineCollectionSelection](./index.md)\. |

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/index.md) | Gets the selected line at the specified index\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/index.md) | Gets a number of selected lines\. |
| [FirstIndex](FirstIndex/index.md) | Gets an index of the first selected line\. \(Implements [ISelection&lt;TextLine&gt;.FirstIndex](../../ISelection-1/FirstIndex/index.md)\) |
| [LastIndex](LastIndex/index.md) | Gets an index of the last selected line\. \(Implements [ISelection&lt;TextLine&gt;.LastIndex](../../ISelection-1/LastIndex/index.md)\) |
| [OriginalSpan](OriginalSpan/index.md) | Gets the original span that was used to determine selected lines\. |
| [UnderlyingLines](UnderlyingLines/index.md) | Gets an underlying collection that contains selected lines\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(TextLineCollection, TextSpan)](Create/index.md) | Creates a new [TextLineCollectionSelection](./index.md) based on the specified list and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](First/index.md) | Gets the first selected line\. \(Implements [ISelection&lt;TextLine&gt;.First](../../ISelection-1/First/index.md)\) |
| [GetEnumerator()](GetEnumerator/index.md) | Returns an enumerator that iterates through selected lines\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](Last/index.md) | Gets the last selected line\. \(Implements [ISelection&lt;TextLine&gt;.Last](../../ISelection-1/Last/index.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(TextLineCollection, TextSpan, TextLineCollectionSelection)](TryCreate/index.md) | Creates a new [TextLineCollectionSelection](./index.md) based on the specified list and span\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;TextLine&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-Text-TextLine--GetEnumerator/index.md) | |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](Enumerator/index.md) | |

