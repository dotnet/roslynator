---
sidebar_label: VerifyDiagnosticAndNoFixAsync
---

# DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;\.VerifyDiagnosticAndNoFixAsync\(DiagnosticTestData, TestOptions, CancellationToken\) Method

**Containing Type**: [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Verifies that specified source will produce specified diagnostic and that the diagnostic will not be fixed\.

```csharp
public System.Threading.Tasks.Task VerifyDiagnosticAndNoFixAsync(Roslynator.Testing.DiagnosticTestData data, Roslynator.Testing.TestOptions options = null, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**data** &ensp; [DiagnosticTestData](../../DiagnosticTestData/index.md)

**options** &ensp; [TestOptions](../../TestOptions/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)

