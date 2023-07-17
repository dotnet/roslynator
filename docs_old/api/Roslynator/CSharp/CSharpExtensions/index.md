---
sidebar_label: CSharpExtensions
---

# CSharpExtensions Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A set of extension methods for a [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)\.

```csharp
public static class CSharpExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [DetermineParameter(SemanticModel, ArgumentSyntax, Boolean, Boolean, CancellationToken)](DetermineParameter/index.md#547493537) | Determines a parameter symbol that matches to the specified argument\. Returns null if no matching parameter is found\. |
| [DetermineParameter(SemanticModel, AttributeArgumentSyntax, Boolean, Boolean, CancellationToken)](DetermineParameter/index.md#3103958802) | Determines a parameter symbol that matches to the specified attribute argument\. Returns null if not matching parameter is found\. |
| [GetExtensionMethodInfo(SemanticModel, ExpressionSyntax, CancellationToken)](GetExtensionMethodInfo/index.md) | Returns what extension method symbol, if any, the specified expression syntax bound to\. |
| [GetMethodSymbol(SemanticModel, ExpressionSyntax, CancellationToken)](GetMethodSymbol/index.md) | Returns method symbol, if any, the specified expression syntax bound to\. |
| [GetReducedExtensionMethodInfo(SemanticModel, ExpressionSyntax, CancellationToken)](GetReducedExtensionMethodInfo/index.md) | Returns what extension method symbol, if any, the specified expression syntax bound to\. |
| [GetSymbol(SemanticModel, AttributeSyntax, CancellationToken)](GetSymbol/index.md#3570389687) | Returns what symbol, if any, the specified attribute syntax bound to\. |
| [GetSymbol(SemanticModel, ConstructorInitializerSyntax, CancellationToken)](GetSymbol/index.md#3142024581) | Returns what symbol, if any, the specified constructor initializer syntax bound to\. |
| [GetSymbol(SemanticModel, CrefSyntax, CancellationToken)](GetSymbol/index.md#423864560) | Returns what symbol, if any, the specified cref syntax bound to\. |
| [GetSymbol(SemanticModel, ExpressionSyntax, CancellationToken)](GetSymbol/index.md#2073342452) | Returns what symbol, if any, the specified expression syntax bound to\. |
| [GetSymbol(SemanticModel, OrderingSyntax, CancellationToken)](GetSymbol/index.md#1387654106) | Returns what symbol, if any, the specified ordering syntax bound to\. |
| [GetSymbol(SemanticModel, SelectOrGroupClauseSyntax, CancellationToken)](GetSymbol/index.md#2866826046) | Returns what symbol, if any, the specified select or group clause bound to\. |
| [GetTypeSymbol(SemanticModel, AttributeSyntax, CancellationToken)](GetTypeSymbol/index.md#4220455895) | Returns type information about an attribute syntax\. |
| [GetTypeSymbol(SemanticModel, ConstructorInitializerSyntax, CancellationToken)](GetTypeSymbol/index.md#2306729789) | Returns type information about a constructor initializer syntax\. |
| [GetTypeSymbol(SemanticModel, ExpressionSyntax, CancellationToken)](GetTypeSymbol/index.md#1357550300) | Returns type information about an expression syntax\. |
| [GetTypeSymbol(SemanticModel, SelectOrGroupClauseSyntax, CancellationToken)](GetTypeSymbol/index.md#1028976081) | Returns type information about a select or group clause\. |
| [HasConstantValue(SemanticModel, ExpressionSyntax, CancellationToken)](HasConstantValue/index.md) | Returns true if the specified node has a constant value\. |
| [IsDefaultValue(SemanticModel, ITypeSymbol, ExpressionSyntax, CancellationToken)](IsDefaultValue/index.md) | Returns true if the specified expression represents default value of the specified type\. |

