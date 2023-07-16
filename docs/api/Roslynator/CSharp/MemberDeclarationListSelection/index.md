---
sidebar_label: MemberDeclarationListSelection
---

# MemberDeclarationListSelection Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Represents selected member declarations in a [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public sealed class MemberDeclarationListSelection : Roslynator.SyntaxListSelection<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md) &#x2192; MemberDeclarationListSelection

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;
* [ISelection](../../ISelection-1/index.md)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

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
| [Parent](Parent/index.md) | Gets a node that contains selected members\. |
| [UnderlyingList](../../SyntaxListSelection-1/UnderlyingList/index.md) | Gets an underlying list that contains selected nodes\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(CompilationUnitSyntax, TextSpan)](Create/index.md#158603944) | Creates a new [MemberDeclarationListSelection](./index.md) based on the specified compilation unit and span\. |
| [Create(NamespaceDeclarationSyntax, TextSpan)](Create/index.md#2965480435) | Creates a new [MemberDeclarationListSelection](./index.md) based on the specified namespace declaration and span\. |
| [Create(TypeDeclarationSyntax, TextSpan)](Create/index.md#3405799454) | Creates a new [MemberDeclarationListSelection](./index.md) based on the specified type declaration and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](../../SyntaxListSelection-1/First/index.md) | Gets the first selected node\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [GetEnumerator()](../../SyntaxListSelection-1/GetEnumerator/index.md) | Returns an enumerator that iterates through selected nodes\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](../../SyntaxListSelection-1/Last/index.md) | Gets the last selected node\. \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(NamespaceDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](TryCreate/index.md#2633164334) | Creates a new [MemberDeclarationListSelection](./index.md) based on the specified namespace declaration and span\. |
| [TryCreate(TypeDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](TryCreate/index.md#3632170245) | Creates a new [MemberDeclarationListSelection](./index.md) based on the specified type declaration and span\. |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](../../SyntaxListSelection-1/Enumerator/index.md) |  \(Inherited from [SyntaxListSelection&lt;TNode&gt;](../../SyntaxListSelection-1/index.md)\) |

