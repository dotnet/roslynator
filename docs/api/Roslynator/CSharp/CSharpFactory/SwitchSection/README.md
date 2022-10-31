# CSharpFactory\.SwitchSection Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SwitchSection(SwitchLabelSyntax, StatementSyntax)](#420993063) | |
| [SwitchSection(SwitchLabelSyntax, SyntaxList\<StatementSyntax\>)](#3075568334) | |
| [SwitchSection(SyntaxList\<SwitchLabelSyntax\>, StatementSyntax)](#2177824214) | |

<a id="420993063"></a>

## SwitchSection\(SwitchLabelSyntax, StatementSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax SwitchSection(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax switchLabel, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
```

### Parameters

**switchLabel** &ensp; [SwitchLabelSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchlabelsyntax)

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

### Returns

[SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)

<a id="3075568334"></a>

## SwitchSection\(SwitchLabelSyntax, SyntaxList\<StatementSyntax\>\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax SwitchSection(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax switchLabel, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax> statements)
```

### Parameters

**switchLabel** &ensp; [SwitchLabelSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchlabelsyntax)

**statements** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)\>

### Returns

[SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)

<a id="2177824214"></a>

## SwitchSection\(SyntaxList\<SwitchLabelSyntax\>, StatementSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax SwitchSection(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax> switchLabels, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
```

### Parameters

**switchLabels** &ensp; [SyntaxList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\<[SwitchLabelSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchlabelsyntax)\>

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

### Returns

[SwitchSectionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.switchsectionsyntax)

