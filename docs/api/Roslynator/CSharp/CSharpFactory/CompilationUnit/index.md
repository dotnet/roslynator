---
sidebar_label: CompilationUnit
---

# CSharpFactory\.CompilationUnit Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [CompilationUnit(MemberDeclarationSyntax)](#Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | |
| [CompilationUnit(SyntaxList&lt;UsingDirectiveSyntax&gt;, MemberDeclarationSyntax)](#Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | |
| [CompilationUnit(SyntaxList&lt;UsingDirectiveSyntax&gt;, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | |

## CompilationUnit\(MemberDeclarationSyntax\) <a id="Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

## CompilationUnit\(SyntaxList&lt;UsingDirectiveSyntax&gt;, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

## CompilationUnit\(SyntaxList&lt;UsingDirectiveSyntax&gt;, SyntaxList&lt;MemberDeclarationSyntax&gt;\) <a id="Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

