---
sidebar_label: FieldDeclaration
---

# CSharpFactory\.FieldDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, String, EqualsValueClauseSyntax)](#1084241695) | |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, String, ExpressionSyntax)](#2192188871) | |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, EqualsValueClauseSyntax)](#2630236285) | |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, ExpressionSyntax)](#1939478107) | |

<a id="1084241695"></a>

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, String, EqualsValueClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax initializer)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**initializer** &ensp; [EqualsValueClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.equalsvalueclausesyntax)

### Returns

[FieldDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.fielddeclarationsyntax)

<a id="2192188871"></a>

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, String, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[FieldDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.fielddeclarationsyntax)

<a id="2630236285"></a>

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, EqualsValueClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax initializer)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**initializer** &ensp; [EqualsValueClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.equalsvalueclausesyntax)

### Returns

[FieldDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.fielddeclarationsyntax)

<a id="1939478107"></a>

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[FieldDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.fielddeclarationsyntax)

