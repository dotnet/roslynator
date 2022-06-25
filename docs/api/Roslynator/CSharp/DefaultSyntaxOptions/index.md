---
sidebar_label: DefaultSyntaxOptions
---

# DefaultSyntaxOptions Enum

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
Defines how a syntax representing a default value of a type should look like\.

```csharp
[Flags]
public enum DefaultSyntaxOptions
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) &#x2192; DefaultSyntaxOptions

### Attributes

* [FlagsAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute)

## Fields

| Name | Value | Summary |
| ---- | ----- | ------- |
| None | 0 | No option specified\. |
| UseDefault | 1 | Use [SyntaxKind.DefaultExpression](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.defaultexpression) or [SyntaxKind.DefaultLiteralExpression](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.defaultliteralexpression)\. |
| AllowDefaultLiteral | 2 | Allow [SyntaxKind.DefaultLiteralExpression](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.defaultliteralexpression) instead of [SyntaxKind.DefaultExpression](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind.defaultexpression)\. |

