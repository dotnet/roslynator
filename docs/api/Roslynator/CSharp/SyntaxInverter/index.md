---
sidebar_label: SyntaxInverter
---

# SyntaxInverter Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

**WARNING: This API is now obsolete\.**

SyntaxInverter is obsolete, use SyntaxLogicalInverter instead\.

  
Provides static methods for syntax inversion\.

```csharp
[Obsolete("SyntaxInverter is obsolete, use SyntaxLogicalInverter instead.")]
public static class SyntaxInverter
```

### Attributes

* [ObsoleteAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.obsoleteattribute)

## Methods

| Method | Summary |
| ------ | ------- |
| [LogicallyInvert(ExpressionSyntax, CancellationToken)](LogicallyInvert/index.md#Roslynator_CSharp_SyntaxInverter_LogicallyInvert_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_System_Threading_CancellationToken_) | Returns new expression that represents logical inversion of the specified expression\. |
| [LogicallyInvert(ExpressionSyntax, SemanticModel, CancellationToken)](LogicallyInvert/index.md#Roslynator_CSharp_SyntaxInverter_LogicallyInvert_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_Microsoft_CodeAnalysis_SemanticModel_System_Threading_CancellationToken_) | Returns new expression that represents logical inversion of the specified expression\. |
