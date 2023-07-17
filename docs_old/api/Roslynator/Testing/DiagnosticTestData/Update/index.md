---
sidebar_label: Update
---

# DiagnosticTestData\.Update\(DiagnosticDescriptor, String, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;TextSpan&gt;, IEnumerable&lt;AdditionalFile&gt;, String, IFormatProvider, String, Boolean\) Method

**Containing Type**: [DiagnosticTestData](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Creates and return new instance of [DiagnosticTestData](../index.md) updated with specified values\.

```csharp
public Roslynator.Testing.DiagnosticTestData Update(Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, string source, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Text.TextSpan> spans, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Text.TextSpan> additionalSpans, System.Collections.Generic.IEnumerable<Roslynator.Testing.AdditionalFile> additionalFiles, string diagnosticMessage, IFormatProvider formatProvider, string equivalenceKey, bool alwaysVerifyAdditionalLocations)
```

### Parameters

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**source** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**spans** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)&gt;

**additionalSpans** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)&gt;

**additionalFiles** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[AdditionalFile](../../AdditionalFile/index.md)&gt;

**diagnosticMessage** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**formatProvider** &ensp; [IFormatProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider)

**equivalenceKey** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**alwaysVerifyAdditionalLocations** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[DiagnosticTestData](../index.md)

