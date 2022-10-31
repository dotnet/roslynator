# CSharpFactory\.FieldDeclaration Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, String, EqualsValueClauseSyntax)](#Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_System_String_Microsoft_CodeAnalysis_CSharp_Syntax_EqualsValueClauseSyntax_) | |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, String, ExpressionSyntax)](#Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_System_String_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_) | |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, EqualsValueClauseSyntax)](#Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_EqualsValueClauseSyntax_) | |
| [FieldDeclaration(SyntaxTokenList, TypeSyntax, SyntaxToken, ExpressionSyntax)](#Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_) | |

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, String, EqualsValueClauseSyntax\) <a id="Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_System_String_Microsoft_CodeAnalysis_CSharp_Syntax_EqualsValueClauseSyntax_"></a>

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

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, String, ExpressionSyntax\) <a id="Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_System_String_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_"></a>

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

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, EqualsValueClauseSyntax\) <a id="Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_EqualsValueClauseSyntax_"></a>

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

## FieldDeclaration\(SyntaxTokenList, TypeSyntax, SyntaxToken, ExpressionSyntax\) <a id="Roslynator_CSharp_CSharpFactory_FieldDeclaration_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_"></a>

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

