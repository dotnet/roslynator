---
sidebar_label: SyntaxAccessibility
---

# SyntaxAccessibility Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of static methods that are related to C\# accessibility\.

```csharp
public static class SyntaxAccessibility
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetAccessibility(SyntaxNode)](GetAccessibility/index.md) | Returns an accessibility of the specified declaration\. |
| [GetDefaultAccessibility(SyntaxNode)](GetDefaultAccessibility/index.md) | Returns a default accessibility of the specified declaration\. |
| [GetDefaultExplicitAccessibility(SyntaxNode)](GetDefaultExplicitAccessibility/index.md) | Returns a default explicit accessibility of the specified declaration\. |
| [GetExplicitAccessibility(SyntaxNode)](GetExplicitAccessibility/index.md#2356463790) | Returns an explicit accessibility of the specified declaration\. |
| [GetExplicitAccessibility(SyntaxTokenList)](GetExplicitAccessibility/index.md#1894639516) | Returns an explicit accessibility of the specified modifiers\. |
| [IsPubliclyVisible(MemberDeclarationSyntax)](IsPubliclyVisible/index.md) | Return true if the specified declaration is publicly visible\. |
| [IsValidAccessibility(SyntaxNode, Accessibility, Boolean)](IsValidAccessibility/index.md) | Returns true if the node can have specified accessibility\. |
| [WithExplicitAccessibility&lt;TNode&gt;(TNode, Accessibility, IComparer&lt;SyntaxKind&gt;)](WithExplicitAccessibility/index.md) | Creates a new node with the specified explicit accessibility updated\. |
| [WithoutExplicitAccessibility&lt;TNode&gt;(TNode)](WithoutExplicitAccessibility/index.md) | Creates a new node with the explicit accessibility removed\. |

