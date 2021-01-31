# MemberDeclarationListInfo Struct

[Home](../../../../README.md) &#x2022; [Indexers](#indexers) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Explicit Interface Implementations](#explicit-interface-implementations)

**Namespace**: [Roslynator.CSharp.Syntax](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Provides information about a list of member declaration list\.

```csharp
public readonly struct MemberDeclarationListInfo : System.Collections.Generic.IReadOnlyList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; MemberDeclarationListInfo

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/README.md) | Gets the member at the specified index in the list\. \(Implements [IReadOnlyList\<MemberDeclarationSyntax>.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)\) |

## Properties

| Property | Summary |
| -------- | ------- |
| [CloseBraceToken](CloseBraceToken/README.md) | Gets a close brace token\. |
| [Count](Count/README.md) | A number of members in the list\. |
| [Members](Members/README.md) | A list of members\. |
| [OpenBraceToken](OpenBraceToken/README.md) | Gets a open brace token\. |
| [Parent](Parent/README.md) | The declaration that contains the members\. |
| [Success](Success/README.md) | Determines whether this struct was initialized with an actual syntax\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Add(MemberDeclarationSyntax)](Add/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified member added at the end\. |
| [AddRange(IEnumerable\<MemberDeclarationSyntax>)](AddRange/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified members added at the end\. |
| [Any()](Any/README.md) | True if the list has at least one member\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [First()](First/README.md) | The first member in the list\. |
| [FirstOrDefault()](FirstOrDefault/README.md) | The first member in the list or null if the list is empty\. |
| [GetEnumerator()](GetEnumerator/README.md) | Gets the enumerator for the list of members\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [IndexOf(Func\<MemberDeclarationSyntax, Boolean>)](IndexOf/README.md#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__) | Searches for a member that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(MemberDeclarationSyntax)](IndexOf/README.md#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | The index of the member in the list\. |
| [Insert(Int32, MemberDeclarationSyntax)](Insert/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified member inserted at the index\. |
| [InsertRange(Int32, IEnumerable\<MemberDeclarationSyntax>)](InsertRange/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified members inserted at the index\. |
| [Last()](Last/README.md) | The last member in the list\. |
| [LastIndexOf(Func\<MemberDeclarationSyntax, Boolean>)](LastIndexOf/README.md#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__) | Searches for a member that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(MemberDeclarationSyntax)](LastIndexOf/README.md#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Searches for a member and returns zero\-based index of the last occurrence in the list\. |
| [LastOrDefault()](LastOrDefault/README.md) | The last member in the list or null if the list is empty\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(MemberDeclarationSyntax)](Remove/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified member removed\. |
| [RemoveAt(Int32)](RemoveAt/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the member at the specified index removed\. |
| [RemoveNode(SyntaxNode, SyntaxRemoveOptions)](RemoveNode/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified node removed\. |
| [Replace(MemberDeclarationSyntax, MemberDeclarationSyntax)](Replace/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified member replaced with the new member\. |
| [ReplaceAt(Int32, MemberDeclarationSyntax)](ReplaceAt/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the member at the specified index replaced with a new member\. |
| [ReplaceNode(SyntaxNode, SyntaxNode)](ReplaceNode/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified old node replaced with a new node\. |
| [ReplaceRange(MemberDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>)](ReplaceRange/README.md) | Creates a new [MemberDeclarationListInfo](./README.md) with the specified member replaced with new members\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [WithMembers(IEnumerable\<MemberDeclarationSyntax>)](WithMembers/README.md#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_WithMembers_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [MemberDeclarationListInfo](./README.md) with the members updated\. |
| [WithMembers(SyntaxList\<MemberDeclarationSyntax>)](WithMembers/README.md#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_WithMembers_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [MemberDeclarationListInfo](./README.md) with the members updated\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/README.md) | |
| [IEnumerable\<MemberDeclarationSyntax>.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-CSharp-Syntax-MemberDeclarationSyntax--GetEnumerator/README.md) | |

