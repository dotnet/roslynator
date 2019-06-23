# StatementListSelection Class

[Home](../../../README.md) &#x2022; [Indexers](#indexers) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Structs](#structs)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Represents selected statements in a [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public sealed class StatementListSelection : Roslynator.SyntaxListSelection<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md) &#x2192; StatementListSelection

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)>
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)>
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)>
* [ISelection](../../ISelection-1/README.md)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)>

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](../../SyntaxListSelection-1/Item/README.md) | Gets the selected node at the specified index\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](../../SyntaxListSelection-1/Count/README.md) | Gets a number of selected nodes\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [FirstIndex](../../SyntaxListSelection-1/FirstIndex/README.md) | Gets an index of the first selected node\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [LastIndex](../../SyntaxListSelection-1/LastIndex/README.md) | Gets an index of the last selected node\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [OriginalSpan](../../SyntaxListSelection-1/OriginalSpan/README.md) | Gets the original span that was used to determine selected nodes\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [UnderlyingList](../../SyntaxListSelection-1/UnderlyingList/README.md) | Gets an underlying list that contains selected nodes\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(BlockSyntax, TextSpan)](Create/README.md#Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_BlockSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](./README.md) based on the specified block and span\. |
| [Create(StatementListInfo, TextSpan)](Create/README.md#Roslynator_CSharp_StatementListSelection_Create_Roslynator_CSharp_Syntax_StatementListInfo__Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](./README.md) based on the specified [StatementListInfo](../Syntax/StatementListInfo/README.md) and span\. |
| [Create(SwitchSectionSyntax, TextSpan)](Create/README.md#Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_SwitchSectionSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](./README.md) based on the specified switch section and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](../../SyntaxListSelection-1/First/README.md) | Gets the first selected node\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [GetEnumerator()](../../SyntaxListSelection-1/GetEnumerator/README.md) | Returns an enumerator that iterates through selected nodes\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](../../SyntaxListSelection-1/Last/README.md) | Gets the last selected node\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(BlockSyntax, TextSpan, StatementListSelection)](TryCreate/README.md#Roslynator_CSharp_StatementListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_BlockSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_StatementListSelection__) | Creates a new [StatementListSelection](./README.md) based on the specified block and span\. |
| [TryCreate(SwitchSectionSyntax, TextSpan, StatementListSelection)](TryCreate/README.md#Roslynator_CSharp_StatementListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_SwitchSectionSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_StatementListSelection__) | Creates a new [StatementListSelection](./README.md) based on the specified switch section and span\. |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](../../SyntaxListSelection-1/Enumerator/README.md) |  \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |

