---
sidebar_label: IsFirst
---

# SyntaxExtensions\.IsFirst Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsFirst&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](#1292391442) | Returns true if the specified node is a first node in the list\. |
| [IsFirst&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](#1691317763) | Returns true if the specified node is a first node in the list\. |

<a id="1292391442"></a>

## IsFirst&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, TNode\) 

  
Returns true if the specified node is a first node in the list\.

```csharp
public static bool IsFirst<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1691317763"></a>

## IsFirst&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, TNode\) 

  
Returns true if the specified node is a first node in the list\.

```csharp
public static bool IsFirst<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

