# CSharpFactory\.VariableDeclaration Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [VariableDeclaration(TypeSyntax, String, ExpressionSyntax)](#Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_System_String_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_) | |
| [VariableDeclaration(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax)](#Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_EqualsValueClauseSyntax_) | |
| [VariableDeclaration(TypeSyntax, SyntaxToken, ExpressionSyntax)](#Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_) | |
| [VariableDeclaration(TypeSyntax, VariableDeclaratorSyntax)](#Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_VariableDeclaratorSyntax_) | |

## VariableDeclaration\(TypeSyntax, String, ExpressionSyntax\) <a id="Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_System_String_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

## VariableDeclaration\(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax\) <a id="Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_EqualsValueClauseSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax initializer)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**initializer** &ensp; [EqualsValueClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.equalsvalueclausesyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

## VariableDeclaration\(TypeSyntax, SyntaxToken, ExpressionSyntax\) <a id="Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

## VariableDeclaration\(TypeSyntax, VariableDeclaratorSyntax\) <a id="Roslynator_CSharp_CSharpFactory_VariableDeclaration_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_VariableDeclaratorSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax variable)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**variable** &ensp; [VariableDeclaratorSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclaratorsyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

