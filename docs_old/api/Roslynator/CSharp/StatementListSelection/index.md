---
sidebar_label: StatementListSelection
---

# StatementListSelection Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Represents selected statements in a [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public sealed class StatementListSelection : Roslynator.SyntaxListSelection<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md) &#x2192; StatementListSelection

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;
* [ISelection](../../ISelection-1/index.md)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](../../SyntaxListSelection-1/Item/index.md) | Gets the selected node at the specified index\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](../../SyntaxListSelection-1/Count/index.md) | Gets a number of selected nodes\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [FirstIndex](../../SyntaxListSelection-1/FirstIndex/index.md) | Gets an index of the first selected node\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [LastIndex](../../SyntaxListSelection-1/LastIndex/index.md) | Gets an index of the last selected node\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [OriginalSpan](../../SyntaxListSelection-1/OriginalSpan/index.md) | Gets the original span that was used to determine selected nodes\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [UnderlyingList](../../SyntaxListSelection-1/UnderlyingList/index.md) | Gets an underlying list that contains selected nodes\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(BlockSyntax, TextSpan)](Create/index.md#933669229) | Creates a new [StatementListSelection](./index.md) based on the specified block and span\. |
| [Create(StatementListInfo, TextSpan)](Create/index.md#4239290103) | Creates a new [StatementListSelection](./index.md) based on the specified [StatementListInfo](../Syntax/StatementListInfo/index.md) and span\. |
| [Create(SwitchSectionSyntax, TextSpan)](Create/index.md#1797202091) | Creates a new [StatementListSelection](./index.md) based on the specified switch section and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](../../SyntaxListSelection-1/First/index.md) | Gets the first selected node\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [GetEnumerator()](../../SyntaxListSelection-1/GetEnumerator/index.md) | Returns an enumerator that iterates through selected nodes\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](../../SyntaxListSelection-1/Last/index.md) | Gets the last selected node\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(BlockSyntax, TextSpan, StatementListSelection)](TryCreate/index.md#736714011) | Creates a new [StatementListSelection](./index.md) based on the specified block and span\. |
| [TryCreate(SwitchSectionSyntax, TextSpan, StatementListSelection)](TryCreate/index.md#1958870021) | Creates a new [StatementListSelection](./index.md) based on the specified switch section and span\. |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](../../SyntaxListSelection-1/Enumerator/index.md) |  \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |

