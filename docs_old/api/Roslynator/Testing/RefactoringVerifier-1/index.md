---
sidebar_label: RefactoringVerifier<TRefactoringProvider>
---

# RefactoringVerifier&lt;TRefactoringProvider&gt; Class

**Namespace**: [Roslynator.Testing](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Represents verifier for a code refactoring\.

```csharp
public abstract class RefactoringVerifier<TRefactoringProvider> : Roslynator.Testing.CodeVerifier where TRefactoringProvider : Microsoft.CodeAnalysis.CodeRefactorings.CodeRefactoringProvider, new()
```

### Type Parameters

**TRefactoringProvider**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [CodeVerifier](../CodeVerifier/index.md) &#x2192; RefactoringVerifier&lt;TRefactoringProvider&gt;

### Derived

* [CSharpRefactoringVerifier&lt;TRefactoringProvider&gt;](../CSharp/CSharpRefactoringVerifier-1/index.md)

## Properties

| Property | Summary |
| -------- | ------- |
| [CommonOptions](../CodeVerifier/CommonOptions/index.md) | Gets a common code verification options\. \(Inherited from [CodeVerifier](../CodeVerifier/index.md)\) |
| [Options](../CodeVerifier/Options/index.md) | Gets a code verification options\. \(Inherited from [CodeVerifier](../CodeVerifier/index.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [VerifyNoRefactoringAsync(RefactoringTestData, TestOptions, CancellationToken)](VerifyNoRefactoringAsync/index.md) | Verifies that refactoring will not be applied using specified **TRefactoringProvider**\. |
| [VerifyRefactoringAsync(RefactoringTestData, ExpectedTestState, TestOptions, CancellationToken)](VerifyRefactoringAsync/index.md) | Verifies that refactoring will be applied correctly using specified **TRefactoringProvider**\. |

