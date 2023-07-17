---
sidebar_label: CSharpTestOptions
---

# CSharpTestOptions Class

**Namespace**: [Roslynator.Testing.CSharp](../index.md)

**Assembly**: Roslynator\.Testing\.CSharp\.dll

  
Represents options for a C\# code verifier\.

```csharp
public sealed class CSharpTestOptions : Roslynator.Testing.TestOptions
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [TestOptions](../../TestOptions/index.md) &#x2192; CSharpTestOptions

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [CSharpTestOptions(CSharpCompilationOptions, CSharpParseOptions, IEnumerable&lt;MetadataReference&gt;, IEnumerable&lt;String&gt;, DiagnosticSeverity, IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt;)](-ctor/index.md) | Initializes a new instance of [CSharpTestOptions](./index.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [AllowedCompilerDiagnosticIds](../../TestOptions/AllowedCompilerDiagnosticIds/index.md) | Gets a collection of compiler diagnostic IDs\. \(Inherited from [TestOptions](../../TestOptions/index.md)\) |
| [AllowedCompilerDiagnosticSeverity](../../TestOptions/AllowedCompilerDiagnosticSeverity/index.md) | Gets a maximal allowed compiler diagnostic severity\. \(Inherited from [TestOptions](../../TestOptions/index.md)\) |
| [CommonCompilationOptions](CommonCompilationOptions/index.md) | Gets a common compilation options\. \(Overrides [TestOptions.CommonCompilationOptions](../../TestOptions/CommonCompilationOptions/index.md)\) |
| [CommonParseOptions](CommonParseOptions/index.md) | Gets a common parse options\. \(Overrides [TestOptions.CommonParseOptions](../../TestOptions/CommonParseOptions/index.md)\) |
| [CompilationOptions](CompilationOptions/index.md) | Gets a compilation options that should be used to compile test project\. |
| [ConfigOptions](../../TestOptions/ConfigOptions/index.md) | Gets a collection of config options\. \(Inherited from [TestOptions](../../TestOptions/index.md)\) |
| [Default](Default/index.md) | Gets a default code verification options\. |
| [Language](Language/index.md) | Gets C\# programming language identifier\. \(Overrides [TestOptions.Language](../../TestOptions/Language/index.md)\) |
| [MetadataReferences](../../TestOptions/MetadataReferences/index.md) | Gets metadata references of a test project\. \(Inherited from [TestOptions](../../TestOptions/index.md)\) |
| [ParseOptions](ParseOptions/index.md) | Gets a parse options that should be used to parse tested source code\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [AddAllowedCompilerDiagnosticId(String)](AddAllowedCompilerDiagnosticId/index.md) | Adds specified compiler diagnostic ID to the list of allowed compiler diagnostic IDs\. |
| [AddAllowedCompilerDiagnosticIds(IEnumerable&lt;String&gt;)](AddAllowedCompilerDiagnosticIds/index.md) | Adds a list of specified compiler diagnostic IDs to the list of allowed compiler diagnostic IDs\. |
| [CommonWithAllowedCompilerDiagnosticIds(IEnumerable&lt;String&gt;)](CommonWithAllowedCompilerDiagnosticIds/index.md) |  \(Overrides [TestOptions.CommonWithAllowedCompilerDiagnosticIds](../../TestOptions/CommonWithAllowedCompilerDiagnosticIds/index.md)\) |
| [CommonWithAllowedCompilerDiagnosticSeverity(DiagnosticSeverity)](CommonWithAllowedCompilerDiagnosticSeverity/index.md) |  \(Overrides [TestOptions.CommonWithAllowedCompilerDiagnosticSeverity](../../TestOptions/CommonWithAllowedCompilerDiagnosticSeverity/index.md)\) |
| [CommonWithConfigOptions(IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt;)](CommonWithConfigOptions/index.md) |  \(Overrides [TestOptions.CommonWithConfigOptions](../../TestOptions/CommonWithConfigOptions/index.md)\) |
| [CommonWithMetadataReferences(IEnumerable&lt;MetadataReference&gt;)](CommonWithMetadataReferences/index.md) |  \(Overrides [TestOptions.CommonWithMetadataReferences](../../TestOptions/CommonWithMetadataReferences/index.md)\) |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [WithAllowedCompilerDiagnosticIds(IEnumerable&lt;String&gt;)](WithAllowedCompilerDiagnosticIds/index.md) | |
| [WithAllowedCompilerDiagnosticSeverity(DiagnosticSeverity)](WithAllowedCompilerDiagnosticSeverity/index.md) | |
| [WithCompilationOptions(CSharpCompilationOptions)](WithCompilationOptions/index.md) | |
| [WithConfigOptions(IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt;)](WithConfigOptions/index.md) | |
| [WithMetadataReferences(IEnumerable&lt;MetadataReference&gt;)](WithMetadataReferences/index.md) | |
| [WithParseOptions(CSharpParseOptions)](WithParseOptions/index.md) | |

