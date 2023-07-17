---
sidebar_label: IsKind
---

# SyntaxExtensions\.IsKind Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#1975403005) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#3028059531) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#580360109) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind)](#3185459241) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxNode, SyntaxKind, SyntaxKind)](#1541609469) | Returns true if a node's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#3094724497) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#3356796170) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#907715724) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind)](#3793139866) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxToken, SyntaxKind, SyntaxKind)](#3033402641) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#2923191543) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#3366590052) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind)](#839254370) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind)](#3487796432) | Returns true if a token's kind is one of the specified kinds\. |
| [IsKind(SyntaxTrivia, SyntaxKind, SyntaxKind)](#94153864) | Returns true if a trivia's kind is one of the specified kinds\. |

<a id="1975403005"></a>

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="3028059531"></a>

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="580360109"></a>

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="3185459241"></a>

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="1541609469"></a>

## IsKind\(SyntaxNode, SyntaxKind, SyntaxKind\) 

  
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

<a id="3094724497"></a>

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="3356796170"></a>

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="907715724"></a>

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="3793139866"></a>

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="3033402641"></a>

## IsKind\(SyntaxToken, SyntaxKind, SyntaxKind\) 

  
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

<a id="2923191543"></a>

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="3366590052"></a>

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="839254370"></a>

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="3487796432"></a>

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind, SyntaxKind\) 

  
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

<a id="94153864"></a>

## IsKind\(SyntaxTrivia, SyntaxKind, SyntaxKind\) 

  
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

