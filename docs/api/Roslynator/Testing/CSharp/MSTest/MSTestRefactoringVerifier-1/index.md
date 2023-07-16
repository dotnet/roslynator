---
sidebar_label: MSTestRefactoringVerifier<TRefactoringProvider>
---

# MSTestRefactoringVerifier&lt;TRefactoringProvider&gt; Class

**Namespace**: [Roslynator.Testing.CSharp.MSTest](../index.md)

**Assembly**: Roslynator\.Testing\.CSharp\.MSTest\.dll

  
Represents verifier for a C\# code refactoring\.

```csharp
public abstract class MSTestRefactoringVerifier<TRefactoringProvider> : Roslynator.Testing.CSharp.CSharpRefactoringVerifier<TRefactoringProvider> where TRefactoringProvider : Microsoft.CodeAnalysis.CodeRefactorings.CodeRefactoringProvider, new()
```

### Type Parameters

**TRefactoringProvider**

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [CodeVerifier](../../../CodeVerifier/index.md) &#x2192; [RefactoringVerifier&lt;TRefactoringProvider&gt;](../../../RefactoringVerifier-1/index.md) &#x2192; [CSharpRefactoringVerifier&lt;TRefactoringProvider&gt;](../../CSharpRefactoringVerifier-1/index.md) &#x2192; MSTestRefactoringVerifier&lt;TRefactoringProvider&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [MSTestRefactoringVerifier()](-ctor/index.md) | Initializes a new instance of [MSTestRefactoringVerifier&lt;TRefactoringProvider&gt;](./index.md)\. |

## Properties

| Property | Summary |
| -------- | ------- |
| [CommonOptions](../../CSharpRefactoringVerifier-1/CommonOptions/index.md) | Gets common test options\. \(Inherited from [CSharpRefactoringVerifier&lt;TRefactoringProvider&gt;](../../CSharpRefactoringVerifier-1/index.md)\) |
| [Options](../../CSharpRefactoringVerifier-1/Options/index.md) | Gets a test options\. \(Inherited from [CSharpRefactoringVerifier&lt;TRefactoringProvider&gt;](../../CSharpRefactoringVerifier-1/index.md)\) |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [VerifyNoRefactoringAsync(RefactoringTestData, TestOptions, CancellationToken)](../../../RefactoringVerifier-1/VerifyNoRefactoringAsync/index.md) | Verifies that refactoring will not be applied using specified **TRefactoringProvider**\. \(Inherited from [RefactoringVerifier&lt;TRefactoringProvider&gt;](../../../RefactoringVerifier-1/index.md)\) |
| [VerifyRefactoringAsync(RefactoringTestData, ExpectedTestState, TestOptions, CancellationToken)](../../../RefactoringVerifier-1/VerifyRefactoringAsync/index.md) | Verifies that refactoring will be applied correctly using specified **TRefactoringProvider**\. \(Inherited from [RefactoringVerifier&lt;TRefactoringProvider&gt;](../../../RefactoringVerifier-1/index.md)\) |

