# WorkspaceSymbolExtensions\.GetDefaultValueSyntax Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceSymbolExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetDefaultValueSyntax(ITypeSymbol, DefaultSyntaxOptions, SymbolDisplayFormat)](#3187258133) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |
| [GetDefaultValueSyntax(ITypeSymbol, TypeSyntax, DefaultSyntaxOptions)](#2331338541) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |

<a id="3187258133"></a>

## GetDefaultValueSyntax\(ITypeSymbol, DefaultSyntaxOptions, SymbolDisplayFormat\) 

  
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

<a id="2331338541"></a>

## GetDefaultValueSyntax\(ITypeSymbol, TypeSyntax, DefaultSyntaxOptions\) 

  
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

