---
sidebar_label: IsEmptyOrWhitespace
---

# SyntaxExtensions\.IsEmptyOrWhitespace\(SyntaxTriviaList\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns true if the list of either empty or contains only whitespace \([SyntaxKind.WhitespaceTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.whitespacetrivia) or [SyntaxKind.EndOfLineTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.endoflinetrivia)\)\.

```csharp
public static bool IsEmptyOrWhitespace(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

