---
sidebar_label: IsLast
---

# SyntaxExtensions\.IsLast Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsLast&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](#Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_) | Returns true if the specified node is a last node in the list\. |
| [IsLast&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](#Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SyntaxList___0____0_) | Returns true if the specified node is a last node in the list\. |

## IsLast&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, TNode\) <a id="Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_"></a>

  
Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsLast&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, TNode\) <a id="Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SyntaxList___0____0_"></a>

  
Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

