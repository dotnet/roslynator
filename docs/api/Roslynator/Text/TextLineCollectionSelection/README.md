# TextLineCollectionSelection Class

[Home](../../../README.md) &#x2022; [Constructors](#constructors) &#x2022; [Indexers](#indexers) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Explicit Interface Implementations](#explicit-interface-implementations) &#x2022; [Structs](#structs)

**Namespace**: [Roslynator.Text](../README.md)

**Assembly**: Roslynator\.Core\.dll

\
Represents selected lines in a [TextLineCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textlinecollection)\.

```csharp
public class TextLineCollectionSelection : Roslynator.ISelection<Microsoft.CodeAnalysis.Text.TextLine>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; TextLineCollectionSelection

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)>
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)\<[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)>
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)\<[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)>
* [ISelection](../../ISelection-1/README.md)\<[TextLine](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textline)>

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [TextLineCollectionSelection(TextLineCollection, TextSpan, Int32, Int32)](-ctor/README.md) | Initializes a new instance of [TextLineCollectionSelection](./README.md)\. |

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/README.md) | Gets the selected line at the specified index\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/README.md) | Gets a number of selected lines\. |
| [FirstIndex](FirstIndex/README.md) | Gets an index of the first selected line\. \(Implements [ISelection\<TextLine>.FirstIndex](../../ISelection-1/FirstIndex/README.md)\) |
| [LastIndex](LastIndex/README.md) | Gets an index of the last selected line\. \(Implements [ISelection\<TextLine>.LastIndex](../../ISelection-1/LastIndex/README.md)\) |
| [OriginalSpan](OriginalSpan/README.md) | Gets the original span that was used to determine selected lines\. |
| [UnderlyingLines](UnderlyingLines/README.md) | Gets an underlying collection that contains selected lines\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(TextLineCollection, TextSpan)](Create/README.md) | Creates a new [TextLineCollectionSelection](./README.md) based on the specified list and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](First/README.md) | Gets the first selected line\. \(Implements [ISelection\<TextLine>.First](../../ISelection-1/First/README.md)\) |
| [GetEnumerator()](GetEnumerator/README.md) | Returns an enumerator that iterates through selected lines\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](Last/README.md) | Gets the last selected line\. \(Implements [ISelection\<TextLine>.Last](../../ISelection-1/Last/README.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(TextLineCollection, TextSpan, TextLineCollectionSelection)](TryCreate/README.md) | Creates a new [TextLineCollectionSelection](./README.md) based on the specified list and span\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/README.md) | |
| [IEnumerable\<TextLine>.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-Text-TextLine--GetEnumerator/README.md) | |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](Enumerator/README.md) | |

