---
sidebar_label: LogicallyInvert
---

# SyntaxLogicalInverter\.LogicallyInvert Method

**Containing Type**: [SyntaxLogicalInverter](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LogicallyInvert(ExpressionSyntax, CancellationToken)](#3557862531) | Returns new expression that represents logical inversion of the specified expression\. |
| [LogicallyInvert(ExpressionSyntax, SemanticModel, CancellationToken)](#1171822982) | Returns new expression that represents logical inversion of the specified expression\. |

<a id="3557862531"></a>

## LogicallyInvert\(ExpressionSyntax, CancellationToken\) 

  
Returns new expression that represents logical inversion of the specified expression\.

```csharp
public Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax LogicallyInvert(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

<a id="1171822982"></a>

## LogicallyInvert\(ExpressionSyntax, SemanticModel, CancellationToken\) 

  
Returns new expression that represents logical inversion of the specified expression\.

```csharp
public Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax LogicallyInvert(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SemanticModel semanticModel, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

