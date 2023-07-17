---
sidebar_label: GetHashCode
---

# MetadataNameEqualityComparer&lt;TSymbol&gt;\.GetHashCode\(TSymbol\) Method

**Containing Type**: [MetadataNameEqualityComparer&lt;TSymbol&gt;](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Serves as a hash function for the specified symbol\.

```csharp
public override int GetHashCode(TSymbol obj)
```

### Parameters

**obj** &ensp; TSymbol

The symbol for which to get a hash code\.

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

A hash code for the specified symbol\.

### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)

**obj** is `null`\.

