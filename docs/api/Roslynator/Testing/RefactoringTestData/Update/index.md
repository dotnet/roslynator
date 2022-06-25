---
sidebar_label: Update
---

# RefactoringTestData\.Update\(String, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;AdditionalFile&gt;, String\) Method

**Containing Type**: [RefactoringTestData](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Creates and return new instance of [RefactoringTestData](../index.md) updated with specified values\.

```csharp
public Roslynator.Testing.RefactoringTestData Update(string source, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Text.TextSpan> spans, System.Collections.Generic.IEnumerable<Roslynator.Testing.AdditionalFile> additionalFiles, string equivalenceKey)
```

### Parameters

**source** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**spans** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)&gt;

**additionalFiles** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[AdditionalFile](../../AdditionalFile/index.md)&gt;

**equivalenceKey** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### Returns

[RefactoringTestData](../index.md)

