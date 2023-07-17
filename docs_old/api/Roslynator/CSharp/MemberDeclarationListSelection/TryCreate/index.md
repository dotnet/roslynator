---
sidebar_label: TryCreate
---

# MemberDeclarationListSelection\.TryCreate Method

**Containing Type**: [MemberDeclarationListSelection](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [TryCreate(NamespaceDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](#2633164334) | Creates a new [MemberDeclarationListSelection](../index.md) based on the specified namespace declaration and span\. |
| [TryCreate(TypeDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](#3632170245) | Creates a new [MemberDeclarationListSelection](../index.md) based on the specified type declaration and span\. |

<a id="2633164334"></a>

## TryCreate\(NamespaceDeclarationSyntax, TextSpan, MemberDeclarationListSelection\) 

  
Creates a new [MemberDeclarationListSelection](../index.md) based on the specified namespace declaration and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax namespaceDeclaration, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.CSharp.MemberDeclarationListSelection selectedMembers)
```

### Parameters

**namespaceDeclaration** &ensp; [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selectedMembers** &ensp; [MemberDeclarationListSelection](../index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one member; otherwise, false\.<a id="3632170245"></a>

## TryCreate\(TypeDeclarationSyntax, TextSpan, MemberDeclarationListSelection\) 

  
Creates a new [MemberDeclarationListSelection](../index.md) based on the specified type declaration and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax typeDeclaration, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.CSharp.MemberDeclarationListSelection selectedMembers)
```

### Parameters

**typeDeclaration** &ensp; [TypeDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typedeclarationsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selectedMembers** &ensp; [MemberDeclarationListSelection](../index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one member; otherwise, false\.