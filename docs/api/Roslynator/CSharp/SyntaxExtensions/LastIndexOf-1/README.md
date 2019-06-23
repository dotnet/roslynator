# SyntaxExtensions\.LastIndexOf Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(SyntaxTriviaList, SyntaxKind)](../LastIndexOf/README.md#Roslynator_CSharp_SyntaxExtensions_LastIndexOf_Microsoft_CodeAnalysis_SyntaxTriviaList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [LastIndexOf\<TNode>(SeparatedSyntaxList\<TNode>, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [LastIndexOf\<TNode>(SyntaxList\<TNode>, SyntaxKind)](#Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |

## LastIndexOf\(SyntaxTriviaList, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_LastIndexOf_Microsoft_CodeAnalysis_SyntaxTriviaList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\.

```csharp
public static int LastIndexOf(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## LastIndexOf\<TNode>\(SeparatedSyntaxList\<TNode>, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## LastIndexOf\<TNode>\(SyntaxList\<TNode>, SyntaxKind\) <a id="Roslynator_CSharp_SyntaxExtensions_LastIndexOf__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_CSharp_SyntaxKind_"></a>

\
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

