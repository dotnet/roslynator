---
sidebar_label: EnsureUniqueLocalName
---

# NameGenerator\.EnsureUniqueLocalName\(String, SemanticModel, Int32, Boolean, CancellationToken\) Method

**Containing Type**: [NameGenerator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Return a local name that will be unique at the specified position\.

```csharp
public string EnsureUniqueLocalName(string baseName, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, bool isCaseSensitive = true, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**baseName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

