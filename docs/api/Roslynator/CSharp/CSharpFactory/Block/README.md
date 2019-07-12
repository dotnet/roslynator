# CSharpFactory\.Block Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Block(StatementSyntax)](#Roslynator_CSharp_CSharpFactory_Block_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_) | |
| [Block(SyntaxToken, StatementSyntax, SyntaxToken)](#Roslynator_CSharp_CSharpFactory_Block_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_Microsoft_CodeAnalysis_SyntaxToken_) | |

## Block\(StatementSyntax\) <a id="Roslynator_CSharp_CSharpFactory_Block_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

### Returns

[BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

## Block\(SyntaxToken, StatementSyntax, SyntaxToken\) <a id="Roslynator_CSharp_CSharpFactory_Block_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_Microsoft_CodeAnalysis_SyntaxToken_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(Microsoft.CodeAnalysis.SyntaxToken openBrace, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.SyntaxToken closeBrace)
```

### Parameters

**openBrace** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**closeBrace** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

