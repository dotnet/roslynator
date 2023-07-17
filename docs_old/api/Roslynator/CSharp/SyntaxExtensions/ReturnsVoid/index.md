---
sidebar_label: ReturnsVoid
---

# SyntaxExtensions\.ReturnsVoid Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReturnsVoid(DelegateDeclarationSyntax)](#1910724647) | Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(LocalFunctionStatementSyntax)](#3225843639) | Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(MethodDeclarationSyntax)](#2714446372) | Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |

<a id="1910724647"></a>

## ReturnsVoid\(DelegateDeclarationSyntax\) 

  
Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax delegateDeclaration)
```

### Parameters

**delegateDeclaration** &ensp; [DelegateDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.delegatedeclarationsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="3225843639"></a>

## ReturnsVoid\(LocalFunctionStatementSyntax\) 

  
Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax localFunctionStatement)
```

### Parameters

**localFunctionStatement** &ensp; [LocalFunctionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localfunctionstatementsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2714446372"></a>

## ReturnsVoid\(MethodDeclarationSyntax\) 

  
Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax methodDeclaration)
```

### Parameters

**methodDeclaration** &ensp; [MethodDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.methoddeclarationsyntax)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

