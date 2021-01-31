# UsingDirectiveListInfo Struct

[Home](../../../../README.md) &#x2022; [Indexers](#indexers) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Explicit Interface Implementations](#explicit-interface-implementations)

**Namespace**: [Roslynator.CSharp.Syntax](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Provides information about a list of using directives\.

```csharp
public readonly struct UsingDirectiveListInfo : System.Collections.Generic.IReadOnlyList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; UsingDirectiveListInfo

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)>
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)>
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)>

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/README.md) | Gets the using directive at the specified index in the list\. \(Implements [IReadOnlyList\<UsingDirectiveSyntax>.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)\) |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/README.md) | A number of usings in the list\. |
| [Parent](Parent/README.md) | The declaration that contains the usings\. |
| [Success](Success/README.md) | Determines whether this struct was initialized with an actual syntax\. |
| [Usings](Usings/README.md) | A list of usings\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Add(UsingDirectiveSyntax)](Add/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified using directive added at the end\. |
| [AddRange(IEnumerable\<UsingDirectiveSyntax>)](AddRange/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified usings added at the end\. |
| [Any()](Any/README.md) | True if the list has at least one using directive\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [First()](First/README.md) | The first using directive in the list\. |
| [FirstOrDefault()](FirstOrDefault/README.md) | The first using directive in the list or null if the list is empty\. |
| [GetEnumerator()](GetEnumerator/README.md) | Gets the enumerator for the list of usings\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [IndexOf(Func\<UsingDirectiveSyntax, Boolean>)](IndexOf/README.md#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_System_Boolean__) | Searches for an using directive that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(UsingDirectiveSyntax)](IndexOf/README.md#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_) | The index of the using directive in the list\. |
| [Insert(Int32, UsingDirectiveSyntax)](Insert/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified using directive inserted at the index\. |
| [InsertRange(Int32, IEnumerable\<UsingDirectiveSyntax>)](InsertRange/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified usings inserted at the index\. |
| [Last()](Last/README.md) | The last using directive in the list\. |
| [LastIndexOf(Func\<UsingDirectiveSyntax, Boolean>)](LastIndexOf/README.md#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_System_Boolean__) | Searches for an using directive that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(UsingDirectiveSyntax)](LastIndexOf/README.md#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_) | Searches for an using directive and returns zero\-based index of the last occurrence in the list\. |
| [LastOrDefault()](LastOrDefault/README.md) | The last using directive in the list or null if the list is empty\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(UsingDirectiveSyntax)](Remove/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified using directive removed\. |
| [RemoveAt(Int32)](RemoveAt/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the using directive at the specified index removed\. |
| [RemoveNode(SyntaxNode, SyntaxRemoveOptions)](RemoveNode/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified node removed\. |
| [Replace(UsingDirectiveSyntax, UsingDirectiveSyntax)](Replace/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified using directive replaced with the new using directive\. |
| [ReplaceAt(Int32, UsingDirectiveSyntax)](ReplaceAt/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the using directive at the specified index replaced with a new using directive\. |
| [ReplaceNode(SyntaxNode, SyntaxNode)](ReplaceNode/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified old node replaced with a new node\. |
| [ReplaceRange(UsingDirectiveSyntax, IEnumerable\<UsingDirectiveSyntax>)](ReplaceRange/README.md) | Creates a new [UsingDirectiveListInfo](./README.md) with the specified using directive replaced with new usings\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [WithUsings(IEnumerable\<UsingDirectiveSyntax>)](WithUsings/README.md#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_WithUsings_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__) | Creates a new [UsingDirectiveListInfo](./README.md) with the usings updated\. |
| [WithUsings(SyntaxList\<UsingDirectiveSyntax>)](WithUsings/README.md#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_WithUsings_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__) | Creates a new [UsingDirectiveListInfo](./README.md) with the usings updated\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/README.md) | |
| [IEnumerable\<UsingDirectiveSyntax>.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-CSharp-Syntax-UsingDirectiveSyntax--GetEnumerator/README.md) | |

