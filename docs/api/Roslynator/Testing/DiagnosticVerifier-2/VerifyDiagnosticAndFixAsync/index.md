---
sidebar_label: VerifyDiagnosticAndFixAsync
---

# DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;\.VerifyDiagnosticAndFixAsync\(DiagnosticTestData, ExpectedTestState, TestOptions, CancellationToken\) Method

**Containing Type**: [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Verifies that specified source will produce specified diagnostic and that the diagnostic will be fixed\.

```csharp
public System.Threading.Tasks.Task VerifyDiagnosticAndFixAsync(Roslynator.Testing.DiagnosticTestData data, Roslynator.Testing.ExpectedTestState expected, Roslynator.Testing.TestOptions options = null, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**data** &ensp; [DiagnosticTestData](../../DiagnosticTestData/index.md)

**expected** &ensp; [ExpectedTestState](../../ExpectedTestState/index.md)

**options** &ensp; [TestOptions](../../TestOptions/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)

