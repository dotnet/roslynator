# SyntaxExtensions\.IndexOf Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(SyntaxTokenList, Func\<SyntaxToken, Boolean>)](#Roslynator_SyntaxExtensions_IndexOf_Microsoft_CodeAnalysis_SyntaxTokenList_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Searches for a token that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [IndexOf(SyntaxTriviaList, Func\<SyntaxTrivia, Boolean>)](#Roslynator_SyntaxExtensions_IndexOf_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Func_Microsoft_CodeAnalysis_SyntaxTrivia_System_Boolean__) | Searches for a trivia that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |

## IndexOf\(SyntaxTokenList, Func\<SyntaxToken, Boolean>\) <a id="Roslynator_SyntaxExtensions_IndexOf_Microsoft_CodeAnalysis_SyntaxTokenList_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__"></a>

\
Searches for a token that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\.

```csharp
public static int IndexOf(this Microsoft.CodeAnalysis.SyntaxTokenList tokens, Func<Microsoft.CodeAnalysis.SyntaxToken, bool> predicate)
```

### Parameters

**tokens** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)>

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## IndexOf\(SyntaxTriviaList, Func\<SyntaxTrivia, Boolean>\) <a id="Roslynator_SyntaxExtensions_IndexOf_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Func_Microsoft_CodeAnalysis_SyntaxTrivia_System_Boolean__"></a>

\
Searches for a trivia that matches the predicate and returns the zero\-based index of the first occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\.

```csharp
public static int IndexOf(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Func<Microsoft.CodeAnalysis.SyntaxTrivia, bool> predicate)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)>

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

