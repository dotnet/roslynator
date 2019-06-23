# SyntaxExtensions\.IsKind Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a trivia's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token's kind is one of the specified kinds\. |

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5)
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

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind6)
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

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5)
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

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxToken token, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind6)
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

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a trivia's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxTrivia trivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2)
```

### Parameters

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxTrivia trivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3)
```

### Parameters

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxTrivia trivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4)
```

### Parameters

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxTrivia trivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5)
```

### Parameters

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind5** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_IsKind_Microsoft_CodeAnalysis_SyntaxTrivia_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token's kind is one of the specified kinds\.

```csharp
public static bool IsKind(this Microsoft.CodeAnalysis.SyntaxTrivia trivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind1, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind2, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind3, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind4, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind5, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind6)
```

### Parameters

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**kind1** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind2** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind3** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind4** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind5** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**kind6** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

