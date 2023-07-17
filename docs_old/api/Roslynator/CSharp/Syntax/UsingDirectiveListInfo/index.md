---
sidebar_label: UsingDirectiveListInfo
---

# UsingDirectiveListInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about a list of using directives\.

```csharp
public readonly struct UsingDirectiveListInfo : System.Collections.Generic.IReadOnlyList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; UsingDirectiveListInfo

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

## Indexers

| Indexer | Summary |
| ------- | ------- |
| [Item\[Int32\]](Item/index.md) | Gets the using directive at the specified index in the list\. \(Implements [IReadOnlyList&lt;UsingDirectiveSyntax&gt;.Item](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1.item)\) |

## Properties

| Property | Summary |
| -------- | ------- |
| [Count](Count/index.md) | A number of usings in the list\. |
| [Parent](Parent/index.md) | The declaration that contains the usings\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |
| [Usings](Usings/index.md) | A list of usings\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Add(UsingDirectiveSyntax)](Add/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified using directive added at the end\. |
| [AddRange(IEnumerable&lt;UsingDirectiveSyntax&gt;)](AddRange/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified usings added at the end\. |
| [Any()](Any/index.md) | True if the list has at least one using directive\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [First()](First/index.md) | The first using directive in the list\. |
| [FirstOrDefault()](FirstOrDefault/index.md) | The first using directive in the list or null if the list is empty\. |
| [GetEnumerator()](GetEnumerator/index.md) | Gets the enumerator for the list of usings\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [IndexOf(Func&lt;UsingDirectiveSyntax, Boolean&gt;)](IndexOf/index.md#2836230997) | Searches for an using directive that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(UsingDirectiveSyntax)](IndexOf/index.md#1386691920) | The index of the using directive in the list\. |
| [Insert(Int32, UsingDirectiveSyntax)](Insert/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified using directive inserted at the index\. |
| [InsertRange(Int32, IEnumerable&lt;UsingDirectiveSyntax&gt;)](InsertRange/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified usings inserted at the index\. |
| [Last()](Last/index.md) | The last using directive in the list\. |
| [LastIndexOf(Func&lt;UsingDirectiveSyntax, Boolean&gt;)](LastIndexOf/index.md#3449962221) | Searches for an using directive that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(UsingDirectiveSyntax)](LastIndexOf/index.md#646248764) | Searches for an using directive and returns zero\-based index of the last occurrence in the list\. |
| [LastOrDefault()](LastOrDefault/index.md) | The last using directive in the list or null if the list is empty\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(UsingDirectiveSyntax)](Remove/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified using directive removed\. |
| [RemoveAt(Int32)](RemoveAt/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the using directive at the specified index removed\. |
| [RemoveNode(SyntaxNode, SyntaxRemoveOptions)](RemoveNode/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified node removed\. |
| [Replace(UsingDirectiveSyntax, UsingDirectiveSyntax)](Replace/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified using directive replaced with the new using directive\. |
| [ReplaceAt(Int32, UsingDirectiveSyntax)](ReplaceAt/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the using directive at the specified index replaced with a new using directive\. |
| [ReplaceNode(SyntaxNode, SyntaxNode)](ReplaceNode/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified old node replaced with a new node\. |
| [ReplaceRange(UsingDirectiveSyntax, IEnumerable&lt;UsingDirectiveSyntax&gt;)](ReplaceRange/index.md) | Creates a new [UsingDirectiveListInfo](./index.md) with the specified using directive replaced with new usings\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [WithUsings(IEnumerable&lt;UsingDirectiveSyntax&gt;)](WithUsings/index.md#2973635367) | Creates a new [UsingDirectiveListInfo](./index.md) with the usings updated\. |
| [WithUsings(SyntaxList&lt;UsingDirectiveSyntax&gt;)](WithUsings/index.md#3245135487) | Creates a new [UsingDirectiveListInfo](./index.md) with the usings updated\. |

## Explicit Interface Implementations

| Member | Summary |
| ------ | ------- |
| [IEnumerable.GetEnumerator()](System-Collections-IEnumerable-GetEnumerator/index.md) | |
| [IEnumerable&lt;UsingDirectiveSyntax&gt;.GetEnumerator()](System-Collections-Generic-IEnumerable-Microsoft-CodeAnalysis-CSharp-Syntax-UsingDirectiveSyntax--GetEnumerator/index.md) | |

