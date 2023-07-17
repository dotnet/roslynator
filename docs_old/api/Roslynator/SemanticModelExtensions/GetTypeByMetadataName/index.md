---
sidebar_label: GetTypeByMetadataName
---

# SemanticModelExtensions\.GetTypeByMetadataName\(SemanticModel, String\) Method

**Containing Type**: [SemanticModelExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns the type within the compilation's assembly using its canonical CLR metadata name\.

```csharp
public static Microsoft.CodeAnalysis.INamedTypeSymbol GetTypeByMetadataName(this Microsoft.CodeAnalysis.SemanticModel semanticModel, string fullyQualifiedMetadataName)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**fullyQualifiedMetadataName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### Returns

[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

