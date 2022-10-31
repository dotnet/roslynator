# CSharpFactory\.CompilationUnit Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [CompilationUnit(MemberDeclarationSyntax)](#Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | |
| [CompilationUnit(SyntaxList\<UsingDirectiveSyntax>, MemberDeclarationSyntax)](#Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | |
| [CompilationUnit(SyntaxList\<UsingDirectiveSyntax>, SyntaxList\<MemberDeclarationSyntax>)](#Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | |

## CompilationUnit\(MemberDeclarationSyntax\) <a id="Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

## CompilationUnit\(SyntaxList\<UsingDirectiveSyntax>, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)>

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

## CompilationUnit\(SyntaxList\<UsingDirectiveSyntax>, SyntaxList\<MemberDeclarationSyntax>\) <a id="Roslynator_CSharp_CSharpFactory_CompilationUnit_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)>

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

