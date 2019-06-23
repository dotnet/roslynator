# SyntaxExtensions\.IsParentKind Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsParentKind(SyntaxNode, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node parent's kind is the specified kind\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token parent's kind is the specified kind\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token parent's kind is one of the specified kinds\. |
| [IsParentKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token parent's kind is one of the specified kinds\. |

## IsParentKind\(SyntaxNode, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node parent's kind is the specified kind\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxNode, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind5** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind6)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind5** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind6** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxToken, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token parent's kind is the specified kind\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxToken, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind5** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParentKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsParentKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token parent's kind is one of the specified kinds\.

```csharp
public static bool IsParentKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind6)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind5** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind6** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

