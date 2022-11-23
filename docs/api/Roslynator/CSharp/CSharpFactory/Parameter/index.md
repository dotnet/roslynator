---
sidebar_label: Parameter
---

# CSharpFactory\.Parameter Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Parameter(TypeSyntax, String, ExpressionSyntax)](#2103208392) | |
| [Parameter(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax)](#3876330429) | |
| [Parameter(TypeSyntax, SyntaxToken, ExpressionSyntax)](#676376439) | |

<a id="2103208392"></a>

## Parameter\(TypeSyntax, String, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax Parameter(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, string identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax @default = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**default** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[ParameterSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parametersyntax)

<a id="3876330429"></a>

## Parameter\(TypeSyntax, SyntaxToken, EqualsValueClauseSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax Parameter(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax @default)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**default** &ensp; [EqualsValueClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.equalsvalueclausesyntax)

### Returns

[ParameterSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parametersyntax)

<a id="676376439"></a>

## Parameter\(TypeSyntax, SyntaxToken, ExpressionSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax Parameter(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax @default = null)
```

### Parameters

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**identifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**default** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[ParameterSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parametersyntax)

