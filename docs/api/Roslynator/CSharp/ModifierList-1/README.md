# ModifierList\<TNode> Class

[Home](../../../README.md) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Represents a list of modifiers\.

```csharp
public abstract class ModifierList<TNode> where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; ModifierList\<TNode>

## Properties

| Property | Summary |
| -------- | ------- |
| [Instance](Instance/README.md) | Gets an instance of the [ModifierList\<TNode>](./README.md) for a syntax specified by the generic argument\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Insert(TNode, SyntaxKind, IComparer\<SyntaxKind>)](Insert/README.md#Roslynator_CSharp_ModifierList_1_Insert__0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert(TNode, SyntaxToken, IComparer\<SyntaxToken>)](Insert/README.md#Roslynator_CSharp_ModifierList_1_Insert__0_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new node with the specified modifier inserted\. |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [Remove(TNode, SyntaxKind)](Remove/README.md#Roslynator_CSharp_ModifierList_1_Remove__0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove(TNode, SyntaxToken)](Remove/README.md#Roslynator_CSharp_ModifierList_1_Remove__0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node with the specified modifier removed\. |
| [RemoveAll(TNode)](RemoveAll/README.md#Roslynator_CSharp_ModifierList_1_RemoveAll__0_) | Creates a new node with all modifiers removed\. |
| [RemoveAll(TNode, Func\<SyntaxToken, Boolean>)](RemoveAll/README.md#Roslynator_CSharp_ModifierList_1_RemoveAll__0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Creates a new node with modifiers that matches the predicate removed\. |
| [RemoveAt(TNode, Int32)](RemoveAt/README.md) | Creates a new node with a modifier at the specified index removed\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

