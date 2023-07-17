---
sidebar_label: GetTypeSymbol
---

# CSharpExtensions\.GetTypeSymbol Method

**Containing Type**: [CSharpExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetTypeSymbol(SemanticModel, AttributeSyntax, CancellationToken)](#4220455895) | Returns type information about an attribute syntax\. |
| [GetTypeSymbol(SemanticModel, ConstructorInitializerSyntax, CancellationToken)](#2306729789) | Returns type information about a constructor initializer syntax\. |
| [GetTypeSymbol(SemanticModel, ExpressionSyntax, CancellationToken)](#1357550300) | Returns type information about an expression syntax\. |
| [GetTypeSymbol(SemanticModel, SelectOrGroupClauseSyntax, CancellationToken)](#1028976081) | Returns type information about a select or group clause\. |

<a id="4220455895"></a>

## GetTypeSymbol\(SemanticModel, AttributeSyntax, CancellationToken\) 

  
Returns type information about an attribute syntax\.

```csharp
public static Microsoft.CodeAnalysis.ITypeSymbol GetTypeSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax attribute, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**attribute** &ensp; [AttributeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.attributesyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

<a id="2306729789"></a>

## GetTypeSymbol\(SemanticModel, ConstructorInitializerSyntax, CancellationToken\) 

  
Returns type information about a constructor initializer syntax\.

```csharp
public static Microsoft.CodeAnalysis.ITypeSymbol GetTypeSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax constructorInitializer, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**constructorInitializer** &ensp; [ConstructorInitializerSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.constructorinitializersyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

<a id="1357550300"></a>

## GetTypeSymbol\(SemanticModel, ExpressionSyntax, CancellationToken\) 

  
Returns type information about an expression syntax\.

```csharp
public static Microsoft.CodeAnalysis.ITypeSymbol GetTypeSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**expression** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

<a id="1028976081"></a>

## GetTypeSymbol\(SemanticModel, SelectOrGroupClauseSyntax, CancellationToken\) 

  
Returns type information about a select or group clause\.

```csharp
public static Microsoft.CodeAnalysis.ITypeSymbol GetTypeSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.SelectOrGroupClauseSyntax selectOrGroupClause, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**selectOrGroupClause** &ensp; [SelectOrGroupClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.selectorgroupclausesyntax)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol)

