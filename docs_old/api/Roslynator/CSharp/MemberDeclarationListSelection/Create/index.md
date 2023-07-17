---
sidebar_label: Create
---

# MemberDeclarationListSelection\.Create Method

**Containing Type**: [MemberDeclarationListSelection](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Create(CompilationUnitSyntax, TextSpan)](#158603944) | Creates a new [MemberDeclarationListSelection](../index.md) based on the specified compilation unit and span\. |
| [Create(NamespaceDeclarationSyntax, TextSpan)](#2965480435) | Creates a new [MemberDeclarationListSelection](../index.md) based on the specified namespace declaration and span\. |
| [Create(TypeDeclarationSyntax, TextSpan)](#3405799454) | Creates a new [MemberDeclarationListSelection](../index.md) based on the specified type declaration and span\. |

<a id="158603944"></a>

## Create\(CompilationUnitSyntax, TextSpan\) 

  
Creates a new [MemberDeclarationListSelection](../index.md) based on the specified compilation unit and span\.

```csharp
public static Roslynator.CSharp.MemberDeclarationListSelection Create(Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**compilationUnit** &ensp; [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[MemberDeclarationListSelection](../index.md)

<a id="2965480435"></a>

## Create\(NamespaceDeclarationSyntax, TextSpan\) 

  
Creates a new [MemberDeclarationListSelection](../index.md) based on the specified namespace declaration and span\.

```csharp
public static Roslynator.CSharp.MemberDeclarationListSelection Create(Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax namespaceDeclaration, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**namespaceDeclaration** &ensp; [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[MemberDeclarationListSelection](../index.md)

<a id="3405799454"></a>

## Create\(TypeDeclarationSyntax, TextSpan\) 

  
Creates a new [MemberDeclarationListSelection](../index.md) based on the specified type declaration and span\.

```csharp
public static Roslynator.CSharp.MemberDeclarationListSelection Create(Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax typeDeclaration, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**typeDeclaration** &ensp; [TypeDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typedeclarationsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[MemberDeclarationListSelection](../index.md)

