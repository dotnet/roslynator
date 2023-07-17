---
sidebar_label: DescendantTrivia
---

# SyntaxExtensions\.DescendantTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](#2025931486) | Get a list of all the trivia associated with the nodes in the list\. |
| [DescendantTrivia&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](#2885561996) | Get a list of all the trivia associated with the nodes in the list\. |

<a id="2025931486"></a>

## DescendantTrivia&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Func&lt;SyntaxNode, Boolean&gt;, Boolean\) 

  
Get a list of all the trivia associated with the nodes in the list\.

```csharp
public static System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> DescendantTrivia<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**descendIntoChildren** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**descendIntoTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

<a id="2885561996"></a>

## DescendantTrivia&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean\) 

  
Get a list of all the trivia associated with the nodes in the list\.

```csharp
public static System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> DescendantTrivia<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.Text.TextSpan span, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**descendIntoChildren** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**descendIntoTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;

