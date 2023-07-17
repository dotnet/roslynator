---
sidebar_label: TryCreate
---

# StatementListSelection\.TryCreate Method

**Containing Type**: [StatementListSelection](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [TryCreate(BlockSyntax, TextSpan, StatementListSelection)](#736714011) | Creates a new [StatementListSelection](../index.md) based on the specified block and span\. |
| [TryCreate(SwitchSectionSyntax, TextSpan, StatementListSelection)](#1958870021) | Creates a new [StatementListSelection](../index.md) based on the specified switch section and span\. |

<a id="736714011"></a>

## TryCreate\(BlockSyntax, TextSpan, StatementListSelection\) 

  
Creates a new [StatementListSelection](../index.md) based on the specified block and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.CSharp.StatementListSelection selectedStatements)
```

### Parameters

**block** &ensp; [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selectedStatements** &ensp; [StatementListSelection](../index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one statement; otherwise, false\.<a id="1958870021"></a>

## TryCreate\(SwitchSectionSyntax, TextSpan, StatementListSelection\) 

  
Creates a new [StatementListSelection](../index.md) based on the specified switch section and span\.

```csharp
public static bool TryCreate(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax switchSection, Microsoft.CodeAnalysis.Text.TextSpan span, out Roslynator.CSharp.StatementListSelection selectedStatements)
```

### Parameters

**switchSection** &ensp; [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

**selectedStatements** &ensp; [StatementListSelection](../index.md)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one statement; otherwise, false\.