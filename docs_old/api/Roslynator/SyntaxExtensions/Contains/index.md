---
sidebar_label: Contains
---

# SyntaxExtensions\.Contains Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Contains(SyntaxTokenList, SyntaxToken)](#4268781350) | Returns true if the specified token is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Contains&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, TNode)](#1049481281) | Returns true if the specified node is in the [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TNode)](#2402474003) | Returns true if the specified node is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |

<a id="4268781350"></a>

## Contains\(SyntaxTokenList, SyntaxToken\) 

  
Returns true if the specified token is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\.

```csharp
public static bool Contains(this Microsoft.CodeAnalysis.SyntaxTokenList tokens, Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**tokens** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1049481281"></a>

## Contains&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, TNode\) 

  
Returns true if the specified node is in the [SeparatedSyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public static bool Contains<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2402474003"></a>

## Contains&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, TNode\) 

  
Returns true if the specified node is in the [SyntaxList&lt;TNode&gt;](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static bool Contains<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

