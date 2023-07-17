---
sidebar_label: BaseList
---

# CSharpFactory\.BaseList Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [BaseList(BaseTypeSyntax)](#1558789727) | |
| [BaseList(BaseTypeSyntax\[\])](#1748860841) | |
| [BaseList(SyntaxToken, BaseTypeSyntax)](#1515038236) | |
| [BaseList(SyntaxToken, BaseTypeSyntax\[\])](#3682631255) | |

<a id="1558789727"></a>

## BaseList\(BaseTypeSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax BaseList(Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax type)
```

### Parameters

**type** &ensp; [BaseTypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.basetypesyntax)

### Returns

[BaseListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.baselistsyntax)

<a id="1748860841"></a>

## BaseList\(BaseTypeSyntax\[\]\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax BaseList(params Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax[] types)
```

### Parameters

**types** &ensp; [BaseTypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.basetypesyntax)\[\]

### Returns

[BaseListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.baselistsyntax)

<a id="1515038236"></a>

## BaseList\(SyntaxToken, BaseTypeSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax BaseList(Microsoft.CodeAnalysis.SyntaxToken colonToken, Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax baseType)
```

### Parameters

**colonToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**baseType** &ensp; [BaseTypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.basetypesyntax)

### Returns

[BaseListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.baselistsyntax)

<a id="3682631255"></a>

## BaseList\(SyntaxToken, BaseTypeSyntax\[\]\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax BaseList(Microsoft.CodeAnalysis.SyntaxToken colonToken, params Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax[] types)
```

### Parameters

**colonToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**types** &ensp; [BaseTypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.basetypesyntax)\[\]

### Returns

[BaseListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.baselistsyntax)

