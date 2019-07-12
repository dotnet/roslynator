# SyntaxAccessibility\.WithExplicitAccessibility\<TNode>\(TNode, Accessibility, IComparer\<SyntaxKind>\) Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxAccessibility](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

\
Creates a new node with the specified explicit accessibility updated\.

```csharp
public static TNode WithExplicitAccessibility<TNode>(TNode node, Microsoft.CodeAnalysis.Accessibility newAccessibility, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**newAccessibility** &ensp; [Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)>

### Returns

TNode

