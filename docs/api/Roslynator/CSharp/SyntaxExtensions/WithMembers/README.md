# SyntaxExtensions\.WithMembers Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithMembers(ClassDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_ClassDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(ClassDeclarationSyntax, MemberDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_ClassDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, IEnumerable\<MemberDeclarationSyntax>)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_CompilationUnitSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, MemberDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_CompilationUnitSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_InterfaceDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, MemberDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_InterfaceDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, MemberDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_RecordDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, MemberDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_RecordDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_StructDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, MemberDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_StructDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |

## WithMembers\(ClassDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_ClassDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

\
Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**classDeclaration** &ensp; [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

## WithMembers\(ClassDeclarationSyntax, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_ClassDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**classDeclaration** &ensp; [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

## WithMembers\(CompilationUnitSyntax, IEnumerable\<MemberDeclarationSyntax>\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_CompilationUnitSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

\
Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**compilationUnit** &ensp; [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

## WithMembers\(CompilationUnitSyntax, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_CompilationUnitSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**compilationUnit** &ensp; [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

## WithMembers\(InterfaceDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_InterfaceDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

\
Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax interfaceDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**interfaceDeclaration** &ensp; [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

### Returns

[InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

## WithMembers\(InterfaceDeclarationSyntax, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_InterfaceDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax interfaceDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**interfaceDeclaration** &ensp; [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

## WithMembers\(NamespaceDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

\
Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax namespaceDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**namespaceDeclaration** &ensp; [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

### Returns

[NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

## WithMembers\(NamespaceDeclarationSyntax, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax namespaceDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**namespaceDeclaration** &ensp; [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

## WithMembers\(RecordDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_RecordDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

\
Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax recordDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**recordDeclaration** &ensp; [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

### Returns

[RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

## WithMembers\(RecordDeclarationSyntax, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_RecordDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax recordDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**recordDeclaration** &ensp; [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

## WithMembers\(StructDeclarationSyntax, IEnumerable\<MemberDeclarationSyntax>\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_StructDeclarationSyntax_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax__"></a>

\
Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax structDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**structDeclaration** &ensp; [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)>

### Returns

[StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

## WithMembers\(StructDeclarationSyntax, MemberDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_WithMembers_Microsoft_CodeAnalysis_CSharp_Syntax_StructDeclarationSyntax_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax structDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**structDeclaration** &ensp; [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

