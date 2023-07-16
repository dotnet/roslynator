---
sidebar_label: Create
---

# StatementListSelection\.Create Method

**Containing Type**: [StatementListSelection](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Create(BlockSyntax, TextSpan)](#933669229) | Creates a new [StatementListSelection](../index.md) based on the specified block and span\. |
| [Create(StatementListInfo, TextSpan)](#4239290103) | Creates a new [StatementListSelection](../index.md) based on the specified [StatementListInfo](../../Syntax/StatementListInfo/index.md) and span\. |
| [Create(SwitchSectionSyntax, TextSpan)](#1797202091) | Creates a new [StatementListSelection](../index.md) based on the specified switch section and span\. |

<a id="933669229"></a>

## Create\(BlockSyntax, TextSpan\) 

  
Creates a new [StatementListSelection](../index.md) based on the specified block and span\.

```csharp
public static Roslynator.CSharp.StatementListSelection Create(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**block** &ensp; [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[StatementListSelection](../index.md)

<a id="4239290103"></a>

## Create\(StatementListInfo, TextSpan\) 

  
Creates a new [StatementListSelection](../index.md) based on the specified [StatementListInfo](../../Syntax/StatementListInfo/index.md) and span\.

```csharp
public static Roslynator.CSharp.StatementListSelection Create(in Roslynator.CSharp.Syntax.StatementListInfo statementsInfo, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**statementsInfo** &ensp; [StatementListInfo](../../Syntax/StatementListInfo/index.md)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[StatementListSelection](../index.md)

<a id="1797202091"></a>

## Create\(SwitchSectionSyntax, TextSpan\) 

  
Creates a new [StatementListSelection](../index.md) based on the specified switch section and span\.

```csharp
public static Roslynator.CSharp.StatementListSelection Create(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax switchSection, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**switchSection** &ensp; [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[StatementListSelection](../index.md)

