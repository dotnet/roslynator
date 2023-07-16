---
sidebar_label: SingleLocalDeclarationStatementInfo
---

# SyntaxInfo\.SingleLocalDeclarationStatementInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SingleLocalDeclarationStatementInfo(ExpressionSyntax)](#2527570496) | Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified value\. |
| [SingleLocalDeclarationStatementInfo(LocalDeclarationStatementSyntax, Boolean)](#1835899654) | Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified local declaration statement\. |
| [SingleLocalDeclarationStatementInfo(StatementSyntax, Boolean)](#3792466676) | Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified statement\. |
| [SingleLocalDeclarationStatementInfo(VariableDeclarationSyntax, Boolean)](#2234878655) | Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified variable declaration\. |

<a id="2527570496"></a>

## SingleLocalDeclarationStatementInfo\(ExpressionSyntax\) 

  
Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified value\.

```csharp
public static Roslynator.CSharp.Syntax.SingleLocalDeclarationStatementInfo SingleLocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax value)
```

### Parameters

**value** &ensp; [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

### Returns

[SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md)

<a id="1835899654"></a>

## SingleLocalDeclarationStatementInfo\(LocalDeclarationStatementSyntax, Boolean\) 

  
Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified local declaration statement\.

```csharp
public static Roslynator.CSharp.Syntax.SingleLocalDeclarationStatementInfo SingleLocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax localDeclarationStatement, bool allowMissing = false)
```

### Parameters

**localDeclarationStatement** &ensp; [LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md)

<a id="3792466676"></a>

## SingleLocalDeclarationStatementInfo\(StatementSyntax, Boolean\) 

  
Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified statement\.

```csharp
public static Roslynator.CSharp.Syntax.SingleLocalDeclarationStatementInfo SingleLocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, bool allowMissing = false)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md)

<a id="2234878655"></a>

## SingleLocalDeclarationStatementInfo\(VariableDeclarationSyntax, Boolean\) 

  
Creates a new [SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md) from the specified variable declaration\.

```csharp
public static Roslynator.CSharp.Syntax.SingleLocalDeclarationStatementInfo SingleLocalDeclarationStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax variableDeclaration, bool allowMissing = false)
```

### Parameters

**variableDeclaration** &ensp; [VariableDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.variabledeclarationsyntax)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SingleLocalDeclarationStatementInfo](../../Syntax/SingleLocalDeclarationStatementInfo/index.md)

