# SyntaxExtensions\.WithoutLeadingTrivia Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithoutLeadingTrivia(SyntaxNodeOrToken)](#Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\. |
| [WithoutLeadingTrivia(SyntaxToken)](#Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new token from this token with the leading trivia removed\. |

## WithoutLeadingTrivia\(SyntaxNodeOrToken\) <a id="Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_"></a>

\
Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxNodeOrToken WithoutLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxNodeOrToken nodeOrToken)
```

### Parameters

**nodeOrToken** &ensp; [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

### Returns

[SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

## WithoutLeadingTrivia\(SyntaxToken\) <a id="Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Creates a new token from this token with the leading trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithoutLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

