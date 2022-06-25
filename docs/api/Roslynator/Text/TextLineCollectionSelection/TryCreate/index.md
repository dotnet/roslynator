---
sidebar_label: TryCreate
---

# TextLineCollectionSelection\.TryCreate\(TextLineCollection, TextSpan, TextLineCollectionSelection\) Method

**Containing Type**: [TextLineCollectionSelection](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Creates a new [TextLineCollectionSelection](../index.md) based on the specified list and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.Text.TextLineCollection lines, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.Text.TextLineCollectionSelection selectedLines)
```

### Parameters

**lines** &ensp; [TextLineCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textlinecollection)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selectedLines** &ensp; [TextLineCollectionSelection](../index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one line; otherwise, false\.