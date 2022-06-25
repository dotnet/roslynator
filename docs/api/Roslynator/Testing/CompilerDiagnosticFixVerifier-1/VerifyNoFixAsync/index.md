---
sidebar_label: VerifyNoFixAsync
---

# CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;\.VerifyNoFixAsync\(CompilerDiagnosticFixTestData, TestOptions, CancellationToken\) Method

**Containing Type**: [CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Verifies that specified source will not produce compiler diagnostic\.

```csharp
public System.Threading.Tasks.Task VerifyNoFixAsync(Roslynator.Testing.CompilerDiagnosticFixTestData data, Roslynator.Testing.TestOptions options = null, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**data** &ensp; [CompilerDiagnosticFixTestData](../../CompilerDiagnosticFixTestData/index.md)

**options** &ensp; [TestOptions](../../TestOptions/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)

