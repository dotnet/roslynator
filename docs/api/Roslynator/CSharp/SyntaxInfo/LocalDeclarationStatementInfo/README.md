# SyntaxInfo\.LocalDeclarationStatementInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LocalDeclarationStatementInfo(ExpressionSyntax, Boolean)](#Roslynator_CSharp_SyntaxInfo_LocalDeclarationStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_System_Boolean_) | Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/README.md) from the specified expression\. |
| [LocalDeclarationStatementInfo(LocalDeclarationStatementSyntax, Boolean)](#Roslynator_CSharp_SyntaxInfo_LocalDeclarationStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LocalDeclarationStatementSyntax_System_Boolean_) | Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/README.md) from the specified local declaration statement\. |

## LocalDeclarationStatementInfo\(ExpressionSyntax, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_LocalDeclarationStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax_System_Boolean_"></a>

\
Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/README.md) from the specified expression\.

```csharp
public static Roslynator.CSharp.Syntax.LocalDeclarationStatementInfo LocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value, bool allowMissing = false)
```

### Parameters

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/README.md)

## LocalDeclarationStatementInfo\(LocalDeclarationStatementSyntax, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_LocalDeclarationStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LocalDeclarationStatementSyntax_System_Boolean_"></a>

\
Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/README.md) from the specified local declaration statement\.

```csharp
public static Roslynator.CSharp.Syntax.LocalDeclarationStatementInfo LocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax localDeclarationStatement, bool allowMissing = false)
```

### Parameters

**localDeclarationStatement** &ensp; [LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/README.md)

