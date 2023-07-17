---
sidebar_label: Block
---

# CSharpFactory\.Block Method

**Containing Type**: [CSharpFactory](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Block(StatementSyntax)](#907225641) | |
| [Block(SyntaxToken, StatementSyntax, SyntaxToken)](#1654242285) | |

<a id="907225641"></a>

## Block\(StatementSyntax\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement)
```

### Parameters

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

### Returns

[BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

<a id="1654242285"></a>

## Block\(SyntaxToken, StatementSyntax, SyntaxToken\) 

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax Block(Microsoft.CodeAnalysis.SyntaxToken openBrace, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement, Microsoft.CodeAnalysis.SyntaxToken closeBrace)
```

### Parameters

**openBrace** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**statement** &ensp; [StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

**closeBrace** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[BlockSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.blocksyntax)

