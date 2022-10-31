# SyntaxInfo\.SimpleIfStatementInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleIfStatementInfo(IfStatementSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleIfStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax_System_Boolean_System_Boolean_) | Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/README.md) from the specified if statement\. |
| [SimpleIfStatementInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleIfStatementInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/README.md) from the specified node\. |

## SimpleIfStatementInfo\(IfStatementSyntax, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleIfStatementInfo_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/README.md) from the specified if statement\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleIfStatementInfo SimpleIfStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax ifStatement, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**ifStatement** &ensp; [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/README.md)

## SimpleIfStatementInfo\(SyntaxNode, Boolean, Boolean\) <a id="Roslynator_CSharp_SyntaxInfo_SimpleIfStatementInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

\
Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleIfStatementInfo SimpleIfStatementInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/README.md)

