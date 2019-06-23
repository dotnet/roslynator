# SyntaxInverter\.LogicallyInvert Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInverter](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LogicallyInvert(ExpressionSyntax, CancellationToken)](#Roslynator_CSharp_SyntaxInverter_LogicallyInvert_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_System_Threading_CancellationToken_) | Returns new expression that represents logical inversion of the specified expression\. |
| [LogicallyInvert(ExpressionSyntax, SemanticModel, CancellationToken)](#Roslynator_CSharp_SyntaxInverter_LogicallyInvert_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_Microsoft_CodeAnalysis_SemanticModel_System_Threading_CancellationToken_) | Returns new expression that represents logical inversion of the specified expression\. |

## LogicallyInvert\(ExpressionSyntax, CancellationToken\) <a id="Roslynator_CSharp_SyntaxInverter_LogicallyInvert_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_System_Threading_CancellationToken_"></a>

\
Returns new expression that represents logical inversion of the specified expression\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax LogicallyInvert(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

## LogicallyInvert\(ExpressionSyntax, SemanticModel, CancellationToken\) <a id="Roslynator_CSharp_SyntaxInverter_LogicallyInvert_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_Microsoft_CodeAnalysis_SemanticModel_System_Threading_CancellationToken_"></a>

\
Returns new expression that represents logical inversion of the specified expression\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax LogicallyInvert(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SemanticModel semanticModel, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

