---
sidebar_label: CSharpRefactoringVerifier<TRefactoringProvider>
---

# CSharpRefactoringVerifier&lt;TRefactoringProvider&gt; Class

**Namespace**: [Roslynator.Testing.CSharp](../index.md)

**Assembly**: Roslynator\.Testing\.CSharp\.dll

  
Represents verifier for a C\# code refactoring\.

```csharp
public abstract class CSharpRefactoringVerifier<TRefactoringProvider> : Roslynator.Testing.RefactoringVerifier<TRefactoringProvider> where TRefactoringProvider : Microsoft.CodeAnalysis.CodeRefactorings.CodeRefactoringProvider, new()
```

### Type Parameters

**TRefactoringProvider**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [CodeVerifier](../../CodeVerifier/index.md) &#x2192; [RefactoringVerifier&lt;TRefactoringProvider&gt;](../../RefactoringVerifier-1/index.md) &#x2192; CSharpRefactoringVerifier&lt;TRefactoringProvider&gt;

### Derived

* [MSTestRefactoringVerifier&lt;TRefactoringProvider&gt;](../MSTest/MSTestRefactoringVerifier-1/index.md)
* [XunitRefactoringVerifier&lt;TRefactoringProvider&gt;](../Xunit/XunitRefactoringVerifier-1/index.md)

## Properties

| Property | Summary |
| -------- | ------- |
| [CommonOptions](CommonOptions/index.md) | Gets common test options\. \(Overrides [CodeVerifier.CommonOptions](../../CodeVerifier/CommonOptions/index.md)\) |
| [Options](Options/index.md) | Gets a test options\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [VerifyNoRefactoringAsync(RefactoringTestData, TestOptions, CancellationToken)](../../RefactoringVerifier-1/VerifyNoRefactoringAsync/index.md) | Verifies that refactoring will not be applied using specified **TRefactoringProvider**\. \(Inherited from [RefactoringVerifier&lt;TRefactoringProvider&gt;](../../RefactoringVerifier-1/index.md)\) |
| [VerifyRefactoringAsync(RefactoringTestData, ExpectedTestState, TestOptions, CancellationToken)](../../RefactoringVerifier-1/VerifyRefactoringAsync/index.md) | Verifies that refactoring will be applied correctly using specified **TRefactoringProvider**\. \(Inherited from [RefactoringVerifier&lt;TRefactoringProvider&gt;](../../RefactoringVerifier-1/index.md)\) |

