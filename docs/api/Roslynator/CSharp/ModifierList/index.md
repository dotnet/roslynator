---
sidebar_label: ModifierList
---

# ModifierList Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of static methods that allows manipulation with modifiers\.

```csharp
public static class ModifierList
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetInsertIndex(SyntaxTokenList, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](GetInsertIndex/index.md#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Returns an index a token with the specified kind should be inserted at\. |
| [GetInsertIndex(SyntaxTokenList, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](GetInsertIndex/index.md#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Returns an index the specified token should be inserted at\. |
| [Insert(SyntaxTokenList, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](Insert/index.md#Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new list of modifiers with the modifier of the specified kind inserted\. |
| [Insert(SyntaxTokenList, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](Insert/index.md#Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new list of modifiers with a specified modifier inserted\. |
| [Insert&lt;TNode&gt;(TNode, SyntaxKind, IComparer&lt;SyntaxKind&gt;)](Insert/index.md#Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert&lt;TNode&gt;(TNode, SyntaxToken, IComparer&lt;SyntaxToken&gt;)](Insert/index.md#Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new node with the specified modifier inserted\. |
| [Remove&lt;TNode&gt;(TNode, SyntaxKind)](Remove/index.md#Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove&lt;TNode&gt;(TNode, SyntaxToken)](Remove/index.md#Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node with the specified modifier removed\. |
| [RemoveAll&lt;TNode&gt;(TNode)](RemoveAll/index.md#Roslynator_CSharp_ModifierList_RemoveAll__1___0_) | Creates a new node with all modifiers removed\. |
| [RemoveAll&lt;TNode&gt;(TNode, Func&lt;SyntaxToken, Boolean&gt;)](RemoveAll/index.md#Roslynator_CSharp_ModifierList_RemoveAll__1___0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAt&lt;TNode&gt;(TNode, Int32)](RemoveAt/index.md) | Creates a new node with a modifier at the specified index removed\. |

