---
sidebar_label: GetExplicitAccessibility
---

# SyntaxAccessibility\.GetExplicitAccessibility Method

**Containing Type**: [SyntaxAccessibility](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetExplicitAccessibility(SyntaxNode)](#2356463790) | Returns an explicit accessibility of the specified declaration\. |
| [GetExplicitAccessibility(SyntaxTokenList)](#1894639516) | Returns an explicit accessibility of the specified modifiers\. |

<a id="2356463790"></a>

## GetExplicitAccessibility\(SyntaxNode\) 

  
Returns an explicit accessibility of the specified declaration\.

```csharp
public static Microsoft.CodeAnalysis.Accessibility GetExplicitAccessibility(Microsoft.CodeAnalysis.SyntaxNode declaration)
```

### Parameters

**declaration** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

<a id="1894639516"></a>

## GetExplicitAccessibility\(SyntaxTokenList\) 

  
Returns an explicit accessibility of the specified modifiers\.

```csharp
public static Microsoft.CodeAnalysis.Accessibility GetExplicitAccessibility(Microsoft.CodeAnalysis.SyntaxTokenList modifiers)
```

### Parameters

**modifiers** &ensp; [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

### Returns

[Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility)

