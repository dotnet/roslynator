---
sidebar_label: CodeVerifier
---

# CodeVerifier Class

**Namespace**: [Roslynator.Testing](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

  
Represents base type for verifying a diagnostic, a code fix and a refactoring\.

```csharp
public abstract class CodeVerifier
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; CodeVerifier

### Derived

* [CompilerDiagnosticFixVerifier&lt;TFixProvider&gt;](../CompilerDiagnosticFixVerifier-1/index.md)
* [DiagnosticVerifier&lt;TAnalyzer, TFixProvider&gt;](../DiagnosticVerifier-2/index.md)
* [RefactoringVerifier&lt;TRefactoringProvider&gt;](../RefactoringVerifier-1/index.md)

## Properties

| Property | Summary |
| -------- | ------- |
| [CommonOptions](CommonOptions/index.md) | Gets a common code verification options\. |
| [Options](Options/index.md) | Gets a code verification options\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.object.tostring) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |

