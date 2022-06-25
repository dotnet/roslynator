---
sidebar_label: EnsureUniqueEnumMemberName
---

# NameGenerator\.EnsureUniqueEnumMemberName\(String, INamedTypeSymbol, Boolean\) Method

**Containing Type**: [NameGenerator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
Returns unique enum member name for a specified enum type\.

```csharp
public string EnsureUniqueEnumMemberName(string baseName, Microsoft.CodeAnalysis.INamedTypeSymbol enumType, bool isCaseSensitive = true)
```

### Parameters

**baseName** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**enumType** &ensp; [INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

**isCaseSensitive** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

