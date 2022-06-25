---
sidebar_label: CSharpTestOptions
---

# CSharpTestOptions\(CSharpCompilationOptions, CSharpParseOptions, IEnumerable&lt;MetadataReference&gt;, IEnumerable&lt;String&gt;, DiagnosticSeverity, IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt;\) Constructor

**Containing Type**: [CSharpTestOptions](../index.md)

**Assembly**: Roslynator\.Testing\.CSharp\.dll

  
Initializes a new instance of [CSharpTestOptions](../index.md)\.

```csharp
public CSharpTestOptions(Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions compilationOptions = null, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions parseOptions = null, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.MetadataReference> metadataReferences = null, System.Collections.Generic.IEnumerable<string> allowedCompilerDiagnosticIds = null, Microsoft.CodeAnalysis.DiagnosticSeverity allowedCompilerDiagnosticSeverity = Info, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> configOptions = null)
```

### Parameters

**compilationOptions** &ensp; [CSharpCompilationOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpcompilationoptions)

**parseOptions** &ensp; [CSharpParseOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpparseoptions)

**metadataReferences** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MetadataReference](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.metadatareference)&gt;

**allowedCompilerDiagnosticIds** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)&gt;

**allowedCompilerDiagnosticSeverity** &ensp; [DiagnosticSeverity](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticseverity)

**configOptions** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[KeyValuePair](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2)&lt;[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)&gt;&gt;