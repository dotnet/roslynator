# SyntaxExtensions\.FirstDescendantOrSelf Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FirstDescendantOrSelf\<TNode\>(SyntaxNode, Func\<SyntaxNode, Boolean\>, Boolean)](#4205056015) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendantOrSelf\<TNode\>(SyntaxNode, TextSpan, Func\<SyntaxNode, Boolean\>, Boolean)](#3421526450) | Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\. |

<a id="4205056015"></a>

## FirstDescendantOrSelf\<TNode\>\(SyntaxNode, Func\<SyntaxNode, Boolean\>, Boolean\) 

  
Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendantOrSelf<TNode>(this Microsoft.CodeAnalysis.SyntaxNode node, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**descendIntoChildren** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>

**descendIntoTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TNode

<a id="3421526450"></a>

## FirstDescendantOrSelf\<TNode\>\(SyntaxNode, TextSpan, Func\<SyntaxNode, Boolean\>, Boolean\) 

  
Searches a list of descendant nodes \(including this node\) in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendantOrSelf<TNode>(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Text.TextSpan span, Func<Microsoft.CodeAnalysis.SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**descendIntoChildren** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>

**descendIntoTrivia** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

TNode

