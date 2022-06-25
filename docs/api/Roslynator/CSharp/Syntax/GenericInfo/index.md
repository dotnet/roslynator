---
sidebar_label: GenericInfo
---

# GenericInfo Struct

**Namespace**: [Roslynator.CSharp.Syntax](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

  
Provides information about generic syntax \(class, struct, interface, delegate, method or local function\)\.

```csharp
public readonly struct GenericInfo
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; GenericInfo

## Properties

| Property | Summary |
| -------- | ------- |
| [ConstraintClauses](ConstraintClauses/index.md) | A list of constraint clauses\. |
| [Kind](Kind/index.md) | The kind of this syntax node\. |
| [Node](Node/index.md) | The syntax node that can be generic \(for example [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) for a class or [LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax) for a local function\)\. |
| [Success](Success/index.md) | Determines whether this struct was initialized with an actual syntax\. |
| [TypeParameterList](TypeParameterList/index.md) | The type parameter list\. |
| [TypeParameters](TypeParameters/index.md) | A list of type parameters\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(Object)](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [FindConstraintClause(String)](FindConstraintClause/index.md) | Searches for a constraint clause with the specified type parameter name and returns the first occurrence within the constraint clauses\. |
| [FindTypeParameter(String)](FindTypeParameter/index.md) | Searches for a type parameter with the specified name and returns the first occurrence within the type parameters\. |
| [GetHashCode()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [RemoveAllConstraintClauses()](RemoveAllConstraintClauses/index.md) | Creates a new [GenericInfo](./index.md) with all constraint clauses removed\. |
| [RemoveConstraintClause(TypeParameterConstraintClauseSyntax)](RemoveConstraintClause/index.md) | Creates a new [GenericInfo](./index.md) with the specified constraint clause removed\. |
| [RemoveTypeParameter(TypeParameterSyntax)](RemoveTypeParameter/index.md) | Creates a new [GenericInfo](./index.md) with the specified type parameter removed\. |
| [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring) |  \(Inherited from [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype)\) |
| [WithConstraintClauses(SyntaxList&lt;TypeParameterConstraintClauseSyntax&gt;)](WithConstraintClauses/index.md) | Creates a new [GenericInfo](./index.md) with the constraint clauses updated\. |
| [WithTypeParameterList(TypeParameterListSyntax)](WithTypeParameterList/index.md) | Creates a new [GenericInfo](./index.md) with the type parameter list updated\. |

