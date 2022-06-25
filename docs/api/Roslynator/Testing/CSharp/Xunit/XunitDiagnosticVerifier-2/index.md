---
sidebar_label: XunitDiagnosticVerifier<TAnalyzer, TFixProvider>
---

# XunitDiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt; Class

**Namespace**: [Roslynator.Testing.CSharp.Xunit](../index.md)

**Assembly**: Roslynator\.Testing\.CSharp\.Xunit\.dll

  
Represents a verifier for a C\# diagnostic that is produced by [DiagnosticAnalyzer](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.diagnosticanalyzer)\.

```csharp
public abstract class XunitDiagnosticVerifier<TAnalyzer, TFixProvider> : Roslynator.Testing.CSharp.CSharpDiagnosticVerifier<TAnalyzer, TFixProvider>
    where TAnalyzer : Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer, new() 
    where TFixProvider : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider, new()
```

### Type Parameters

**TAnalyzer**

**TFixProvider**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [CodeVerifier](../../../CodeVerifier/index.md) &#x2192; [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../../DiagnosticVerifier-2/index.md) &#x2192; [CSharpDiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../CSharpDiagnosticVerifier-2/index.md) &#x2192; XunitDiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [XunitDiagnosticVerifier()](-ctor/index.md) | Initializes a new instance of [XunitDiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](./index.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [CommonOptions](../../CSharpDiagnosticVerifier-2/CommonOptions/index.md) | Gets common test options\. \(Inherited from [CSharpDiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../CSharpDiagnosticVerifier-2/index.md)\) |
| [Options](../../CSharpDiagnosticVerifier-2/Options/index.md) | Gets a test options\. \(Inherited from [CSharpDiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../CSharpDiagnosticVerifier-2/index.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [VerifyDiagnosticAndFixAsync(DiagnosticTestData, ExpectedTestState, TestOptions, CancellationToken)](../../../DiagnosticVerifier-2/VerifyDiagnosticAndFixAsync/index.md) | Verifies that specified source will produce specified diagnostic and that the diagnostic will be fixed\. \(Inherited from [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../../DiagnosticVerifier-2/index.md)\) |
| [VerifyDiagnosticAndNoFixAsync(DiagnosticTestData, TestOptions, CancellationToken)](../../../DiagnosticVerifier-2/VerifyDiagnosticAndNoFixAsync/index.md) | Verifies that specified source will produce specified diagnostic and that the diagnostic will not be fixed\. \(Inherited from [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../../DiagnosticVerifier-2/index.md)\) |
| [VerifyDiagnosticAsync(DiagnosticTestData, TestOptions, CancellationToken)](../../../DiagnosticVerifier-2/VerifyDiagnosticAsync/index.md) | Verifies that specified source will produce specified diagnostic\(s\)\. \(Inherited from [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../../DiagnosticVerifier-2/index.md)\) |
| [VerifyNoDiagnosticAsync(DiagnosticTestData, TestOptions, CancellationToken)](../../../DiagnosticVerifier-2/VerifyNoDiagnosticAsync/index.md) | Verifies that specified source will not produce specified diagnostic\. \(Inherited from [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../../../DiagnosticVerifier-2/index.md)\) |

