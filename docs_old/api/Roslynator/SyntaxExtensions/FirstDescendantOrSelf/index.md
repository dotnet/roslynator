---
sidebar_label: FirstDescendantOrSelf
---

# SyntaxExtensions\.FirstDescendantOrSelf Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FirstDescendantOrSelf&lt;TNode&gt;(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](#4205056015) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendantOrSelf&lt;TNode&gt;(SyntaxNode, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean)](#3421526450) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |

<a id="4205056015"></a>

## FirstDescendantOrSelf&lt;TNode&gt;\(SyntaxNode, Func&lt;SyntaxNode, Boolean&gt;, Boolean\) 

  
Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendantOrSelf<TNode>(this Microsoft.CodeAnalysis.SyntaxNode node, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**descendIntoChildren** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

**descendIntoTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TNode

<a id="3421526450"></a>

## FirstDescendantOrSelf&lt;TNode&gt;\(SyntaxNode, TextSpan, Func&lt;SyntaxNode, Boolean&gt;, Boolean\) 

  
Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendantOrSelf<TNode>(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Text.TextSpan span, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
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

