---
sidebar_label: CommentFilter
---

# CommentFilter Enum

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Specifies C\# comments\.

```csharp
[Flags]
public enum CommentFilter
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) &#x2192; CommentFilter

### Attributes

* [FlagsAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute)

## Fields

| Name | Value | Combination of | Summary |
| ---- | ----- | -------------- | ------- |
| None | 0 | | None comment specified\. |
| SingleLine | 1 | | Single\-line comment\. |
| MultiLine | 2 | | Multi\-line comment\. |
| NonDocumentation | 3 | SingleLine \| MultiLine | Non\-documentation comment \(single\-line or multi\-line\)\. |
| SingleLineDocumentation | 4 | | Single\-line documentation comment\. |
| MultiLineDocumentation | 8 | | Multi\-line documentation comment\. |
| Documentation | 12 | SingleLineDocumentation \| MultiLineDocumentation | Documentation comment \(single\-line or multi\-line\)\. |
| All | 15 | NonDocumentation \| Documentation | Documentation or non\-documentation comment\. |

