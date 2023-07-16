---
sidebar_label: BinaryExpressionInfo
---

# SyntaxInfo\.BinaryExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [BinaryExpressionInfo(BinaryExpressionSyntax, Boolean, Boolean)](#3276400696) | Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/index.md) from the specified binary expression\. |
| [BinaryExpressionInfo(SyntaxNode, Boolean, Boolean)](#3948535411) | Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/index.md) from the specified node\. |

<a id="3276400696"></a>

## BinaryExpressionInfo\(BinaryExpressionSyntax, Boolean, Boolean\) 

  
Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/index.md) from the specified binary expression\.

```csharp
public static Roslynator.CSharp.Syntax.BinaryExpressionInfo BinaryExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax binaryExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**binaryExpression** &ensp; [BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/index.md)

<a id="3948535411"></a>

## BinaryExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.BinaryExpressionInfo BinaryExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/index.md)

