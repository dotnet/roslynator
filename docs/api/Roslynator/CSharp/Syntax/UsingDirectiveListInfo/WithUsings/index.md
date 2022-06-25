---
sidebar_label: WithUsings
---

# UsingDirectiveListInfo\.WithUsings Method

**Containing Type**: [UsingDirectiveListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithUsings(IEnumerable&lt;UsingDirectiveSyntax&gt;)](#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_WithUsings_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__) | Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\. |
| [WithUsings(SyntaxList&lt;UsingDirectiveSyntax&gt;)](#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_WithUsings_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__) | Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\. |

## WithUsings\(IEnumerable&lt;UsingDirectiveSyntax&gt;\) <a id="Roslynator_CSharp_Syntax_UsingDirectiveListInfo_WithUsings_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__"></a>

  
Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\.

```csharp
public Roslynator.CSharp.Syntax.UsingDirectiveListInfo WithUsings(System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings)
```

### Parameters

**usings** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

### Returns

[UsingDirectiveListInfo](../index.md)

## WithUsings\(SyntaxList&lt;UsingDirectiveSyntax&gt;\) <a id="Roslynator_CSharp_Syntax_UsingDirectiveListInfo_WithUsings_Microsoft_CodeAnalysis_SyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax__"></a>

  
Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\.

```csharp
public Roslynator.CSharp.Syntax.UsingDirectiveListInfo WithUsings(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

### Returns

[UsingDirectiveListInfo](../index.md)

