---
sidebar_label: ToSyntaxList
---

# SyntaxExtensions\.ToSyntaxList&lt;TNode&gt;\(IEnumerable&lt;TNode&gt;\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Creates a list of syntax nodes from a sequence of nodes\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ToSyntaxList<TNode>(this System.Collections.Generic.IEnumerable<TNode> nodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**nodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;TNode&gt;

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

