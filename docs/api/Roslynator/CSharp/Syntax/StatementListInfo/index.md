---
sidebar_label: StatementListInfo
---

# StatementListInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about a list of statements\.

```csharp
public readonly struct StatementListInfo : System.Collections.Generic.IReadOnlyList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; StatementListInfo

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)&gt;

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/index.md) | Gets the statement at the specified index in the list\. \(Implements [IReadOnlyList&lt;StatementSyntax&gt;.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)\) |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/index.md) | The number of statement in the list\. |
| [IsParentBlock](IsParentBlock/index.md) | Determines whether the statements are contained in a [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)\. |
| [IsParentSwitchSection](IsParentSwitchSection/index.md) | Determines whether the statements are contained in a [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)\. |
| [Parent](Parent/index.md) | The node that contains the statements\. It can be either a [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax) or a [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)\. |
| [ParentAsBlock](ParentAsBlock/index.md) | Gets a block that contains the statements\. Returns null if the statements are not contained in a block\. |
| [ParentAsSwitchSection](ParentAsSwitchSection/index.md) | Gets a switch section that contains the statements\. Returns null if the statements are not contained in a switch section\. |
| [Statements](Statements/index.md) | The list of statements\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Add(StatementSyntax)](Add/index.md) | Creates a new [StatementListInfo](./index.md) with the specified statement added at the end\. |
| [AddRange(IEnumerable&lt;StatementSyntax&gt;)](AddRange/index.md) | Creates a new [StatementListInfo](./index.md) with the specified statements added at the end\. |
| [Any()](Any/index.md) | True if the list has at least one statement\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [First()](First/index.md) | The first statement in the list\. |
| [FirstOrDefault()](FirstOrDefault/index.md) | The first statement in the list or null if the list is empty\. |
| [GetEnumerator()](GetEnumerator/index.md) | Gets the enumerator the list of statements\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [IndexOf(Func&lt;StatementSyntax, Boolean&gt;)](IndexOf/index.md#Roslynator_CSharp_Syntax_StatementListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_System_Boolean__) | Searches for a statement that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(StatementSyntax)](IndexOf/index.md#Roslynator_CSharp_Syntax_StatementListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_) | The index of the statement in the list\. |
| [Insert(Int32, StatementSyntax)](Insert/index.md) | Creates a new [StatementListInfo](./index.md) with the specified statement inserted at the index\. |
| [InsertRange(Int32, IEnumerable&lt;StatementSyntax&gt;)](InsertRange/index.md) | Creates a new [StatementListInfo](./index.md) with the specified statements inserted at the index\. |
| [Last()](Last/index.md) | The last statement in the list\. |
| [LastIndexOf(Func&lt;StatementSyntax, Boolean&gt;)](LastIndexOf/index.md#Roslynator_CSharp_Syntax_StatementListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_System_Boolean__) | Searches for a statement that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(StatementSyntax)](LastIndexOf/index.md#Roslynator_CSharp_Syntax_StatementListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_) | Searches for a statement and returns zero\-based index of the last occurrence in the list\. |
| [LastOrDefault()](LastOrDefault/index.md) | The last statement in the list or null if the list is empty\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(StatementSyntax)](Remove/index.md) | Creates a new [StatementListInfo](./index.md) with the specified statement removed\. |
| [RemoveAt(Int32)](RemoveAt/index.md) | Creates a new [StatementListInfo](./index.md) with the statement at the specified index removed\. |
| [RemoveNode(SyntaxNode, SyntaxRemoveOptions)](RemoveNode/index.md) | Creates a new [StatementListInfo](./index.md) with the specified node removed\. |
| [Replace(StatementSyntax, StatementSyntax)](Replace/index.md) | Creates a new [StatementListInfo](./index.md) with the specified statement replaced with the new statement\. |
| [ReplaceAt(Int32, StatementSyntax)](ReplaceAt/index.md) | Creates a new [StatementListInfo](./index.md) with the statement at the specified index replaced with a new statement\. |
| [ReplaceNode(SyntaxNode, SyntaxNode)](ReplaceNode/index.md) | Creates a new [StatementListInfo](./index.md) with the specified old node replaced with a new node\. |
| [ReplaceRange(StatementSyntax, IEnumerable&lt;StatementSyntax&gt;)](ReplaceRange/index.md) | Creates a new [StatementListInfo](./index.md) with the specified statement replaced with new statements\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [WithStatements(IEnumerable&lt;StatementSyntax&gt;)](WithStatements/index.md#Roslynator_CSharp_Syntax_StatementListInfo_WithStatements_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__) | Creates a new [StatementListInfo](./index.md) with the statements updated\. |
| [WithStatements(SyntaxList&lt;StatementSyntax&gt;)](WithStatements/index.md#Roslynator_CSharp_Syntax_StatementListInfo_WithStatements_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax__) | Creates a new [StatementListInfo](./index.md) with the statements updated\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;StatementSyntax&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-CSharp-Syntax-StatementSyntax--GetEnumerator/index.md) | |

