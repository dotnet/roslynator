---
sidebar_label: SimpleIfStatementInfo
---

# SyntaxInfo\.SimpleIfStatementInfo Method

**Containing Type**: [SyntaxInfo](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleIfStatementInfo(IfStatementSyntax, Boolean, Boolean)](#2944896454) | Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/index.md) from the specified if statement\. |
| [SimpleIfStatementInfo(SyntaxNode, Boolean, Boolean)](#2920991926) | Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/index.md) from the specified node\. |

<a id="2944896454"></a>

## SimpleIfStatementInfo\(IfStatementSyntax, Boolean, Boolean\) 

  
Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/index.md) from the specified if statement\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleIfStatementInfo SimpleIfStatementInfo(Microsoft.CodeAnalysis.CSharp.Syntax.IfStatementSyntax ifStatement, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**ifStatement** &ensp; [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/index.md)

<a id="2920991926"></a>

## SimpleIfStatementInfo\(SyntaxNode, Boolean, Boolean\) 

  
Creates a new [SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/index.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.SimpleIfStatementInfo SimpleIfStatementInfo(Microsoft.CodeAnalysis.SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**walkDownParentheses** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowMissing** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

### Returns

[SimpleIfStatementInfo](../../Syntax/SimpleIfStatementInfo/index.md)

