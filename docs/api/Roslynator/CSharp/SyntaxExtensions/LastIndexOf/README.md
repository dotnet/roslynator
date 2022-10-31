# SyntaxExtensions\.LastIndexOf Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(SyntaxTriviaList, SyntaxKind)](#2989371063) | Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\. |
| [LastIndexOf\<TNode\>(SeparatedSyntaxList\<TNode\>, SyntaxKind)](#1073548081) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [LastIndexOf\<TNode\>(SyntaxList\<TNode\>, SyntaxKind)](#2386444843) | Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |

<a id="2989371063"></a>

## LastIndexOf\(SyntaxTriviaList, SyntaxKind\) 

  
Searches for a trivia of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)\.

```csharp
public static int LastIndexOf(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="1073548081"></a>

## LastIndexOf\<TNode\>\(SeparatedSyntaxList\<TNode\>, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SeparatedSyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="2386444843"></a>

## LastIndexOf\<TNode\>\(SyntaxList\<TNode\>, SyntaxKind\) 

  
Searches for a node of the specified kind and returns the zero\-based index of the last occurrence within the entire [SyntaxList\<TNode\>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static int LastIndexOf<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

