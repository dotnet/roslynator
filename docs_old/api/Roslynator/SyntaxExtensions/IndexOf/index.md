---
sidebar_label: IndexOf
---

# SyntaxExtensions\.IndexOf Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;)](#3314040654) | Searches for a token that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [IndexOf(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;)](#2746233850) | Searches for a trivia that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |

<a id="3314040654"></a>

## IndexOf\(SyntaxTokenList, Func&lt;SyntaxToken, Boolean&gt;\) 

  
Searches for a token that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\.

```csharp
public static int IndexOf(this Microsoft.CodeAnalysis.SyntaxTokenList tokens, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate)
```

### Parameters

**tokens** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="2746233850"></a>

## IndexOf\(SyntaxTriviaList, Func&lt;SyntaxTrivia, Boolean&gt;\) 

  
Searches for a trivia that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\.

```csharp
public static int IndexOf(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Func<Microsoft.CodeAnalysis.SyntaxTrivia, bool> predicate)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)&gt;

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

