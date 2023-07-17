---
sidebar_label: DelegateDeclaration
---

# CSharpFactory\.DelegateDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [DelegateDeclaration(SyntaxTokenList, TypeSyntax, String, ParameterListSyntax)](#1942786720) | |
| [DelegateDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax)](#1727476607) | |

<a id="1942786720"></a>

## DelegateDeclaration\(SyntaxTokenList, TypeSyntax, String, ParameterListSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax DelegateDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**returnType** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**parameterList** &ensp; [ParameterListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parameterlistsyntax)

### Returns

[DelegateDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.delegatedeclarationsyntax)

<a id="1727476607"></a>

## DelegateDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, ParameterListSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax DelegateDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**returnType** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**parameterList** &ensp; [ParameterListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parameterlistsyntax)

### Returns

[DelegateDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.delegatedeclarationsyntax)

