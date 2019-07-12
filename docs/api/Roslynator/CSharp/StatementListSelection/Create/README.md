# StatementListSelection\.Create Method

[Home](../../../../README.md)

**Containing Type**: [StatementListSelection](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Create(BlockSyntax, TextSpan)](#Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_BlockSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](../README.md) based on the specified block and span\. |
| [Create(StatementListInfo, TextSpan)](#Roslynator_CSharp_StatementListSelection_Create_Roslynator_CSharp_Syntax_StatementListInfo__Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](../README.md) based on the specified [StatementListInfo](../../Syntax/StatementListInfo/README.md) and span\. |
| [Create(SwitchSectionSyntax, TextSpan)](#Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_SwitchSectionSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](../README.md) based on the specified switch section and span\. |

## Create\(BlockSyntax, TextSpan\) <a id="Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_BlockSyntax_Microsoft_CodeAnalysis_Text_TextSpan_"></a>

\
Creates a new [StatementListSelection](../README.md) based on the specified block and span\.

```csharp
public static Roslynator.CSharp.StatementListSelection Create(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax block, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**block** &ensp; [BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[StatementListSelection](../README.md)

## Create\(StatementListInfo, TextSpan\) <a id="Roslynator_CSharp_StatementListSelection_Create_Roslynator_CSharp_Syntax_StatementListInfo__Microsoft_CodeAnalysis_Text_TextSpan_"></a>

\
Creates a new [StatementListSelection](../README.md) based on the specified [StatementListInfo](../../Syntax/StatementListInfo/README.md) and span\.

```csharp
public static Roslynator.CSharp.StatementListSelection Create(in Roslynator.CSharp.Syntax.StatementListInfo statementsInfo, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**statementsInfo** &ensp; [StatementListInfo](../../Syntax/StatementListInfo/README.md)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[StatementListSelection](../README.md)

## Create\(SwitchSectionSyntax, TextSpan\) <a id="Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_SwitchSectionSyntax_Microsoft_CodeAnalysis_Text_TextSpan_"></a>

\
Creates a new [StatementListSelection](../README.md) based on the specified switch section and span\.

```csharp
public static Roslynator.CSharp.StatementListSelection Create(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax switchSection, Microsoft.CodeAnalysis.Text.TextSpan span)
```

### Parameters

**switchSection** &ensp; [SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)

**span** &ensp; [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

### Returns

[StatementListSelection](../README.md)

