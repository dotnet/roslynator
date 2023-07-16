---
sidebar_label: IsExpressionInfo
---

# SyntaxInfo\.IsExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsExpressionInfo(BinaryExpressionSyntax, Boolean, Boolean)](#2849571983) | Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/index.md) from the specified binary expression\. |
| [IsExpressionInfo(SyntaxNode, Boolean, Boolean)](#1645034337) | Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/index.md) from the specified node\. |

<a id="2849571983"></a>

## IsExpressionInfo\(BinaryExpressionSyntax, Boolean, Boolean\) 

  
Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/index.md) from the specified binary expression\.

```csharp
public static Roslynator.CSharp.Syntax.IsExpressionInfo IsExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax binaryExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**binaryExpression** &ensp; [BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[IsExpressionInfo](../../Syntax/IsExpressionInfo/index.md)

<a id="1645034337"></a>

## IsExpressionInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.IsExpressionInfo IsExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[IsExpressionInfo](../../Syntax/IsExpressionInfo/index.md)

