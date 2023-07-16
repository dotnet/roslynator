---
sidebar_label: StructDeclaration
---

# CSharpFactory\.StructDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [StructDeclaration(SyntaxTokenList, String, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#3834144922) | |
| [StructDeclaration(SyntaxTokenList, SyntaxToken, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#2002546578) | |

<a id="3834144922"></a>

## StructDeclaration\(SyntaxTokenList, String, SyntaxList&lt;MemberDeclarationSyntax&gt;\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax StructDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, string identifier, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members = default)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

<a id="2002546578"></a>

## StructDeclaration\(SyntaxTokenList, SyntaxToken, SyntaxList&lt;MemberDeclarationSyntax&gt;\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax StructDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members = default)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

