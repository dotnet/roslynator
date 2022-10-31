# SyntaxExtensions\.ReplaceRange Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceRange(SyntaxTokenList, Int32, Int32, IEnumerable\<SyntaxToken>)](../ReplaceRange/README.md#Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new list with the tokens in the specified range replaced with new tokens\. |
| [ReplaceRange(SyntaxTriviaList, Int32, Int32, IEnumerable\<SyntaxTrivia>)](../ReplaceRange/README.md#Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new list with the trivia in the specified range replaced with new trivia\. |
| [ReplaceRange\<TNode>(SeparatedSyntaxList\<TNode>, Int32, Int32, IEnumerable\<TNode>)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange\<TNode>(SeparatedSyntaxList\<TNode>, Int32, Int32, TNode)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32___0_) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceRange\<TNode>(SyntaxList\<TNode>, Int32, Int32, IEnumerable\<TNode>)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange\<TNode>(SyntaxList\<TNode>, Int32, Int32, TNode)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32___0_) | Creates a new list with the elements in the specified range replaced with new node\. |

## ReplaceRange\(SyntaxTokenList, Int32, Int32, IEnumerable\<SyntaxToken>\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__"></a>

\
Creates a new list with the tokens in the specified range replaced with new tokens\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList ReplaceRange(this Microsoft.CodeAnalysis.SyntaxTokenList list, int index, int count, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> newTokens)
```

### Parameters

**list** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newTokens** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)>

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

## ReplaceRange\(SyntaxTriviaList, Int32, Int32, IEnumerable\<SyntaxTrivia>\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

\
Creates a new list with the trivia in the specified range replaced with new trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTriviaList ReplaceRange(this Microsoft.CodeAnalysis.SyntaxTriviaList list, int index, int count, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> newTrivia)
```

### Parameters

**list** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newTrivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)>

### Returns

[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

## ReplaceRange\<TNode>\(SeparatedSyntaxList\<TNode>, Int32, Int32, IEnumerable\<TNode>\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__"></a>

\
Creates a new list with the elements in the specified range replaced with new nodes\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, int count, System.Collections.Generic.IEnumerable<TNode> newNodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<TNode>

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

## ReplaceRange\<TNode>\(SeparatedSyntaxList\<TNode>, Int32, Int32, TNode\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32___0_"></a>

\
Creates a new list with the elements in the specified range replaced with new node\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, int count, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode>

## ReplaceRange\<TNode>\(SyntaxList\<TNode>, Int32, Int32, IEnumerable\<TNode>\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__"></a>

\
Creates a new list with the elements in the specified range replaced with new nodes\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, int count, System.Collections.Generic.IEnumerable<TNode> newNodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<TNode>

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

## ReplaceRange\<TNode>\(SyntaxList\<TNode>, Int32, Int32, TNode\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32___0_"></a>

\
Creates a new list with the elements in the specified range replaced with new node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, int count, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode>

