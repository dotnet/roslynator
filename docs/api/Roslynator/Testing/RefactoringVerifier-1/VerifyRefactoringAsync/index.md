---
sidebar_label: VerifyRefactoringAsync
---

# RefactoringVerifier&lt;TRefactoringProvider&gt;\.VerifyRefactoringAsync\(RefactoringTestData, ExpectedTestState, TestOptions, CancellationToken\) Method

**Containing Type**: [RefactoringVerifier&lt;TRefactoringProvider&gt;](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Verifies that refactoring will be applied correctly using specified **TRefactoringProvider**\.

```csharp
public System.Threading.Tasks.Task VerifyRefactoringAsync(Roslynator.Testing.RefactoringTestData data, Roslynator.Testing.ExpectedTestState expected, Roslynator.Testing.TestOptions options = null, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**data** &ensp; [RefactoringTestData](../../RefactoringTestData/index.md)

**expected** &ensp; [ExpectedTestState](../../ExpectedTestState/index.md)

**options** &ensp; [TestOptions](../../TestOptions/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)

