---
sidebar_label: AsExpressionInfo
---

# SyntaxInfo\.AsExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AsExpressionInfo(BinaryExpressionSyntax, Boolean, Boolean)](#2937610935) | Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/index.md) from the specified binary expression\. |
| [AsExpressionInfo(SyntaxNode, Boolean, Boolean)](#2854397302) | Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/index.md) from the specified node\. |

<a id="2937610935"></a>

## AsExpressionInfo\(BinaryExpressionSyntax, Boolean, Boolean\) 

  
Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/index.md) from the specified binary expression\.

```csharp
public static Roslynator.CSharp.Syntax.AsExpressionInfo AsExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax binaryExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**binaryExpression** &ensp; [BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AsExpressionInfo](../../Syntax/AsExpressionInfo/index.md)

<a id="2854397302"></a>

## AsExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.AsExpressionInfo AsExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AsExpressionInfo](../../Syntax/AsExpressionInfo/index.md)

