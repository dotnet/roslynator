# WorkspaceSyntaxExtensions\.WithFormatterAnnotation Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceSyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithFormatterAnnotation(SyntaxToken)](../WithFormatterAnnotation/README.md#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithFormatterAnnotation_Microsoft_CodeAnalysis_SyntaxToken_) | Adds [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) to the specified token, creating a new token of the same type with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) on it\. |
| [WithFormatterAnnotation\<TNode>(TNode)](#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithFormatterAnnotation__1___0_) | Creates a new node with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) attached\. |

## WithFormatterAnnotation\(SyntaxToken\) <a id="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithFormatterAnnotation_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Adds [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) to the specified token, creating a new token of the same type with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) on it\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithFormatterAnnotation(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## WithFormatterAnnotation\<TNode>\(TNode\) <a id="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithFormatterAnnotation__1___0_"></a>

\
Creates a new node with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) attached\.

```csharp
public static TNode WithFormatterAnnotation<TNode>(this TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

### Returns

TNode

