# SyntaxInfo\.AsExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AsExpressionInfo(BinaryExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_AsExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/README.md) from the specified binary expression\. |
| [AsExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_AsExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/README.md) from the specified node\. |

## AsExpressionInfo\(BinaryExpressionSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_AsExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/README.md) from the specified binary expression\.

```csharp
public static Roslynator.CSharp.Syntax.AsExpressionInfo AsExpressionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax binaryExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**binaryExpression** &ensp; [BinaryExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.binaryexpressionsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AsExpressionInfo](../../Syntax/AsExpressionInfo/README.md)

## AsExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_AsExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

\
Creates a new [AsExpressionInfo](../../Syntax/AsExpressionInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.AsExpressionInfo AsExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[AsExpressionInfo](../../Syntax/AsExpressionInfo/README.md)

