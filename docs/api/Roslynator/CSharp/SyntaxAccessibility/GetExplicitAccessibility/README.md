# SyntaxAccessibility\.GetExplicitAccessibility Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxAccessibility](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetExplicitAccessibility(SyntaxNode)](#Roslynator_CSharp_SyntaxAccessibility_GetExplicitAccessibility_Microsoft_CodeAnalysis_SyntaxNode_) | Returns an explicit accessibility of the specified declaration\. |
| [GetExplicitAccessibility(SyntaxTokenList)](#Roslynator_CSharp_SyntaxAccessibility_GetExplicitAccessibility_Microsoft_CodeAnalysis_SyntaxTokenList_) | Returns an explicit accessibility of the specified modifiers\. |

## GetExplicitAccessibility\(SyntaxNode\) <a id="Roslynator_CSharp_SyntaxAccessibility_GetExplicitAccessibility_Microsoft_CodeAnalysis_SyntaxNode_"></a>

\
Returns an explicit accessibility of the specified declaration\.

```csharp
public static Microsoft.CodeAnalysis.Accessibility GetExplicitAccessibility(Microsoft.CodeAnalysis.SyntaxNode declaration)
```

### Parameters

**declaration** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

## GetExplicitAccessibility\(SyntaxTokenList\) <a id="Roslynator_CSharp_SyntaxAccessibility_GetExplicitAccessibility_Microsoft_CodeAnalysis_SyntaxTokenList_"></a>

\
Returns an explicit accessibility of the specified modifiers\.

```csharp
public static Microsoft.CodeAnalysis.Accessibility GetExplicitAccessibility(Microsoft.CodeAnalysis.SyntaxTokenList modifiers)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

### Returns

[Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

