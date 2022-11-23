---
sidebar_label: WithSimplifierAnnotation
---

# WorkspaceSyntaxExtensions\.WithSimplifierAnnotation Method

**Containing Type**: [WorkspaceSyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithSimplifierAnnotation/index.md
| [WithSimplifierAnnotation(SyntaxToken)](#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation_Microsoft_CodeAnalysis_SyntaxToken_) | Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\. "Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\. |
| [WithSimplifierAnnotation&lt;TNode&gt;(TNode)](#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation__1___0_) | Creates a new node with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) attached\. |
========
| [WithSimplifierAnnotation(SyntaxToken)](#1047085782) | Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\. "Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\. |
| [WithSimplifierAnnotation\<TNode\>(TNode)](#1448778190) | Creates a new node with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) attached\. |
>>>>>>>> main:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithSimplifierAnnotation/README.md

<a id="1047085782"></a>

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithSimplifierAnnotation/index.md
========
## WithSimplifierAnnotation\(SyntaxToken\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithSimplifierAnnotation/README.md
  
Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\.
"Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithSimplifierAnnotation(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithSimplifierAnnotation/index.md
## WithSimplifierAnnotation&lt;TNode&gt;\(TNode\) <a id="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation__1___0_"></a>

========
<a id="1448778190"></a>

## WithSimplifierAnnotation\<TNode\>\(TNode\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/WorkspaceSyntaxExtensions/WithSimplifierAnnotation/README.md
  
Creates a new node with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) attached\.

```csharp
public static TNode WithSimplifierAnnotation<TNode>(this TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

### Returns

TNode

