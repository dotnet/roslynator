---
sidebar_label: VerifyNoRefactoringAsync
---

# RefactoringVerifier&lt;TRefactoringProvider&gt;\.VerifyNoRefactoringAsync\(RefactoringTestData, TestOptions, CancellationToken\) Method

**Containing Type**: [RefactoringVerifier&lt;TRefactoringProvider&gt;](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Verifies that refactoring will not be applied using specified **TRefactoringProvider**\.

```csharp
public System.Threading.Tasks.Task VerifyNoRefactoringAsync(Roslynator.Testing.RefactoringTestData data, Roslynator.Testing.TestOptions options = null, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**data** &ensp; [RefactoringTestData](../../RefactoringTestData/index.md)

**options** &ensp; [TestOptions](../../TestOptions/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)

