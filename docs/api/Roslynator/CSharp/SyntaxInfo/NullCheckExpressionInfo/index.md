---
sidebar_label: NullCheckExpressionInfo
---

# SyntaxInfo\.NullCheckExpressionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [NullCheckExpressionInfo(SyntaxNode, NullCheckStyles, Boolean, Boolean)](#3194305842) | Creates a new [NullCheckExpressionInfo](../../Syntax/NullCheckExpressionInfo/index.md) from the specified node\. |
| [NullCheckExpressionInfo(SyntaxNode, SemanticModel, NullCheckStyles, Boolean, Boolean, CancellationToken)](#2114617976) | Creates a new [NullCheckExpressionInfo](../../Syntax/NullCheckExpressionInfo/index.md) from the specified node\. |

<a id="3194305842"></a>

## NullCheckExpressionInfo\(SyntaxNode, NullCheckStyles, Boolean, Boolean\) 

  
Creates a new [NullCheckExpressionInfo](../../Syntax/NullCheckExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.NullCheckExpressionInfo NullCheckExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, Roslynator.CSharp.NullCheckStyles allowedStyles = ComparisonToNull | IsPattern, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**allowedStyles** &ensp; [NullCheckStyles](../../NullCheckStyles/index.md)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[NullCheckExpressionInfo](../../Syntax/NullCheckExpressionInfo/index.md)

<a id="2114617976"></a>

## NullCheckExpressionInfo\(SyntaxNode, SemanticModel, NullCheckStyles, Boolean, Boolean, CancellationToken\) 

  
Creates a new [NullCheckExpressionInfo](../../Syntax/NullCheckExpressionInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.NullCheckExpressionInfo NullCheckExpressionInfo(Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.SemanticModel semanticModel, Roslynator.CSharp.NullCheckStyles allowedStyles = All, bool walkDownParentheses = true, bool allowMissing = false, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**allowedStyles** &ensp; [NullCheckStyles](../../NullCheckStyles/index.md)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[NullCheckExpressionInfo](../../Syntax/NullCheckExpressionInfo/index.md)

