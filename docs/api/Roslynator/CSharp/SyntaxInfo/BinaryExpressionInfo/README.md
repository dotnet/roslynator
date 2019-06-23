# SyntaxInfo\.BinaryExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [BinaryExpressionInfo(BinaryExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_BinaryExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/README.md) from the specified binary expression\. |
| [BinaryExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_BinaryExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/README.md) from the specified node\. |

## BinaryExpressionInfo\(BinaryExpressionSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_BinaryExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/README.md) from the specified binary expression\.

```csharp
public static Roslynator.CSharp.Syntax.BinaryExpressionInfo BinaryExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax binaryExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**binaryExpression** &ensp; [BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/README.md)

## BinaryExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_BinaryExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

\
Creates a new [BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.BinaryExpressionInfo BinaryExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[BinaryExpressionInfo](../../Syntax/BinaryExpressionInfo/README.md)

