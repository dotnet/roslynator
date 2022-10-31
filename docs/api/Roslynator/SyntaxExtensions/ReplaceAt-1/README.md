# SyntaxExtensions\.ReplaceAt Method

[Home](../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceAt(SyntaxTokenList, Int32, SyntaxToken)](../ReplaceAt/README.md#Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\. |
| [ReplaceAt(SyntaxTriviaList, Int32, SyntaxTrivia)](../ReplaceAt/README.md#Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\. |
| [ReplaceAt\<TNode>(SeparatedSyntaxList\<TNode>, Int32, TNode)](#Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32___0_) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceAt\<TNode>(SyntaxList\<TNode>, Int32, TNode)](#Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32___0_) | Creates a new list with the node at the specified index replaced with a new node\. |

## ReplaceAt\(SyntaxTokenList, Int32, SyntaxToken\) <a id="Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_Microsoft_CodeAnalysis_SyntaxToken_"></a>

\
Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList ReplaceAt(this Microsoft.CodeAnalysis.SyntaxTokenList tokenList, int index, Microsoft.CodeAnalysis.SyntaxToken newToken)
```

### Parameters

**tokenList** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

## ReplaceAt\(SyntaxTriviaList, Int32, SyntaxTrivia\) <a id="Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

\
Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTriviaList ReplaceAt(this Microsoft.CodeAnalysis.SyntaxTriviaList triviaList, int index, Microsoft.CodeAnalysis.SyntaxTrivia newTrivia)
```

### Parameters

**triviaList** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newTrivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

### Returns

[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

## ReplaceAt\<TNode>\(SeparatedSyntaxList\<TNode>, Int32, TNode\) <a id="Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32___0_"></a>

\
Creates a new list with a node at the specified index replaced with a new node\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceAt<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

## ReplaceAt\<TNode>\(SyntaxList\<TNode>, Int32, TNode\) <a id="Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32___0_"></a>

\
Creates a new list with the node at the specified index replaced with a new node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceAt<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

