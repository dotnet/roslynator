---
sidebar_label: MemberDeclarationListInfo
---

# MemberDeclarationListInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about a list of member declaration list\.

```csharp
public readonly struct MemberDeclarationListInfo : System.Collections.Generic.IReadOnlyList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; MemberDeclarationListInfo

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/index.md) | Gets the member at the specified index in the list\. \(Implements [IReadOnlyList&lt;MemberDeclarationSyntax&gt;.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)\) |

## Properties

| Property | Summary |
| -------- | ------- |
| [CloseBraceToken](CloseBraceToken/index.md) | Gets a close brace token\. |
| [Count](Count/index.md) | A number of members in the list\. |
| [Members](Members/index.md) | A list of members\. |
| [OpenBraceToken](OpenBraceToken/index.md) | Gets a open brace token\. |
| [Parent](Parent/index.md) | The declaration that contains the members\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Add(MemberDeclarationSyntax)](Add/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified member added at the end\. |
| [AddRange(IEnumerable&lt;MemberDeclarationSyntax&gt;)](AddRange/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified members added at the end\. |
| [Any()](Any/index.md) | True if the list has at least one member\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [First()](First/index.md) | The first member in the list\. |
| [FirstOrDefault()](FirstOrDefault/index.md) | The first member in the list or null if the list is empty\. |
| [GetEnumerator()](GetEnumerator/index.md) | Gets the enumerator for the list of members\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [IndexOf(Func&lt;MemberDeclarationSyntax, Boolean&gt;)](IndexOf/index.md#442472242) | Searches for a member that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(MemberDeclarationSyntax)](IndexOf/index.md#3381813943) | The index of the member in the list\. |
| [Insert(Int32, MemberDeclarationSyntax)](Insert/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified member inserted at the index\. |
| [InsertRange(Int32, IEnumerable&lt;MemberDeclarationSyntax&gt;)](InsertRange/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified members inserted at the index\. |
| [Last()](Last/index.md) | The last member in the list\. |
| [LastIndexOf(Func&lt;MemberDeclarationSyntax, Boolean&gt;)](LastIndexOf/index.md#2832811949) | Searches for a member that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(MemberDeclarationSyntax)](LastIndexOf/index.md#3105192583) | Searches for a member and returns zero\-based index of the last occurrence in the list\. |
| [LastOrDefault()](LastOrDefault/index.md) | The last member in the list or null if the list is empty\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(MemberDeclarationSyntax)](Remove/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified member removed\. |
| [RemoveAt(Int32)](RemoveAt/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the member at the specified index removed\. |
| [RemoveNode(SyntaxNode, SyntaxRemoveOptions)](RemoveNode/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified node removed\. |
| [Replace(MemberDeclarationSyntax, MemberDeclarationSyntax)](Replace/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified member replaced with the new member\. |
| [ReplaceAt(Int32, MemberDeclarationSyntax)](ReplaceAt/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the member at the specified index replaced with a new member\. |
| [ReplaceNode(SyntaxNode, SyntaxNode)](ReplaceNode/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified old node replaced with a new node\. |
| [ReplaceRange(MemberDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](ReplaceRange/index.md) | Creates a new [MemberDeclarationListInfo](./index.md) with the specified member replaced with new members\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [WithMembers(IEnumerable&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#1171711008) | Creates a new [MemberDeclarationListInfo](./index.md) with the members updated\. |
| [WithMembers(SyntaxList&lt;MemberDeclarationSyntax&gt;)](WithMembers/index.md#30413986) | Creates a new [MemberDeclarationListInfo](./index.md) with the members updated\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;MemberDeclarationSyntax&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-CSharp-Syntax-MemberDeclarationSyntax--GetEnumerator/index.md) | |

