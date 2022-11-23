---
sidebar_label: Remove
---

# ModifierList\.Remove Method

**Containing Type**: [ModifierList](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/ModifierList/Remove/index.md
| [Remove&lt;TNode&gt;(TNode, SyntaxKind)](#Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove&lt;TNode&gt;(TNode, SyntaxToken)](#Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node with the specified modifier removed\. |

## Remove&lt;TNode&gt;\(TNode, SyntaxKind\) <a id="Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

========
| [Remove\<TNode\>(TNode, SyntaxKind)](#2255088376) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove\<TNode\>(TNode, SyntaxToken)](#2345772034) | Creates a new node with the specified modifier removed\. |

<a id="2255088376"></a>

## Remove\<TNode\>\(TNode, SyntaxKind\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/ModifierList/Remove/README.md
  
Creates a new node with a modifier of the specified kind removed\.

```csharp
public static TNode Remove<TNode>(TNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

TNode

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/ModifierList/Remove/index.md
## Remove&lt;TNode&gt;\(TNode, SyntaxToken\) <a id="Roslynator_CSharp_ModifierList_Remove__1___0_Microsoft_CodeAnalysis_SyntaxToken_"></a>

========
<a id="2345772034"></a>

## Remove\<TNode\>\(TNode, SyntaxToken\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/ModifierList/Remove/README.md
  
Creates a new node with the specified modifier removed\.

```csharp
public static TNode Remove<TNode>(TNode node, Microsoft.CodeAnalysis.SyntaxToken modifier) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

TNode

