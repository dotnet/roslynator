# SyntaxExtensions\.WithoutTrailingTrivia Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithoutTrailingTrivia(SyntaxNodeOrToken)](#Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\. |
| [WithoutTrailingTrivia(SyntaxToken)](#Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new token from this token with the trailing trivia removed\. |

## WithoutTrailingTrivia\(SyntaxNodeOrToken\) <a id="Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_"></a>

\
Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxNodeOrToken WithoutTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxNodeOrToken nodeOrToken)
```

### Parameters

**nodeOrToken** &ensp; [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

### Returns

[SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

## WithoutTrailingTrivia\(SyntaxToken\) <a id="Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Creates a new token from this token with the trailing trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithoutTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

