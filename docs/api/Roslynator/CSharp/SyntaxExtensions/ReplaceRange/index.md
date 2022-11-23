---
sidebar_label: ReplaceRange
---

# SyntaxExtensions\.ReplaceRange Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
| [ReplaceRange(SyntaxTokenList, Int32, Int32, IEnumerable&lt;SyntaxToken&gt;)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new list with the tokens in the specified range replaced with new tokens\. |
| [ReplaceRange(SyntaxTriviaList, Int32, Int32, IEnumerable&lt;SyntaxTrivia&gt;)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new list with the trivia in the specified range replaced with new trivia\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32___0_) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange&lt;TNode&gt;(SyntaxList&lt;TNode&gt;, Int32, Int32, TNode)](#Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32___0_) | Creates a new list with the elements in the specified range replaced with new node\. |

## ReplaceRange\(SyntaxTokenList, Int32, Int32, IEnumerable&lt;SyntaxToken&gt;\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxToken__"></a>

========
| [ReplaceRange(SyntaxTokenList, Int32, Int32, IEnumerable\<SyntaxToken\>)](#4257224275) | Creates a new list with the tokens in the specified range replaced with new tokens\. |
| [ReplaceRange(SyntaxTriviaList, Int32, Int32, IEnumerable\<SyntaxTrivia\>)](#4063342571) | Creates a new list with the trivia in the specified range replaced with new trivia\. |
| [ReplaceRange\<TNode\>(SeparatedSyntaxList\<TNode\>, Int32, Int32, IEnumerable\<TNode\>)](#607003656) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange\<TNode\>(SeparatedSyntaxList\<TNode\>, Int32, Int32, TNode)](#2148171151) | Creates a new list with the elements in the specified range replaced with new node\. |
| [ReplaceRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32, IEnumerable\<TNode\>)](#3814604200) | Creates a new list with the elements in the specified range replaced with new nodes\. |
| [ReplaceRange\<TNode\>(SyntaxList\<TNode\>, Int32, Int32, TNode)](#3682382942) | Creates a new list with the elements in the specified range replaced with new node\. |

<a id="4257224275"></a>

## ReplaceRange\(SyntaxTokenList, Int32, Int32, IEnumerable\<SyntaxToken\>\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md
  
Creates a new list with the tokens in the specified range replaced with new tokens\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList ReplaceRange(this Microsoft.CodeAnalysis.SyntaxTokenList list, int index, int count, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxToken> newTokens)
```

### Parameters

**list** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**newTokens** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)&gt;
========
**newTokens** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
## ReplaceRange\(SyntaxTriviaList, Int32, Int32, IEnumerable&lt;SyntaxTrivia&gt;\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_System_Int32_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

========
<a id="4063342571"></a>

## ReplaceRange\(SyntaxTriviaList, Int32, Int32, IEnumerable\<SyntaxTrivia\>\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md
  
Creates a new list with the trivia in the specified range replaced with new trivia\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTriviaList ReplaceRange(this Microsoft.CodeAnalysis.SyntaxTriviaList list, int index, int count, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.SyntaxTrivia> newTrivia)
```

### Parameters

**list** &ensp; [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**newTrivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)&gt;
========
**newTrivia** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md

### Returns

[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
## ReplaceRange&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__"></a>

========
<a id="607003656"></a>

## ReplaceRange\<TNode\>\(SeparatedSyntaxList\<TNode\>, Int32, Int32, IEnumerable\<TNode\>\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md
  
Creates a new list with the elements in the specified range replaced with new nodes\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, int count, System.Collections.Generic.IEnumerable<TNode> newNodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;TNode&gt;

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

## ReplaceRange&lt;TNode&gt;\(SeparatedSyntaxList&lt;TNode&gt;, Int32, Int32, TNode\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32___0_"></a>

========
**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<TNode\>

### Returns

[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>

<a id="2148171151"></a>

## ReplaceRange\<TNode\>\(SeparatedSyntaxList\<TNode\>, Int32, Int32, TNode\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md
  
Creates a new list with the elements in the specified range replaced with new node\.

```csharp
public static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> list, int index, int count, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)&lt;TNode&gt;

## ReplaceRange&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Int32, Int32, IEnumerable&lt;TNode&gt;\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_System_Collections_Generic_IEnumerable___0__"></a>

========
[SeparatedSyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\<TNode\>

<a id="3814604200"></a>

## ReplaceRange\<TNode\>\(SyntaxList\<TNode\>, Int32, Int32, IEnumerable\<TNode\>\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md
  
Creates a new list with the elements in the specified range replaced with new nodes\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, int count, System.Collections.Generic.IEnumerable<TNode> newNodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;TNode&gt;

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;

## ReplaceRange&lt;TNode&gt;\(SyntaxList&lt;TNode&gt;, Int32, Int32, TNode\) <a id="Roslynator_CSharp_SyntaxExtensions_ReplaceRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32___0_"></a>

========
**newNodes** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<TNode\>

### Returns

[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>

<a id="3682382942"></a>

## ReplaceRange\<TNode\>\(SyntaxList\<TNode\>, Int32, Int32, TNode\) 

>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md
  
Creates a new list with the elements in the specified range replaced with new node\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxList<TNode> ReplaceRange<TNode>(this Microsoft.CodeAnalysis.SyntaxList<TNode> list, int index, int count, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
**list** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md

**index** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**newNode** &ensp; TNode

### Returns

<<<<<<<< HEAD:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/index.md
[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;TNode&gt;
========
[SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<TNode\>
>>>>>>>> main:docs/api/Roslynator/CSharp/SyntaxExtensions/ReplaceRange/README.md

