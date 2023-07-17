---
sidebar_label: PreprocessorDirectiveFilter
---

# PreprocessorDirectiveFilter Enum

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Specifies C\# preprocessor directives\.

```csharp
[Flags]
public enum PreprocessorDirectiveFilter
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) &#x2192; PreprocessorDirectiveFilter

### Attributes

* [FlagsAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute)

## Fields

| Name | Value | Combination of | Summary |
| ---- | ----- | -------------- | ------- |
| None | 0 | | No preprocessor directive\. |
| If | 1 | | \#if preprocessor directive\. |
| Elif | 2 | | \#elif preprocessor directive\. |
| Else | 4 | | \#else preprocessor directive\. |
| EndIf | 8 | | \#endif preprocessor directive\. |
| Region | 16 | | \#region preprocessor directive\. |
| EndRegion | 32 | | \#endregion preprocessor directive\. |
| Define | 64 | | \#define preprocessor directive\. |
| Undef | 128 | | \#undef preprocessor directive\. |
| Error | 256 | | \#error preprocessor directive\. |
| Warning | 512 | | \#warning preprocessor directive\. |
| Line | 1024 | | \#line preprocessor directive\. |
| PragmaWarning | 2048 | | \#pragma warning preprocessor directive\. |
| PragmaChecksum | 4096 | | \#pragma checksum preprocessor directive\. |
| Pragma | 6144 | PragmaWarning \| PragmaChecksum | \#pragma preprocessor directive\. |
| Reference | 8192 | | \#r preprocessor directive\. |
| Load | 16384 | | \#load preprocessor directive\. |
| Bad | 32768 | | Bad preprocessor directive\. |
| Shebang | 65536 | | Shebang preprocessor directive\. |
| Nullable | 131072 | | Nullable preprocessor directive\. |
| All | 262143 | If \| Elif \| Else \| EndIf \| Region \| EndRegion \| Define \| Undef \| Error \| Warning \| Line \| Pragma \| Reference \| Load \| Bad \| Shebang \| Nullable | All preprocessor directives\. |

