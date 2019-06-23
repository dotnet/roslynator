# SyntaxExtensions\.ContainsYield Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ContainsYield(LocalFunctionStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_ContainsYield_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_) | Returns true if the specified local function contains yield statement\. Nested local functions are excluded\. |
| [ContainsYield(MethodDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_ContainsYield_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_) | Returns true if the specified method contains yield statement\. Nested local functions are excluded\. |

## ContainsYield\(LocalFunctionStatementSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ContainsYield_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_"></a>

\
Returns true if the specified local function contains yield statement\. Nested local functions are excluded\.

```csharp
public static bool ContainsYield(this Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax localFunctionStatement)
```

### Parameters

**localFunctionStatement** &ensp; [LocalFunctionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localfunctionstatementsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ContainsYield\(MethodDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_ContainsYield_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_"></a>

\
Returns true if the specified method contains yield statement\. Nested local functions are excluded\.

```csharp
public static bool ContainsYield(this Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax methodDeclaration)
```

### Parameters

**methodDeclaration** &ensp; [MethodDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.methoddeclarationsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

