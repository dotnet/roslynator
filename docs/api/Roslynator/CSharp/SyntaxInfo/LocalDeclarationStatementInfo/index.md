---
sidebar_label: LocalDeclarationStatementInfo
---

# SyntaxInfo\.LocalDeclarationStatementInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LocalDeclarationStatementInfo(ExpressionSyntax, Boolean)](#1217906988) | Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/index.md) from the specified expression\. |
| [LocalDeclarationStatementInfo(LocalDeclarationStatementSyntax, Boolean)](#168424774) | Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/index.md) from the specified local declaration statement\. |

<a id="1217906988"></a>

## LocalDeclarationStatementInfo\(ExpressionSyntax, Boolean\) 

  
Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/index.md) from the specified expression\.

```csharp
public static Roslynator.CSharp.Syntax.LocalDeclarationStatementInfo LocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value, bool allowMissing = false)
```

### Parameters

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/index.md)

<a id="168424774"></a>

## LocalDeclarationStatementInfo\(LocalDeclarationStatementSyntax, Boolean\) 

  
Creates a new [LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/index.md) from the specified local declaration statement\.

```csharp
public static Roslynator.CSharp.Syntax.LocalDeclarationStatementInfo LocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax localDeclarationStatement, bool allowMissing = false)
```

### Parameters

**localDeclarationStatement** &ensp; [LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[LocalDeclarationStatementInfo](../../Syntax/LocalDeclarationStatementInfo/index.md)

