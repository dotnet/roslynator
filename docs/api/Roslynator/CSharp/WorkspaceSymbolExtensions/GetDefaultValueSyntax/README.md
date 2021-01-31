# WorkspaceSymbolExtensions\.GetDefaultValueSyntax Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceSymbolExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetDefaultValueSyntax(ITypeSymbol, DefaultSyntaxOptions, SymbolDisplayFormat)](#Roslynator_CSharp_WorkspaceSymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Roslynator_CSharp_DefaultSyntaxOptions_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |
| [GetDefaultValueSyntax(ITypeSymbol, TypeSyntax, DefaultSyntaxOptions)](#Roslynator_CSharp_WorkspaceSymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Roslynator_CSharp_DefaultSyntaxOptions_) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |

## GetDefaultValueSyntax\(ITypeSymbol, DefaultSyntaxOptions, SymbolDisplayFormat\) <a id="Roslynator_CSharp_WorkspaceSymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Roslynator_CSharp_DefaultSyntaxOptions_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

\
Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax GetDefaultValueSyntax(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Roslynator.CSharp.DefaultSyntaxOptions options = None, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**options** &ensp; [DefaultSyntaxOptions](../../DefaultSyntaxOptions/README.md)

**format** &ensp; [SymbolDisplayFormat](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.symboldisplayformat)

### Returns

[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

## GetDefaultValueSyntax\(ITypeSymbol, TypeSyntax, DefaultSyntaxOptions\) <a id="Roslynator_CSharp_WorkspaceSymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_Roslynator_CSharp_DefaultSyntaxOptions_"></a>

\
Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax GetDefaultValueSyntax(this Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Roslynator.CSharp.DefaultSyntaxOptions options = None)
```

### Parameters

**typeSymbol** &ensp; [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

**type** &ensp; [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

**options** &ensp; [DefaultSyntaxOptions](../../DefaultSyntaxOptions/README.md)

### Returns

[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

