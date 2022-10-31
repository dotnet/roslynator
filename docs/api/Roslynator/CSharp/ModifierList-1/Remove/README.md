# ModifierList\.Remove Method

[Home](../../../../README.md)

**Containing Type**: [ModifierList\<TNode>](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Remove(TNode, SyntaxKind)](#Roslynator_CSharp_ModifierList_1_Remove__0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove(TNode, SyntaxToken)](#Roslynator_CSharp_ModifierList_1_Remove__0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node with the specified modifier removed\. |

## Remove\(TNode, SyntaxKind\) <a id="Roslynator_CSharp_ModifierList_1_Remove__0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Creates a new node with a modifier of the specified kind removed\.

```csharp
public TNode Remove(TNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**node** &ensp; TNode

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

TNode

## Remove\(TNode, SyntaxToken\) <a id="Roslynator_CSharp_ModifierList_1_Remove__0_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Creates a new node with the specified modifier removed\.

```csharp
public TNode Remove(TNode node, Microsoft.CodeAnalysis.SyntaxToken modifier)
```

### Parameters

**node** &ensp; TNode

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

TNode

