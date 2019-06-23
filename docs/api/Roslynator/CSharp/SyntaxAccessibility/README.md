# SyntaxAccessibility Class

[Home](../../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
A set of static methods that are related to C\# accessibility\.

```csharp
public static class SyntaxAccessibility
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetAccessibility(SyntaxNode)](GetAccessibility/README.md) | Returns an accessibility of the specified declaration\. |
| [GetDefaultAccessibility(SyntaxNode)](GetDefaultAccessibility/README.md) | Returns a default accessibility of the specified declaration\. |
| [GetDefaultExplicitAccessibility(SyntaxNode)](GetDefaultExplicitAccessibility/README.md) | Returns a default explicit accessibility of the specified declaration\. |
| [GetExplicitAccessibility(SyntaxNode)](GetExplicitAccessibility/README.md#Roslynator_CSharp_SyntaxAccessibility_GetExplicitAccessibility_Microsoft_CodeAnalysis_SyntaxNode_) | Returns an explicit accessibility of the specified declaration\. |
| [GetExplicitAccessibility(SyntaxTokenList)](GetExplicitAccessibility/README.md#Roslynator_CSharp_SyntaxAccessibility_GetExplicitAccessibility_Microsoft_CodeAnalysis_SyntaxTokenList_) | Returns an explicit accessibility of the specified modifiers\. |
| [IsPubliclyVisible(MemberDeclarationSyntax)](IsPubliclyVisible/README.md) | Return true if the specified declaration is publicly visible\. |
| [IsValidAccessibility(SyntaxNode, Accessibility, Boolean)](IsValidAccessibility/README.md) | Returns true if the node can have specified accessibility\. |
| [WithExplicitAccessibility\<TNode>(TNode, Accessibility, IComparer\<SyntaxKind>)](WithExplicitAccessibility-1/README.md) | Creates a new node with the specified explicit accessibility updated\. |
| [WithoutExplicitAccessibility\<TNode>(TNode)](WithoutExplicitAccessibility-1/README.md) | Creates a new node with the explicit accessibility removed\. |

