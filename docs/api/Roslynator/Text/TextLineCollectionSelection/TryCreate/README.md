# TextLineCollectionSelection\.TryCreate\(TextLineCollection, TextSpan, TextLineCollectionSelection\) Method

[Home](../../../../README.md)

**Containing Type**: [TextLineCollectionSelection](../README.md)

**Assembly**: Roslynator\.Core\.dll

\
Creates a new [TextLineCollectionSelection](../README.md) based on the specified list and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.Text.TextLineCollection lines, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.Text.TextLineCollectionSelection selectedLines)
```

### Parameters

**lines** &ensp; [TextLineCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textlinecollection)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selectedLines** &ensp; [TextLineCollectionSelection](../README.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one line; otherwise, false\.