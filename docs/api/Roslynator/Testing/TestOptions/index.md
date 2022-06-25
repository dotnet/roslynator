---
sidebar_label: TestOptions
---

# TestOptions Class

**Namespace**: [Roslynator.Testing](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Represents options for a code verifier\.

```csharp
public abstract class TestOptions
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; TestOptions

### Derived

* [CSharpTestOptions](../CSharp/CSharpTestOptions/index.md)

## Properties

| Property | Summary |
| -------- | ------- |
| [AllowedCompilerDiagnosticIds](AllowedCompilerDiagnosticIds/index.md) | Gets a collection of compiler diagnostic IDs\. |
| [AllowedCompilerDiagnosticSeverity](AllowedCompilerDiagnosticSeverity/index.md) | Gets a maximal allowed compiler diagnostic severity\. |
| [CommonCompilationOptions](CommonCompilationOptions/index.md) | Gets a common compilation options\. |
| [CommonParseOptions](CommonParseOptions/index.md) | Gets a common parse options\. |
| [CompilationOptions](CompilationOptions/index.md) | Gets a compilation options that should be used to compile test project\. |
| [ConfigOptions](ConfigOptions/index.md) | Gets a collection of config options\. |
| [Language](Language/index.md) | Gets a programming language identifier\. |
| [MetadataReferences](MetadataReferences/index.md) | Gets metadata references of a test project\. |
| [ParseOptions](ParseOptions/index.md) | Gets a parse options that should be used to parse tested source code\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [CommonWithAllowedCompilerDiagnosticIds(IEnumerable&lt;String&gt;)](CommonWithAllowedCompilerDiagnosticIds/index.md) | |
| [CommonWithAllowedCompilerDiagnosticSeverity(DiagnosticSeverity)](CommonWithAllowedCompilerDiagnosticSeverity/index.md) | |
| [CommonWithConfigOptions(IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt;)](CommonWithConfigOptions/index.md) | |
| [CommonWithMetadataReferences(IEnumerable&lt;MetadataReference&gt;)](CommonWithMetadataReferences/index.md) | |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [WithAllowedCompilerDiagnosticIds(IEnumerable&lt;String&gt;)](WithAllowedCompilerDiagnosticIds/index.md) | |
| [WithAllowedCompilerDiagnosticSeverity(DiagnosticSeverity)](WithAllowedCompilerDiagnosticSeverity/index.md) | |
| [WithConfigOptions(IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt;)](WithConfigOptions/index.md) | |
| [WithMetadataReferences(IEnumerable&lt;MetadataReference&gt;)](WithMetadataReferences/index.md) | |

