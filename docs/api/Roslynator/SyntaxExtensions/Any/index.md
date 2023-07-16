---
sidebar_label: Any
---

# SyntaxExtensions\.Any Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Any(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;)](#3052208275) | Returns true if any token in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\. |
| [Any(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;)](#1292593986) | Returns true if any trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\. |
| [Any&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](#3469033055) | Returns true if any node in a list matches the predicate\. |
| [Any&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;)](#2324683886) | Returns true if any node in a list matches the predicate\. |

<a id="3052208275"></a>

## Any\(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;\) 

  
Returns true if any token in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\.

```csharp
public static bool Any(this Microsoft.CodeAnalysis.SyntaxTokenList list, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate)
```

### Parameters

**list** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="1292593986"></a>

## Any\(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;\) 

  
Returns true if any trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\.

```csharp
public static bool Any(this Microsoft.CodeAnalysis.SyntaxTriviaList list, Func<Microsoft.CodeAnalysis.SyntaxTrivia, bool> predicate)
```

### Parameters

**list** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="3469033055"></a>

## Any&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;\) 

  
Returns true if any node in a list matches the predicate\.

```csharp
public static bool Any<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Func<TNode, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TNode, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2324683886"></a>

## Any&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Func&lt;TNode, Boolean&gt;\) 

  
Returns true if any node in a list matches the predicate\.

```csharp
public static bool Any<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Func<TNode, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;TNode, [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

