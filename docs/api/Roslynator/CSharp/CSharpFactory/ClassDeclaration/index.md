---
sidebar_label: ClassDeclaration
---

# CSharpFactory\.ClassDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ClassDeclaration(SyntaxTokenList, String, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#1797779004) | |
| [ClassDeclaration(SyntaxTokenList, SyntaxToken, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#3930390176) | |

<a id="1797779004"></a>

## ClassDeclaration\(SyntaxTokenList, String, SyntaxList&lt;MemberDeclarationSyntax&gt;\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, string identifier, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members = default)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

<a id="3930390176"></a>

## ClassDeclaration\(SyntaxTokenList, SyntaxToken, SyntaxList&lt;MemberDeclarationSyntax&gt;\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members = default)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

