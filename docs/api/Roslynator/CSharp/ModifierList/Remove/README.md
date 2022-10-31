# ModifierList\.Remove Method

[Home](../../../../README.md)

**Containing Type**: [ModifierList](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Remove\<TNode\>(TNode, SyntaxKind)](#2255088376) | Creates a new node with a modifier of the specified kind removed\. |
| [Remove\<TNode\>(TNode, SyntaxToken)](#2345772034) | Creates a new node with the specified modifier removed\. |

<a id="2255088376"></a>

## Remove\<TNode\>\(TNode, SyntaxKind\) 

  
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

<a id="2345772034"></a>

## Remove\<TNode\>\(TNode, SyntaxToken\) 

  
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

