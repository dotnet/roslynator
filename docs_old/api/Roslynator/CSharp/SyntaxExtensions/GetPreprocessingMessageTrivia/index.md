---
sidebar_label: GetPreprocessingMessageTrivia
---

# SyntaxExtensions\.GetPreprocessingMessageTrivia Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetPreprocessingMessageTrivia(EndRegionDirectiveTriviaSyntax)](#3549782897) | Gets preprocessing message for the specified endregion directive if such message exists\. |
| [GetPreprocessingMessageTrivia(RegionDirectiveTriviaSyntax)](#1660117599) | Gets preprocessing message for the specified region directive if such message exists\. |

<a id="3549782897"></a>

## GetPreprocessingMessageTrivia\(EndRegionDirectiveTriviaSyntax\) 

  
Gets preprocessing message for the specified endregion directive if such message exists\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTrivia GetPreprocessingMessageTrivia(this Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax endRegionDirective)
```

### Parameters

**endRegionDirective** &ensp; [EndRegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.endregiondirectivetriviasyntax)

### Returns

[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

<a id="1660117599"></a>

## GetPreprocessingMessageTrivia\(RegionDirectiveTriviaSyntax\) 

  
Gets preprocessing message for the specified region directive if such message exists\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTrivia GetPreprocessingMessageTrivia(this Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax regionDirective)
```

### Parameters

**regionDirective** &ensp; [RegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.regiondirectivetriviasyntax)

### Returns

[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

