---
sidebar_label: ToSeparatedSyntaxList
---

# SyntaxExtensions\.ToSeparatedSyntaxList Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ToSeparatedSyntaxList&lt;TNode&gt;(IEnumerable&lt;SyntaxNodeOrToken&gt;)](#Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNodeOrToken__) | Creates a separated list of syntax nodes from a sequence of nodes and tokens\. |
| [ToSeparatedSyntaxList&lt;TNode&gt;(IEnumerable&lt;TNode&gt;)](#Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable___0__) | Creates a separated list of syntax nodes from a sequence of nodes\. |

## ToSeparatedSyntaxList&lt;TNode&gt;\(IEnumerable&lt;SyntaxNodeOrToken&gt;\) <a id="Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNodeOrToken__"></a>

  
Creates a separated list of syntax nodes from a sequence of nodes and tokens\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxNodeOrToken> nodesAndTokens) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**nodesAndTokens** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)&gt;

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

## ToSeparatedSyntaxList&lt;TNode&gt;\(IEnumerable&lt;TNode&gt;\) <a id="Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable___0__"></a>

  
Creates a separated list of syntax nodes from a sequence of nodes\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this System.Collections.Generic.IEnumerable<TNode> nodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**nodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;TNode&gt;

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

