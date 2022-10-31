# SyntaxExtensions\.GetPreprocessingMessageTrivia Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetPreprocessingMessageTrivia(EndRegionDirectiveTriviaSyntax)](#Roslynator_CSharp_SyntaxExtensions_GetPreprocessingMessageTrivia_Microsoft_CodeAnalysis_CSharp_Syntax_EndRegionDirectiveTriviaSyntax_) | Gets preprocessing message for the specified endregion directive if such message exists\. |
| [GetPreprocessingMessageTrivia(RegionDirectiveTriviaSyntax)](#Roslynator_CSharp_SyntaxExtensions_GetPreprocessingMessageTrivia_Microsoft_CodeAnalysis_CSharp_Syntax_RegionDirectiveTriviaSyntax_) | Gets preprocessing message for the specified region directive if such message exists\. |

## GetPreprocessingMessageTrivia\(EndRegionDirectiveTriviaSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_GetPreprocessingMessageTrivia_Microsoft_CodeAnalysis_CSharp_Syntax_EndRegionDirectiveTriviaSyntax_"></a>

\
Gets preprocessing message for the specified endregion directive if such message exists\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTrivia GetPreprocessingMessageTrivia(this Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax endRegionDirective)
```

### Parameters

**endRegionDirective** &ensp; [EndRegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.endregiondirectivetriviasyntax)

### Returns

[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

## GetPreprocessingMessageTrivia\(RegionDirectiveTriviaSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_GetPreprocessingMessageTrivia_Microsoft_CodeAnalysis_CSharp_Syntax_RegionDirectiveTriviaSyntax_"></a>

\
Gets preprocessing message for the specified region directive if such message exists\.

```csharp
public static Microsoft.CodeAnalysis.SyntaxTrivia GetPreprocessingMessageTrivia(this Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax regionDirective)
```

### Parameters

**regionDirective** &ensp; [RegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.regiondirectivetriviasyntax)

### Returns

[SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

