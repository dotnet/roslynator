# ModifierList Class

[Home](../../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
A set of static methods that allows manipulation with modifiers\.

```csharp
public static class ModifierList
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetInsertIndex(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>)](GetInsertIndex/README.md#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Returns an index a token with the specified kind should be inserted at\. |
| [GetInsertIndex(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>)](GetInsertIndex/README.md#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Returns an index the specified token should be inserted at\. |
| [Insert(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>)](Insert/README.md#Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new list of modifiers with the modifier of the specified kind inserted\. |
| [Insert(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>)](Insert/README.md#Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new list of modifiers with a specified modifier inserted\. |
| [Insert\<TNode>(TNode, SyntaxKind, IComparer\<SyntaxKind>)](Insert-1/README.md#Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert\<TNode>(TNode, SyntaxToken, IComparer\<SyntaxToken>)](Insert-1/README.md#Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new node with the specified modifier inserted\. |
| [Remove\<TNode>(TNode, SyntaxKind)](Remove-1/README.md#Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove\<TNode>(TNode, SyntaxToken)](Remove-1/README.md#Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node with the specified modifier removed\. |
| [RemoveAll\<TNode>(TNode)](RemoveAll-1/README.md#Roslynator_CSharp_ModifierList_RemoveAll__1___0_) | Creates a new node with all modifiers removed\. |
| [RemoveAll\<TNode>(TNode, Func\<SyntaxToken, Boolean>)](RemoveAll-1/README.md#Roslynator_CSharp_ModifierList_RemoveAll__1___0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAt\<TNode>(TNode, Int32)](RemoveAt-1/README.md) | Creates a new node with a modifier at the specified index removed\. |

