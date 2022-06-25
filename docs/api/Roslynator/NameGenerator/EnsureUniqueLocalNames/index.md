---
sidebar_label: EnsureUniqueLocalNames
---

# NameGenerator\.EnsureUniqueLocalNames\(String, SemanticModel, Int32, Int32, Boolean, CancellationToken\) Method

**Containing Type**: [NameGenerator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Return a local names that will be unique at the specified position\.

```csharp
public System.Collections.Immutable.ImmutableArray<string> EnsureUniqueLocalNames(string baseName, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, int count, bool isCaseSensitive = true, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**baseName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**position** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**count** &ensp; [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[ImmutableArray](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablearray-1)&lt;[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)&gt;

