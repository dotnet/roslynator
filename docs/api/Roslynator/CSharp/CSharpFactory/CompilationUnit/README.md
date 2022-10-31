# CSharpFactory\.CompilationUnit Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [CompilationUnit(MemberDeclarationSyntax)](#3275445737) | |
| [CompilationUnit(SyntaxList\<UsingDirectiveSyntax\>, MemberDeclarationSyntax)](#813881929) | |
| [CompilationUnit(SyntaxList\<UsingDirectiveSyntax\>, SyntaxList\<MemberDeclarationSyntax\>)](#2664110828) | |

<a id="3275445737"></a>

## CompilationUnit\(MemberDeclarationSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

<a id="813881929"></a>

## CompilationUnit\(SyntaxList\<UsingDirectiveSyntax\>, MemberDeclarationSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)\>

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

<a id="2664110828"></a>

## CompilationUnit\(SyntaxList\<UsingDirectiveSyntax\>, SyntaxList\<MemberDeclarationSyntax\>\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)\>

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)\>

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

