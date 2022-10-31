# SyntaxExtensions\.IsLast Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsLast\<TNode>(SeparatedSyntaxList\<TNode>, TNode)](#Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_) | Returns true if the specified node is a last node in the list\. |
| [IsLast\<TNode>(SyntaxList\<TNode>, TNode)](#Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SyntaxList___0____0_) | Returns true if the specified node is a last node in the list\. |

## IsLast\<TNode>\(SeparatedSyntaxList\<TNode>, TNode\) <a id="Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_"></a>

\
Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsLast\<TNode>\(SyntaxList\<TNode>, TNode\) <a id="Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SyntaxList___0____0_"></a>

\
Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**node** &ensp; TNode

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

