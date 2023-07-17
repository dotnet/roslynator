---
sidebar_label: LocalDeclarationStatement
---

# CSharpFactory\.LocalDeclarationStatement Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LocalDeclarationStatement(TypeSyntax, String, EqualsValueClauseSyntax)](#692390918) | |
| [LocalDeclarationStatement(TypeSyntax, String, ExpressionSyntax)](#1887855677) | |
| [LocalDeclarationStatement(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax)](#3020861050) | |
| [LocalDeclarationStatement(TypeSyntax, SyntaxToken, ExpressionSyntax)](#3459213941) | |

<a id="692390918"></a>

## LocalDeclarationStatement\(TypeSyntax, String, EqualsValueClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax initializer)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**initializer** &ensp; [EqualsValueClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.equalsvalueclausesyntax)

### Returns

[LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

<a id="1887855677"></a>

## LocalDeclarationStatement\(TypeSyntax, String, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

<a id="3020861050"></a>

## LocalDeclarationStatement\(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax initializer)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**initializer** &ensp; [EqualsValueClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.equalsvalueclausesyntax)

### Returns

[LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

<a id="3459213941"></a>

## LocalDeclarationStatement\(TypeSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax LocalDeclarationStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

