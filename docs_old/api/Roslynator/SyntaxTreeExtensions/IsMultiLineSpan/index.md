---
sidebar_label: IsMultiLineSpan
---

# SyntaxTreeExtensions\.IsMultiLineSpan\(SyntaxTree, TextSpan, CancellationToken\) Method

**Containing Type**: [SyntaxTreeExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns true if the specified [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan) spans over multiple lines\.

```csharp
public static bool IsMultiLineSpan(this Microsoft.CodeAnalysis.SyntaxTree syntaxTree, Microsoft.CodeAnalysis.Text.TextSpan span, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**syntaxTree** &ensp; [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

