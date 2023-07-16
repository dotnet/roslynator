---
sidebar_label: WorkspaceSyntaxExtensions
---

# WorkspaceSyntaxExtensions Class

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
A set of extension methods for syntax\. These methods are dependent on the workspace layer\.

```csharp
public static class WorkspaceSyntaxExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [Parenthesize(ExpressionSyntax, Boolean, Boolean)](Parenthesize/index.md) | Creates parenthesized expression that is parenthesizing the specified expression\. |
| [WithFormatterAnnotation(SyntaxToken)](WithFormatterAnnotation/index.md#1202034538) | Adds [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) to the specified token, creating a new token of the same type with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) on it\. |
| [WithFormatterAnnotation&lt;TNode&gt;(TNode)](WithFormatterAnnotation/index.md#3493763853) | Creates a new node with the [Formatter.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.formatting.formatter.annotation) attached\. |
| [WithNavigationAnnotation(SyntaxToken)](WithNavigationAnnotation/index.md) | Adds navigation annotation to the specified token, creating a new token of the same type with the navigation annotation on it\. Navigation annotation allows to mark a token that should be selected after the code action is applied\. |
| [WithRenameAnnotation(SyntaxToken)](WithRenameAnnotation/index.md) | Adds "rename" annotation to the specified token, creating a new token of the same type with the "rename" annotation on it\. "Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\. |
| [WithSimplifierAnnotation(SyntaxToken)](WithSimplifierAnnotation/index.md#1047085782) | Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\. "Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\. |
| [WithSimplifierAnnotation&lt;TNode&gt;(TNode)](WithSimplifierAnnotation/index.md#1448778190) | Creates a new node with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) attached\. |

