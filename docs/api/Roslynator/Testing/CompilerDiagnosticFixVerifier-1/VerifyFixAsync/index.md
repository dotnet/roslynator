---
sidebar_label: VerifyFixAsync
---

# CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;\.VerifyFixAsync\(CompilerDiagnosticFixTestData, ExpectedTestState, TestOptions, CancellationToken\) Method

**Containing Type**: [CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Verifies that specified source will produce compiler diagnostic\.

```csharp
public System.Threading.Tasks.Task VerifyFixAsync(Roslynator.Testing.CompilerDiagnosticFixTestData data, Roslynator.Testing.ExpectedTestState expected, Roslynator.Testing.TestOptions options = null, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**data** &ensp; [CompilerDiagnosticFixTestData](../../CompilerDiagnosticFixTestData/index.md)

**expected** &ensp; [ExpectedTestState](../../ExpectedTestState/index.md)

**options** &ensp; [TestOptions](../../TestOptions/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)
