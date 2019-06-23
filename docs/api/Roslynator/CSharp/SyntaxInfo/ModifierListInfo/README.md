# SyntaxInfo\.ModifierListInfo Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ModifierListInfo(AccessorDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified accessor declaration\. |
| [ModifierListInfo(ClassDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ClassDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified class declaration\. |
| [ModifierListInfo(ConstructorDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ConstructorDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified constructor declaration\. |
| [ModifierListInfo(ConversionOperatorDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ConversionOperatorDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified conversion operator declaration\. |
| [ModifierListInfo(DelegateDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_DelegateDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified delegate declaration\. |
| [ModifierListInfo(DestructorDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_DestructorDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified destructor declaration\. |
| [ModifierListInfo(EnumDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EnumDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified enum declaration\. |
| [ModifierListInfo(EventDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EventDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified event declaration\. |
| [ModifierListInfo(EventFieldDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EventFieldDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified event field declaration\. |
| [ModifierListInfo(FieldDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_FieldDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified field declaration\. |
| [ModifierListInfo(IncompleteMemberSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_IncompleteMemberSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified incomplete member\. |
| [ModifierListInfo(IndexerDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_IndexerDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified indexer declaration\. |
| [ModifierListInfo(InterfaceDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_InterfaceDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified interface declaration\. |
| [ModifierListInfo(LocalDeclarationStatementSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LocalDeclarationStatementSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified local declaration statement\. |
| [ModifierListInfo(LocalFunctionStatementSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified local function\. |
| [ModifierListInfo(MethodDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified method declaration\. |
| [ModifierListInfo(OperatorDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_OperatorDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified operator declaration\. |
| [ModifierListInfo(ParameterSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ParameterSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified parameter\. |
| [ModifierListInfo(PropertyDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_PropertyDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified property declaration\. |
| [ModifierListInfo(StructDeclarationSyntax)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_StructDeclarationSyntax_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified struct declaration\. |
| [ModifierListInfo(SyntaxNode)](#Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified node\. |

## ModifierListInfo\(AccessorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified accessor declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax accessorDeclaration)
```

### Parameters

**accessorDeclaration** &ensp; [AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(ClassDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ClassDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified class declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDeclaration)
```

### Parameters

**classDeclaration** &ensp; [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(ConstructorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ConstructorDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified constructor declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax constructorDeclaration)
```

### Parameters

**constructorDeclaration** &ensp; [ConstructorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.constructordeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(ConversionOperatorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ConversionOperatorDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified conversion operator declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
```

### Parameters

**conversionOperatorDeclaration** &ensp; [ConversionOperatorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.conversionoperatordeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(DelegateDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_DelegateDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified delegate declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax delegateDeclaration)
```

### Parameters

**delegateDeclaration** &ensp; [DelegateDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.delegatedeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(DestructorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_DestructorDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified destructor declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax destructorDeclaration)
```

### Parameters

**destructorDeclaration** &ensp; [DestructorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.destructordeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(EnumDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EnumDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified enum declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax enumDeclaration)
```

### Parameters

**enumDeclaration** &ensp; [EnumDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.enumdeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(EventDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EventDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified event declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax eventDeclaration)
```

### Parameters

**eventDeclaration** &ensp; [EventDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.eventdeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(EventFieldDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_EventFieldDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified event field declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax eventFieldDeclaration)
```

### Parameters

**eventFieldDeclaration** &ensp; [EventFieldDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.eventfielddeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(FieldDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_FieldDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified field declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax fieldDeclaration)
```

### Parameters

**fieldDeclaration** &ensp; [FieldDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.fielddeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(IncompleteMemberSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_IncompleteMemberSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified incomplete member\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax incompleteMember)
```

### Parameters

**incompleteMember** &ensp; [IncompleteMemberSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.incompletemembersyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(IndexerDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_IndexerDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified indexer declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax indexerDeclaration)
```

### Parameters

**indexerDeclaration** &ensp; [IndexerDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.indexerdeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(InterfaceDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_InterfaceDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified interface declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax interfaceDeclaration)
```

### Parameters

**interfaceDeclaration** &ensp; [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(LocalDeclarationStatementSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LocalDeclarationStatementSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified local declaration statement\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax localDeclarationStatement)
```

### Parameters

**localDeclarationStatement** &ensp; [LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(LocalFunctionStatementSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified local function\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax localFunctionStatement)
```

### Parameters

**localFunctionStatement** &ensp; [LocalFunctionStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localfunctionstatementsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(MethodDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified method declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax methodDeclaration)
```

### Parameters

**methodDeclaration** &ensp; [MethodDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.methoddeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(OperatorDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_OperatorDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified operator declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax operatorDeclaration)
```

### Parameters

**operatorDeclaration** &ensp; [OperatorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.operatordeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(ParameterSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_ParameterSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified parameter\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter)
```

### Parameters

**parameter** &ensp; [ParameterSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.parametersyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(PropertyDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_PropertyDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified property declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax propertyDeclaration)
```

### Parameters

**propertyDeclaration** &ensp; [PropertyDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.propertydeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(StructDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_CSharp_Syntax_StructDeclarationSyntax_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified struct declaration\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax structDeclaration)
```

### Parameters

**structDeclaration** &ensp; [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

## ModifierListInfo\(SyntaxNode\) <a id="Roslynator_CSharp_SyntaxInfo_ModifierListInfo_Microsoft_CodeAnalysis_SyntaxNode_"></a>

\
Creates a new [ModifierListInfo](../../Syntax/ModifierListInfo/README.md) from the specified node\.

```csharp
public static Roslynator.CSharp.Syntax.ModifierListInfo ModifierListInfo(Microsoft.CodeAnalysis.SyntaxNode node)
```

### Parameters

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

### Returns

[ModifierListInfo](../../Syntax/ModifierListInfo/README.md)

