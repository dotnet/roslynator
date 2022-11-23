---
sidebar_label: VariableDeclaration
---

# CSharpFactory\.VariableDeclaration Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [VariableDeclaration(TypeSyntax, String, ExpressionSyntax)](#506445895) | |
| [VariableDeclaration(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax)](#2135840671) | |
| [VariableDeclaration(TypeSyntax, SyntaxToken, ExpressionSyntax)](#2343218715) | |
| [VariableDeclaration(TypeSyntax, VariableDeclaratorSyntax)](#2881605525) | |

<a id="506445895"></a>

## VariableDeclaration\(TypeSyntax, String, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

<a id="2135840671"></a>

## VariableDeclaration\(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax initializer)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**initializer** &ensp; [EqualsValueClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.equalsvalueclausesyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

<a id="2343218715"></a>

## VariableDeclaration\(TypeSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

<a id="2881605525"></a>

## VariableDeclaration\(TypeSyntax, VariableDeclaratorSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax VariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax variable)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**variable** &ensp; [VariableDeclaratorSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclaratorsyntax)

### Returns

[VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

