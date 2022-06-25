---
sidebar_label: EnsureUniqueParameterName
---

# NameGenerator\.EnsureUniqueParameterName\(String, ISymbol, SemanticModel, Boolean, CancellationToken\) Method

**Containing Type**: [NameGenerator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Return a parameter name that will be unique at the specified position\.

```csharp
public string EnsureUniqueParameterName(string baseName, Microsoft.CodeAnalysis.ISymbol containingSymbol, Microsoft.CodeAnalysis.SemanticModel semanticModel, bool isCaseSensitive = true, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**baseName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**containingSymbol** &ensp; [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

