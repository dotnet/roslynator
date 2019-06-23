# SyntaxExtensions\.Contains Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Contains(SyntaxTokenList, SyntaxKind)](../Contains/README.md#Roslynator_CSharp_SyntaxExtensions_Contains_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a token of the specified kind is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Contains(SyntaxTriviaList, SyntaxKind)](../Contains/README.md#Roslynator_CSharp_SyntaxExtensions_Contains_Microsoft_CodeAnalysis_SyntaxTriviaList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a trivia of the specified kind is in the [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [Contains\<TNode>(SeparatedSyntaxList\<TNode>, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the zero\-based index of the first occurrence within the entire [SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains\<TNode>(SyntaxList\<TNode>, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Returns true if a node of the specified kind is in the [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |

## Contains\(SyntaxTokenList, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_Contains_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a token of the specified kind is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\.

```csharp
public static bool Contains(this Microsoft.CodeAnalysis.SyntaxTokenList tokenList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**tokenList** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## Contains\(SyntaxTriviaList, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_Contains_Microsoft_CodeAnalysis_SyntaxTriviaList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a trivia of the specified kind is in the [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\.

```csharp
public static bool Contains(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## Contains\<TNode>\(SeparatedSyntaxList\<TNode>, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Searches for a node of the specified kind and returns the zero\-based index of the first occurrence within the entire [SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public static bool Contains<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## Contains\<TNode>\(SyntaxList\<TNode>, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Returns true if a node of the specified kind is in the [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static bool Contains<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

