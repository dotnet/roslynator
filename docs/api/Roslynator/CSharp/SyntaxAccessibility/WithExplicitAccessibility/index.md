---
sidebar_label: WithExplicitAccessibility
---

# SyntaxAccessibility\.WithExplicitAccessibility&lt;TNode&gt;\(TNode, Accessibility, IComparer&lt;SyntaxKind&gt;\) Method

**Containing Type**: [SyntaxAccessibility](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Creates a new node with the specified explicit accessibility updated\.

```csharp
public static TNode WithExplicitAccessibility<TNode>(TNode node, Microsoft.CodeAnalysis.Accessibility newAccessibility, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**newAccessibility** &ensp; [Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)&lt;[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)&gt;

### Returns

TNode

