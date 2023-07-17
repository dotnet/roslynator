---
sidebar_label: TokenList
---

# CSharpFactory\.TokenList Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [TokenList(Accessibility)](#3748838306) | Creates a list of modifiers from the specified accessibility\. |
| [TokenList(SyntaxKind, SyntaxKind, SyntaxKind)](#1229634363) | |
| [TokenList(SyntaxKind, SyntaxKind)](#2344561993) | |
| [TokenList(SyntaxKind)](#4143969600) | |

<a id="3748838306"></a>

## TokenList\(Accessibility\) 

  
Creates a list of modifiers from the specified accessibility\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList TokenList(Microsoft.CodeAnalysis.Accessibility accessibility)
```

### Parameters

**accessibility** &ensp; [Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="1229634363"></a>

## TokenList\(SyntaxKind, SyntaxKind, SyntaxKind\) 

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList TokenList(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3)
```

### Parameters

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="2344561993"></a>

## TokenList\(SyntaxKind, SyntaxKind\) 

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList TokenList(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2)
```

### Parameters

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<a id="4143969600"></a>

## TokenList\(SyntaxKind\) 

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList TokenList(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

