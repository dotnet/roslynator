---
sidebar_label: EventDeclaration
---

# CSharpFactory\.EventDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [EventDeclaration(SyntaxTokenList, TypeSyntax, String, AccessorListSyntax)](#3484208712) | |
| [EventDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, AccessorListSyntax)](#3312331935) | |

<a id="3484208712"></a>

## EventDeclaration\(SyntaxTokenList, TypeSyntax, String, AccessorListSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**accessorList** &ensp; [AccessorListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessorlistsyntax)

### Returns

[EventDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.eventdeclarationsyntax)

<a id="3312331935"></a>

## EventDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, AccessorListSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax EventDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**accessorList** &ensp; [AccessorListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessorlistsyntax)

### Returns

[EventDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.eventdeclarationsyntax)

