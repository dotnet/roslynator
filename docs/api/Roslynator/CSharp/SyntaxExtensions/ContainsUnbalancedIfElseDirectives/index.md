---
sidebar_label: ContainsUnbalancedIfElseDirectives
---

# SyntaxExtensions\.ContainsUnbalancedIfElseDirectives Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode, TextSpan)](#1293653798) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode)](#3758239446) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |

<a id="1293653798"></a>

## ContainsUnbalancedIfElseDirectives\(SyntaxNode, TextSpan\) 

  
Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\.

```csharp
public static bool ContainsUnbalancedIfElseDirectives(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="3758239446"></a>

## ContainsUnbalancedIfElseDirectives\(SyntaxNode\) 

  
Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\.

```csharp
public static bool ContainsUnbalancedIfElseDirectives(this Microsoft.CodeAnalysis.SyntaxNode node)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

