---
sidebar_label: WithoutTrailingTrivia
---

# SyntaxExtensions\.WithoutTrailingTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithoutTrailingTrivia(SyntaxNodeOrToken)](#3602009992) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\. |
| [WithoutTrailingTrivia(SyntaxToken)](#3451371873) | Creates a new token from this token with the trailing trivia removed\. |

<a id="3602009992"></a>

## WithoutTrailingTrivia\(SyntaxNodeOrToken\) 

  
Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxNodeOrToken WithoutTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxNodeOrToken nodeOrToken)
```

### Parameters

**nodeOrToken** &ensp; [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

### Returns

[SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

<a id="3451371873"></a>

## WithoutTrailingTrivia\(SyntaxToken\) 

  
Creates a new token from this token with the trailing trivia removed\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithoutTrailingTrivia(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

