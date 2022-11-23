---
sidebar_label: UsingStaticDirective
---

# CSharpFactory\.UsingStaticDirective Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [UsingStaticDirective(NameSyntax)](#3383817532) | |
| [UsingStaticDirective(SyntaxToken, SyntaxToken, NameSyntax, SyntaxToken)](#1460243000) | |

<a id="3383817532"></a>

## UsingStaticDirective\(NameSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingStaticDirective(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
```

### Parameters

**name** &ensp; [NameSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namesyntax)

### Returns

[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)

<a id="1460243000"></a>

## UsingStaticDirective\(SyntaxToken, SyntaxToken, NameSyntax, SyntaxToken\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingStaticDirective(Microsoft.CodeAnalysis.SyntaxToken usingKeyword, Microsoft.CodeAnalysis.SyntaxToken staticKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, Microsoft.CodeAnalysis.SyntaxToken semicolonToken)
```

### Parameters

**usingKeyword** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**staticKeyword** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**name** &ensp; [NameSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namesyntax)

**semicolonToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)

