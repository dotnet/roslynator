---
sidebar_label: All
---

# SyntaxExtensions\.All Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [All(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;)](#3911797928) | Returns true if all tokens in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\. |
| [All(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;)](#1935784235) | Returns true if all trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\. |
| [All&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](#1104261355) | Returns true if all nodes in a list matches the predicate\. |
| [All&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](#1644057626) | Returns true if all nodes in a list matches the predicate\. |

<a id="3911797928"></a>

## All\(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;\) 

  
Returns true if all tokens in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\.

```csharp
public static bool All(this Microsoft.CodeAnalysis.SyntaxTokenList list, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate)
```

### Parameters

**list** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1935784235"></a>

## All\(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;\) 

  
Returns true if all trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\.

```csharp
public static bool All(this Microsoft.CodeAnalysis.SyntaxTriviaList list, Func<Microsoft.CodeAnalysis.SyntaxTrivia, bool> predicate)
```

### Parameters

**list** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1104261355"></a>

## All&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;\) 

  
Returns true if all nodes in a list matches the predicate\.

```csharp
public static bool All<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Func<TNode, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TNode, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1644057626"></a>

## All&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;\) 

  
Returns true if all nodes in a list matches the predicate\.

```csharp
public static bool All<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Func<TNode, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TNode, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

