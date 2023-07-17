---
sidebar_label: MSTestCompilerDiagnosticFixVerifier<TFixProvider>
---

# MSTestCompilerDiagnosticFixVerifier&lt;TFixProvider&gt; Class

**Namespace**: [Roslynator.Testing.CSharp.MSTest](../index.md)

**Assembly**: Roslynator\.Testing\.CSharp\.MSTest\.dll

  
Represents a verifier for C\# compiler diagnostics\.

```csharp
public abstract class MSTestCompilerDiagnosticFixVerifier<TFixProvider> : Roslynator.Testing.CSharp.CSharpCompilerDiagnosticFixVerifier<TFixProvider> where TFixProvider : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider, new()
```

### Type Parameters

**TFixProvider**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [CodeVerifier](../../../CodeVerifier/index.md) &#x2192; [CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../../../CompilerDiagnosticFixVerifier-1/index.md) &#x2192; [CSharpCompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../../CSharpCompilerDiagnosticFixVerifier-1/index.md) &#x2192; MSTestCompilerDiagnosticFixVerifier&lt;TFixProvider&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [MSTestCompilerDiagnosticFixVerifier()](-ctor/index.md) | Initializes a new instance of [MSTestCompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](./index.md) |

## Properties

| Property | Summary |
| -------- | ------- |
| [CommonOptions](../../CSharpCompilerDiagnosticFixVerifier-1/CommonOptions/index.md) | Gets common test options\. \(Inherited from [CSharpCompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../../CSharpCompilerDiagnosticFixVerifier-1/index.md)\) |
| [Options](../../CSharpCompilerDiagnosticFixVerifier-1/Options/index.md) | Gets a test options\. \(Inherited from [CSharpCompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../../CSharpCompilerDiagnosticFixVerifier-1/index.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [VerifyFixAsync(CompilerDiagnosticFixTestData, ExpectedTestState, TestOptions, CancellationToken)](../../../CompilerDiagnosticFixVerifier-1/VerifyFixAsync/index.md) | Verifies that specified source will produce compiler diagnostic\. \(Inherited from [CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../../../CompilerDiagnosticFixVerifier-1/index.md)\) |
| [VerifyNoFixAsync(CompilerDiagnosticFixTestData, TestOptions, CancellationToken)](../../../CompilerDiagnosticFixVerifier-1/VerifyNoFixAsync/index.md) | Verifies that specified source will not produce compiler diagnostic\. \(Inherited from [CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../../../CompilerDiagnosticFixVerifier-1/index.md)\) |

