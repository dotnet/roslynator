# SyntaxExtensions\.IsLast Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsLast\<TNode\>(SeparatedSyntaxList\<TNode\>, TNode)](#3058017669) | Returns true if the specified node is a last node in the list\. |
| [IsLast\<TNode\>(SyntaxList\<TNode\>, TNode)](#554961423) | Returns true if the specified node is a last node in the list\. |

<a id="3058017669"></a>

## IsLast\<TNode\>\(SeparatedSyntaxList\<TNode\>, TNode\) 

  
Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="554961423"></a>

## IsLast\<TNode\>\(SyntaxList\<TNode\>, TNode\) 

  
Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

