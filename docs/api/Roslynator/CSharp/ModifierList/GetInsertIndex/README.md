# ModifierList\.GetInsertIndex Method

[Home](../../../../README.md)

**Containing Type**: [ModifierList](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetInsertIndex(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>)](#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Returns an index a token with the specified kind should be inserted at\. |
| [GetInsertIndex(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>)](#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Returns an index the specified token should be inserted at\. |

## GetInsertIndex\(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>\) <a id="Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__"></a>

\
Returns an index a token with the specified kind should be inserted at\.

```csharp
public static int GetInsertIndex(Microsoft.CodeAnalysis.SyntaxTokenList tokens, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.CSharp.SyntaxKind> comparer = null)
```

### Parameters

**tokens** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**kind** &ensp; [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind)>

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## GetInsertIndex\(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>\) <a id="Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__"></a>

\
Returns an index the specified token should be inserted at\.

```csharp
public static int GetInsertIndex(Microsoft.CodeAnalysis.SyntaxTokenList tokens, Microsoft.CodeAnalysis.SyntaxToken token, System.Collections.Generic.IComparer<Microsoft.CodeAnalysis.SyntaxToken> comparer = null)
```

### Parameters

**tokens** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**comparer** &ensp; [IComparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1)\<[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)>

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

