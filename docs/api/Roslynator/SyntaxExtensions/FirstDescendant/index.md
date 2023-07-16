---
sidebar_label: FirstDescendant
---

# SyntaxExtensions\.FirstDescendant Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FirstDescendant&lt;TNode&gt;(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](#3727489774) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendant&lt;TNode&gt;(SyntaxNode, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](#2271502195) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |

<a id="3727489774"></a>

## FirstDescendant&lt;TNode&gt;\(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean\) 

  
Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendant<TNode>(this Microsoft.CodeAnalysis.SyntaxNode node, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**descendIntoChildren** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**descendIntoTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TNode

<a id="2271502195"></a>

## FirstDescendant&lt;TNode&gt;\(SyntaxNode, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean\) 

  
Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendant<TNode>(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Text.TextSpan span, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**descendIntoChildren** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**descendIntoTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TNode

