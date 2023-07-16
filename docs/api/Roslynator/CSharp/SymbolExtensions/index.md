---
sidebar_label: SymbolExtensions
---

# SymbolExtensions Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of static methods for [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol) and derived types\.

```csharp
public static class SymbolExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [SupportsConstantValue(ITypeSymbol)](SupportsConstantValue/index.md) | Returns true if the specified type can be used to declare constant value\. |
| [ToMinimalTypeSyntax(INamespaceOrTypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](ToMinimalTypeSyntax/index.md#3588425153) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol |
| [ToMinimalTypeSyntax(INamespaceSymbol, SemanticModel, Int32, SymbolDisplayFormat)](ToMinimalTypeSyntax/index.md#4080868940) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\. |
| [ToMinimalTypeSyntax(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](ToMinimalTypeSyntax/index.md#2161128311) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |
| [ToTypeSyntax(INamespaceOrTypeSymbol, SymbolDisplayFormat)](ToTypeSyntax/index.md#3148432103) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol\. |
| [ToTypeSyntax(INamespaceSymbol, SymbolDisplayFormat)](ToTypeSyntax/index.md#3888206216) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\. |
| [ToTypeSyntax(ITypeSymbol, SymbolDisplayFormat)](ToTypeSyntax/index.md#3779029411) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |

