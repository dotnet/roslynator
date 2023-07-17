---
sidebar_label: WithoutLeadingTrivia
---

# SyntaxExtensions\.WithoutLeadingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithoutLeadingTrivia(SyntaxNodeOrToken)](#3431085438) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\. |
| [WithoutLeadingTrivia(SyntaxToken)](#43937718) | Creates a new token from this token with the leading trivia removed\. |

<a id="3431085438"></a>

## WithoutLeadingTrivia\(SyntaxNodeOrToken\) 

  
Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxNodeOrToken WithoutLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxNodeOrToken nodeOrToken)
```

### Parameters

**nodeOrToken** &ensp; [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

### Returns

[SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

<a id="43937718"></a>

## WithoutLeadingTrivia\(SyntaxToken\) 

  
Creates a new token from this token with the leading trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithoutLeadingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

