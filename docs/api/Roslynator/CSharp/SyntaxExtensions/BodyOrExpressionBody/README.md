# SyntaxExtensions\.BodyOrExpressionBody Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [BodyOrExpressionBody(AccessorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorDeclarationSyntax_) | Returns accessor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConstructorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConstructorDeclarationSyntax_) | Returns constructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConversionOperatorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConversionOperatorDeclarationSyntax_) | Returns conversion operator body or an expression body if the body is null\. |
| [BodyOrExpressionBody(DestructorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_DestructorDeclarationSyntax_) | Returns destructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(LocalFunctionStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_) | Returns local function body or an expression body if the body is null\. |
| [BodyOrExpressionBody(MethodDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_) | Returns method body or an expression body if the body is null\. |
| [BodyOrExpressionBody(OperatorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_OperatorDeclarationSyntax_) | Returns operator body or an expression body if the body is null\. |

## BodyOrExpressionBody\(AccessorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorDeclarationSyntax_"></a>

\
Returns accessor body or an expression body if the body is null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode BodyOrExpressionBody(this Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax accessorDeclaration)
```

### Parameters

**accessorDeclaration** &ensp; [AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

### Returns

[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(ConstructorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConstructorDeclarationSyntax_"></a>

\
Returns constructor body or an expression body if the body is null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode BodyOrExpressionBody(this Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax constructorDeclaration)
```

### Parameters

**constructorDeclaration** &ensp; [ConstructorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.constructordeclarationsyntax)

### Returns

[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(ConversionOperatorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConversionOperatorDeclarationSyntax_"></a>

\
Returns conversion operator body or an expression body if the body is null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode BodyOrExpressionBody(this Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
```

### Parameters

**conversionOperatorDeclaration** &ensp; [ConversionOperatorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.conversionoperatordeclarationsyntax)

### Returns

[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(DestructorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_DestructorDeclarationSyntax_"></a>

\
Returns destructor body or an expression body if the body is null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode BodyOrExpressionBody(this Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax destructorDeclaration)
```

### Parameters

**destructorDeclaration** &ensp; [DestructorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.destructordeclarationsyntax)

### Returns

[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(LocalFunctionStatementSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_"></a>

\
Returns local function body or an expression body if the body is null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode BodyOrExpressionBody(this Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax localFunctionStatement)
```

### Parameters

**localFunctionStatement** &ensp; [LocalFunctionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localfunctionstatementsyntax)

### Returns

[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(MethodDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_"></a>

\
Returns method body or an expression body if the body is null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode BodyOrExpressionBody(this Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax methodDeclaration)
```

### Parameters

**methodDeclaration** &ensp; [MethodDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.methoddeclarationsyntax)

### Returns

[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(OperatorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_OperatorDeclarationSyntax_"></a>

\
Returns operator body or an expression body if the body is null\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode BodyOrExpressionBody(this Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax operatorDeclaration)
```

### Parameters

**operatorDeclaration** &ensp; [OperatorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.operatordeclarationsyntax)

### Returns

[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

