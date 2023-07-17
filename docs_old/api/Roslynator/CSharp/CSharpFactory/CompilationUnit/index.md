---
sidebar_label: CompilationUnit
---

# CSharpFactory\.CompilationUnit Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [CompilationUnit(MemberDeclarationSyntax)](#3275445737) | |
| [CompilationUnit(SyntaxList&lt;UsingDirectiveSyntax&gt;, MemberDeclarationSyntax)](#813881929) | |
| [CompilationUnit(SyntaxList&lt;UsingDirectiveSyntax&gt;, SyntaxList&lt;MemberDeclarationSyntax&gt;)](#2664110828) | |

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

## CompilationUnit\(SyntaxList&lt;UsingDirectiveSyntax&gt;, MemberDeclarationSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

<a id="2664110828"></a>

## CompilationUnit\(SyntaxList&lt;UsingDirectiveSyntax&gt;, SyntaxList&lt;MemberDeclarationSyntax&gt;\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

**members** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

