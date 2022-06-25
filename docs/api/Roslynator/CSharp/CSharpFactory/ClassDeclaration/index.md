---
sidebar_label: ClassDeclaration
---

# CSharpFactory\.ClassDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ClassDeclaration(SyntaxTokenList, String, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#Roslynator_CSharp_CSharpFactory_ClassDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_System_String_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | |
| [ClassDeclaration(SyntaxTokenList, SyntaxToken, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#Roslynator_CSharp_CSharpFactory_ClassDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | |

## ClassDeclaration\(SyntaxTokenList, String, SyntaxList&lt;MemberDeclarationSyntax&gt;\) <a id="Roslynator_CSharp_CSharpFactory_ClassDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_System_String_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, string identifier, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members = default)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

## ClassDeclaration\(SyntaxTokenList, SyntaxToken, SyntaxList&lt;MemberDeclarationSyntax&gt;\) <a id="Roslynator_CSharp_CSharpFactory_ClassDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ClassDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members = default)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)
