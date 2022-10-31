# SyntaxExtensions\.ContainsUnbalancedIfElseDirectives Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode)](#Roslynator_CSharp_SyntaxExtensions_ContainsUnbalancedIfElseDirectives_Microsoft_CodeAnalysis_SyntaxNode_) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |
| [ContainsUnbalancedIfElseDirectives(SyntaxNode, TextSpan)](#Roslynator_CSharp_SyntaxExtensions_ContainsUnbalancedIfElseDirectives_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_Text_TextSpan_) | Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\. |

## ContainsUnbalancedIfElseDirectives\(SyntaxNode\) <a id="Roslynator_CSharp_SyntaxExtensions_ContainsUnbalancedIfElseDirectives_Microsoft_CodeAnalysis_SyntaxNode_"></a>

\
Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\.

```csharp
public static bool ContainsUnbalancedIfElseDirectives(this Microsoft.CodeAnalysis.SyntaxNode node)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ContainsUnbalancedIfElseDirectives\(SyntaxNode, TextSpan\) <a id="Roslynator_CSharp_SyntaxExtensions_ContainsUnbalancedIfElseDirectives_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_Text_TextSpan_"></a>

\
Returns true if the specified node contains `#if` directive but it does not contain related `#endif` directive\.

```csharp
public static bool ContainsUnbalancedIfElseDirectives(this Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

