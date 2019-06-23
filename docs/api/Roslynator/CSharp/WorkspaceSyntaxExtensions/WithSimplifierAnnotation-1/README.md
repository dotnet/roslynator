# WorkspaceSyntaxExtensions\.WithSimplifierAnnotation Method

[Home](../../../../README.md)

**Containing Type**: [WorkspaceSyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithSimplifierAnnotation(SyntaxToken)](../WithSimplifierAnnotation/README.md#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation_Microsoft_CodeAnalysis_SyntaxToken_) | Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\. "Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\. |
| [WithSimplifierAnnotation\<TNode>(TNode)](#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation__1___0_) | Creates a new node with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) attached\. |

## WithSimplifierAnnotation\(SyntaxToken\) <a id="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\.
"Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxToken WithSimplifierAnnotation(this Microsoft.CodeAnalysis.SyntaxToken token)
```

### Parameters

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## WithSimplifierAnnotation\<TNode>\(TNode\) <a id="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation__1___0_"></a>

\
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

