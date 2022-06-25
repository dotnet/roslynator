---
sidebar_label: GetEndRegionDirective
---

# SyntaxExtensions\.GetEndRegionDirective\(RegionDirectiveTriviaSyntax\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns endregion directive that is related to the specified region directive\. Returns null if no matching endregion directive is found\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax GetEndRegionDirective(this Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax regionDirective)
```

### Parameters

**regionDirective** &ensp; [RegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.regiondirectivetriviasyntax)

### Returns

[EndRegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.endregiondirectivetriviasyntax)

