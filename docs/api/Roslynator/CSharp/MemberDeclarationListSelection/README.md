# MemberDeclarationListSelection Class

[Home](../../../README.md) &#x2022; [Indexers](#indexers) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Structs](#structs)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Represents selected member declarations in a [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public sealed class MemberDeclarationListSelection : Roslynator.SyntaxListSelection<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md) &#x2192; MemberDeclarationListSelection

### Implements

* [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>
* [IReadOnlyCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>
* [IReadOnlyList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>
* [ISelection](../../ISelection-1/README.md)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

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
| [Parent](Parent/README.md) | Gets a node that contains selected members\. |
| [UnderlyingList](../../SyntaxListSelection-1/UnderlyingList/README.md) | Gets an underlying list that contains selected nodes\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Create(CompilationUnitSyntax, TextSpan)](Create/README.md#Roslynator_CSharp_MemberDeclarationListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_CompilationUnitSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [MemberDeclarationListSelection](./README.md) based on the specified compilation unit and span\. |
| [Create(NamespaceDeclarationSyntax, TextSpan)](Create/README.md#Roslynator_CSharp_MemberDeclarationListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [MemberDeclarationListSelection](./README.md) based on the specified namespace declaration and span\. |
| [Create(TypeDeclarationSyntax, TextSpan)](Create/README.md#Roslynator_CSharp_MemberDeclarationListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_TypeDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [MemberDeclarationListSelection](./README.md) based on the specified type declaration and span\. |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [First()](../../SyntaxListSelection-1/First/README.md) | Gets the first selected node\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [GetEnumerator()](../../SyntaxListSelection-1/GetEnumerator/README.md) | Returns an enumerator that iterates through selected nodes\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Last()](../../SyntaxListSelection-1/Last/README.md) | Gets the last selected node\. \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [TryCreate(NamespaceDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](TryCreate/README.md#Roslynator_CSharp_MemberDeclarationListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_MemberDeclarationListSelection__) | Creates a new [MemberDeclarationListSelection](./README.md) based on the specified namespace declaration and span\. |
| [TryCreate(TypeDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](TryCreate/README.md#Roslynator_CSharp_MemberDeclarationListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_TypeDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_MemberDeclarationListSelection__) | Creates a new [MemberDeclarationListSelection](./README.md) based on the specified type declaration and span\. |

## Structs

| Struct | Summary |
| ------ | ------- |
| [Enumerator](../../SyntaxListSelection-1/Enumerator/README.md) |  \(Inherited from [SyntaxListSelection\<TNode>](../../SyntaxListSelection-1/README.md)\) |

