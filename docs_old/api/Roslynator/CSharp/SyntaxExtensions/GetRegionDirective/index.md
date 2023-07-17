---
sidebar_label: GetRegionDirective
---

# SyntaxExtensions\.GetRegionDirective\(EndRegionDirectiveTriviaSyntax\) Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Returns region directive that is related to the specified endregion directive\. Returns null if no matching region directive is found\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax GetRegionDirective(this Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax endRegionDirective)
```

### Parameters

**endRegionDirective** &ensp; [EndRegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.endregiondirectivetriviasyntax)

### Returns

[RegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.regiondirectivetriviasyntax)

