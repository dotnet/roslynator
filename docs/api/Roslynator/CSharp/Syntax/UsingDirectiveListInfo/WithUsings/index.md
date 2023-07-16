---
sidebar_label: WithUsings
---

# UsingDirectiveListInfo\.WithUsings Method

**Containing Type**: [UsingDirectiveListInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithUsings(IEnumerable&lt;UsingDirectiveSyntax&gt;)](#2973635367) | Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\. |
| [WithUsings(SyntaxList&lt;UsingDirectiveSyntax&gt;)](#3245135487) | Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\. |

<a id="2973635367"></a>

## WithUsings\(IEnumerable&lt;UsingDirectiveSyntax&gt;\) 

  
Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\.

```csharp
public Roslynator.CSharp.Syntax.UsingDirectiveListInfo WithUsings(System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings)
```

### Parameters

**usings** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

### Returns

[UsingDirectiveListInfo](../index.md)

<a id="3245135487"></a>

## WithUsings\(SyntaxList&lt;UsingDirectiveSyntax&gt;\) 

  
Creates a new [UsingDirectiveListInfo](../index.md) with the usings updated\.

```csharp
public Roslynator.CSharp.Syntax.UsingDirectiveListInfo WithUsings(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings)
```

### Parameters

**usings** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)&lt;[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)&gt;

### Returns

[UsingDirectiveListInfo](../index.md)

