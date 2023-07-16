---
sidebar_label: IfStatementOrElseClause
---

# IfStatementOrElseClause Struct

**Namespace**: [Roslynator.CSharp](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
A wrapper for either an [IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax) or an [ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax)\.

```csharp
public readonly struct IfStatementOrElseClause : IEquatable<Roslynator.CSharp.IfStatementOrElseClause>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; IfStatementOrElseClause

### Implements

* [IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)&lt;[IfStatementOrElseClause](./index.md)&gt;

## Constructors

| Constructor | Summary |
| ----------- | ------- |
| [IfStatementOrElseClause(ElseClauseSyntax)](-ctor/index.md#3645664103) | |
| [IfStatementOrElseClause(IfStatementSyntax)](-ctor/index.md#100512352) | |

## Properties

| Property | Summary |
| -------- | ------- |
| [FullSpan](FullSpan/index.md) | The absolute span of this node in characters, including its leading and trailing trivia\. |
| [IsElse](IsElse/index.md) | Determines whether this [IfStatementOrElseClause](./index.md) is wrapping an else clause\. |
| [IsIf](IsIf/index.md) | Determines whether this [IfStatementOrElseClause](./index.md) is wrapping an if statement\. |
| [Kind](Kind/index.md) | Gets an underlying node kind\. |
| [Parent](Parent/index.md) | The node that contains the underlying node in its [SyntaxNode.ChildNodes](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode.childnodes) collection\. |
| [Span](Span/index.md) | The absolute span of this node in characters, not including its leading and trailing trivia\. |
| [Statement](Statement/index.md) | Gets [IfStatementSyntax.Statement](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax.statement) or [ElseClauseSyntax.Statement](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax.statement)\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [AsElse()](AsElse/index.md) | Returns the underlying else clause if this [ElseClauseSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.elseclausesyntax) is wrapping else clause\. |
| [AsIf()](AsIf/index.md) | Returns the underlying if statement if this [IfStatementOrElseClause](./index.md) is wrapping if statement\. |
| [Equals(IfStatementOrElseClause)](Equals/index.md#4208149588) | Determines whether this instance is equal to another object of the same type\. \(Implements [IEquatable&lt;IfStatementOrElseClause&gt;.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/index.md#3273483447) | Determines whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [GetHashCode()](GetHashCode/index.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [ToString()](ToString/index.md) | Returns the string representation of the underlying node, not including its leading and trailing trivia\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(IfStatementOrElseClause, IfStatementOrElseClause)](op_Equality/index.md) | |
| [Implicit(ElseClauseSyntax to IfStatementOrElseClause)](op_Implicit/index.md#2383111912) | |
| [Implicit(IfStatementOrElseClause to ElseClauseSyntax)](op_Implicit/index.md#832248303) | |
| [Implicit(IfStatementOrElseClause to IfStatementSyntax)](op_Implicit/index.md#1165473507) | |
| [Implicit(IfStatementSyntax to IfStatementOrElseClause)](op_Implicit/index.md#3428183196) | |
| [Inequality(IfStatementOrElseClause, IfStatementOrElseClause)](op_Inequality/index.md) | |

