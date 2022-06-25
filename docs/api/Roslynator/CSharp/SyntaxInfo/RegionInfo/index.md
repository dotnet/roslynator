---
sidebar_label: RegionInfo
---

# SyntaxInfo\.RegionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RegionInfo(EndRegionDirectiveTriviaSyntax)](#Roslynator_CSharp_SyntaxInfo_RegionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EndRegionDirectiveTriviaSyntax_) | Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified endregion directive\. |
| [RegionInfo(RegionDirectiveTriviaSyntax)](#Roslynator_CSharp_SyntaxInfo_RegionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_RegionDirectiveTriviaSyntax_) | Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified region directive\. |

## RegionInfo\(EndRegionDirectiveTriviaSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_RegionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EndRegionDirectiveTriviaSyntax_"></a>

  
Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified endregion directive\.

```csharp
public static Roslynator.CSharp.Syntax.RegionInfo RegionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax endRegionDirective)
```

### Parameters

**endRegionDirective** &ensp; [EndRegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.endregiondirectivetriviasyntax)

### Returns

[RegionInfo](../../Syntax/RegionInfo/index.md)

## RegionInfo\(RegionDirectiveTriviaSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_RegionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_RegionDirectiveTriviaSyntax_"></a>

  
Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified region directive\.

```csharp
public static Roslynator.CSharp.Syntax.RegionInfo RegionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax regionDirective)
```

### Parameters

**regionDirective** &ensp; [RegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.regiondirectivetriviasyntax)

### Returns

[RegionInfo](../../Syntax/RegionInfo/index.md)

