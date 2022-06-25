---
sidebar_label: GetStartLine
---

# SyntaxTreeExtensions\.GetStartLine\(SyntaxTree, TextSpan, CancellationToken\) Method

**Containing Type**: [SyntaxTreeExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns zero\-based index of the start line of the specified span\.

```csharp
public static int GetStartLine(this Microsoft.CodeAnalysis.SyntaxTree syntaxTree, Microsoft.CodeAnalysis.Text.TextSpan span, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**syntaxTree** &ensp; [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

