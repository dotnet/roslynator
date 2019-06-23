# ModifierList\.Insert Method

[Home](../../../../README.md)

**Containing Type**: [ModifierList](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Insert(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>)](../Insert/README.md#Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new list of modifiers with the modifier of the specified kind inserted\. |
| [Insert(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>)](../Insert/README.md#Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new list of modifiers with a specified modifier inserted\. |
| [Insert\<TNode>(TNode, SyntaxKind, IComparer\<SyntaxKind>)](#Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert\<TNode>(TNode, SyntaxToken, IComparer\<SyntaxToken>)](#Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new node with the specified modifier inserted\. |

## Insert\(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>\) <a id="Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__"></a>

\
Creates a new list of modifiers with the modifier of the specified kind inserted\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList Insert(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)>

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

## Insert\(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>\) <a id="Roslynator_CSharp_ModifierList_Insert_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__"></a>

\
Creates a new list of modifiers with a specified modifier inserted\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTokenList Insert(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken modifier, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.SyntaxToken> comparer = null)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)>

### Returns

[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

## Insert\<TNode>\(TNode, SyntaxKind, IComparer\<SyntaxKind>\) <a id="Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__"></a>

\
Creates a new node with a modifier of the specified kind inserted\.

```csharp
public static TNode Insert<TNode>(TNode node, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)>

### Returns

TNode

## Insert\<TNode>\(TNode, SyntaxToken, IComparer\<SyntaxToken>\) <a id="Roslynator_CSharp_ModifierList_Insert__1___0_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__"></a>

\
Creates a new node with the specified modifier inserted\.

```csharp
public static TNode Insert<TNode>(TNode node, Microsoft.CodeAnalysis.SyntaxToken modifier, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.SyntaxToken> comparer = null) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node** &ensp; TNode

**modifier** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)>

### Returns

TNode

