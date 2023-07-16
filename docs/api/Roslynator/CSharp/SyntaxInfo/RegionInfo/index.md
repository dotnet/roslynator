---
sidebar_label: RegionInfo
---

# SyntaxInfo\.RegionInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RegionInfo(EndRegionDirectiveTriviaSyntax)](#1176917118) | Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified endregion directive\. |
| [RegionInfo(RegionDirectiveTriviaSyntax)](#2968553518) | Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified region directive\. |

<a id="1176917118"></a>

## RegionInfo\(EndRegionDirectiveTriviaSyntax\) 

  
Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified endregion directive\.

```csharp
public static Roslynator.CSharp.Syntax.RegionInfo RegionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax endRegionDirective)
```

### Parameters

**endRegionDirective** &ensp; [EndRegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.endregiondirectivetriviasyntax)

### Returns

[RegionInfo](../../Syntax/RegionInfo/index.md)

<a id="2968553518"></a>

## RegionInfo\(RegionDirectiveTriviaSyntax\) 

  
Creates a new [RegionInfo](../../Syntax/RegionInfo/index.md) from the specified region directive\.

```csharp
public static Roslynator.CSharp.Syntax.RegionInfo RegionInfo(Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax regionDirective)
```

### Parameters

**regionDirective** &ensp; [RegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.regiondirectivetriviasyntax)

### Returns

[RegionInfo](../../Syntax/RegionInfo/index.md)

