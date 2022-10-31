# ModifierList\.Insert Method

[Home](../../../../README.md)

**Containing Type**: [ModifierList\<TNode\>](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Insert(TNode, SyntaxKind, IComparer\<SyntaxKind\>)](#4255247645) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert(TNode, SyntaxToken, IComparer\<SyntaxToken\>)](#2540511869) | Creates a new node with the specified modifier inserted\. |

<a id="4255247645"></a>

## Insert\(TNode, SyntaxKind, IComparer\<SyntaxKind\>\) 

  
Creates a new node with a modifier of the specified kind inserted\.

```csharp
public TNode Insert(TNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null)
```

### Parameters

**node** &ensp; TNode

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)\>

### Returns

TNode

<a id="2540511869"></a>

## Insert\(TNode, SyntaxToken, IComparer\<SyntaxToken\>\) 

  
Creates a new node with the specified modifier inserted\.

```csharp
public TNode Insert(TNode node, Microsoft.CodeAnalysis.SyntaxToken modifier, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.SyntaxToken> comparer = null)
```

### Parameters

**node** &ensp; TNode

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)\>

### Returns

TNode

