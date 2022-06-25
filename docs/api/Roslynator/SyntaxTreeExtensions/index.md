---
sidebar_label: SyntaxTreeExtensions
---

# SyntaxTreeExtensions Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
A set of extension methods for [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static class SyntaxTreeExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetEndLine(SyntaxTree, TextSpan, CancellationToken)](GetEndLine/index.md) | Returns zero\-based index of the end line of the specified span\. |
| [GetStartLine(SyntaxTree, TextSpan, CancellationToken)](GetStartLine/index.md) | Returns zero\-based index of the start line of the specified span\. |
| [IsMultiLineSpan(SyntaxTree, TextSpan, CancellationToken)](IsMultiLineSpan/index.md) | Returns true if the specified [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan) spans over multiple lines\. |
| [IsSingleLineSpan(SyntaxTree, TextSpan, CancellationToken)](IsSingleLineSpan/index.md) | Returns true if the specified [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan) does not span over multiple lines\. |

