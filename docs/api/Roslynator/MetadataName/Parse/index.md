---
sidebar_label: Parse
---

# MetadataName\.Parse\(String\) Method

**Containing Type**: [MetadataName](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Converts the string representation of a fully qualified metadata name to its [MetadataName](../index.md) equivalent\.

```csharp
public static Roslynator.MetadataName Parse(string name)
```

### Parameters

**name** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### Returns

[MetadataName](../index.md)

### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)

**name** is empty or invalid\.

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)

**name** is `null`\.

