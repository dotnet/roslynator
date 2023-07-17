---
sidebar_label: ExpectedTestState
---

# ExpectedTestState\(String, String, IEnumerable&lt;\(String, TextSpan\)&gt;, IEnumerable&lt;String&gt;\) Constructor

**Containing Type**: [ExpectedTestState](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Initializes a new instance of [ExpectedTestState](../index.md)\.

```csharp
public ExpectedTestState(string source, string codeActionTitle = null, System.Collections.Generic.IEnumerable<(string, Microsoft.CodeAnalysis.Text.TextSpan)> annotations = null, System.Collections.Generic.IEnumerable<string> alwaysVerifyAnnotations = null)
```

### Parameters

**source** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**codeActionTitle** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**annotations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;\([String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)\)&gt;

**alwaysVerifyAnnotations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)&gt;