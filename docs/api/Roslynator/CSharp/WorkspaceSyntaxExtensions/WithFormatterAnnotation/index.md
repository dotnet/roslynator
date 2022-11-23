---
sidebar_label: WithFormatterAnnotation
---

# WorkspaceSyntaxExtensions\.WithFormatterAnnotation Method

**Containing Type**: [WorkspaceSyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithFormatterAnnotation/index.md
| [WithFormatterAnnotation(SyntaxToken)](#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithFormatterAnnotation_Microsoft_CodeAnalysis_SyntaxToken_) | Adds [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) to the specified token, creating a new token of the same type with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) on it\. |
| [WithFormatterAnnotation&lt;TNode&gt;(TNode)](#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithFormatterAnnotation__1___0_) | Creates a new node with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) attached\. |
========
| [WithFormatterAnnotation(SyntaxToken)](#1202034538) | Adds [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) to the specified token, creating a new token of the same type with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) on it\. |
| [WithFormatterAnnotation\<TNode\>(TNode)](#3493763853) | Creates a new node with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) attached\. |
>>>>>>>> main:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithFormatterAnnotation/README.md

<a id="1202034538"></a>

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithFormatterAnnotation/index.md
========
## WithFormatterAnnotation\(SyntaxToken\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithFormatterAnnotation/README.md
  
Adds [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) to the specified token, creating a new token of the same type with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) on it\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithFormatterAnnotation(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithFormatterAnnotation/index.md
## WithFormatterAnnotation&lt;TNode&gt;\(TNode\) <a id="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithFormatterAnnotation__1___0_"></a>

========
<a id="3493763853"></a>

## WithFormatterAnnotation\<TNode\>\(TNode\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithFormatterAnnotation/README.md
  
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

