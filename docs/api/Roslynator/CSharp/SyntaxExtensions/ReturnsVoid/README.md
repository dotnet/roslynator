# SyntaxExtensions\.ReturnsVoid Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReturnsVoid(DelegateDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_DelegateDeclarationSyntax_) | Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(LocalFunctionStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_) | Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(MethodDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_) | Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |

## ReturnsVoid\(DelegateDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_DelegateDeclarationSyntax_"></a>

\
Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax delegateDeclaration)
```

### Parameters

**delegateDeclaration** &ensp; [DelegateDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.delegatedeclarationsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ReturnsVoid\(LocalFunctionStatementSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_"></a>

\
Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax localFunctionStatement)
```

### Parameters

**localFunctionStatement** &ensp; [LocalFunctionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localfunctionstatementsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ReturnsVoid\(MethodDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_"></a>

\
Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax methodDeclaration)
```

### Parameters

**methodDeclaration** &ensp; [MethodDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.methoddeclarationsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

