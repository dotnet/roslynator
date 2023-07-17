---
sidebar_label: TryGetContainingList
---

# SyntaxExtensions\.TryGetContainingList\(SyntaxTrivia, SyntaxTriviaList, Boolean, Boolean\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Gets a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) the specified trivia is contained in\.

```csharp
public static bool TryGetContainingList(this Microsoft.CodeAnalysis.SyntaxTrivia trivia, out Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, bool allowLeading = true, bool allowTrailing = true)
```

### Parameters

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**allowLeading** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, trivia can be part of leading trivia\.

**allowTrailing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

If true, trivia can be part of trailing trivia\.

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

