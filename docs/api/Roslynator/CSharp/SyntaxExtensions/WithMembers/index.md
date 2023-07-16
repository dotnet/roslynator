---
sidebar_label: WithMembers
---

# SyntaxExtensions\.WithMembers Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithMembers(ClassDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](#509233246) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(ClassDeclarationSyntax, MemberDeclarationSyntax)](#1261436636) | Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](#1993657641) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(CompilationUnitSyntax, MemberDeclarationSyntax)](#1847012895) | Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](#3092068364) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(InterfaceDeclarationSyntax, MemberDeclarationSyntax)](#3435655262) | Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](#1938122577) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(NamespaceDeclarationSyntax, MemberDeclarationSyntax)](#1822379855) | Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](#546601379) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(RecordDeclarationSyntax, MemberDeclarationSyntax)](#2335486806) | Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;)](#3849653050) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |
| [WithMembers(StructDeclarationSyntax, MemberDeclarationSyntax)](#2059906427) | Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\. |

<a id="509233246"></a>

## WithMembers\(ClassDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;\) 

  
Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**classDeclaration** &ensp; [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

<a id="1261436636"></a>

## WithMembers\(ClassDeclarationSyntax, MemberDeclarationSyntax\) 

  
Creates a new [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**classDeclaration** &ensp; [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax)

<a id="1993657641"></a>

## WithMembers\(CompilationUnitSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;\) 

  
Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**compilationUnit** &ensp; [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

<a id="1847012895"></a>

## WithMembers\(CompilationUnitSyntax, MemberDeclarationSyntax\) 

  
Creates a new [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**compilationUnit** &ensp; [CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[CompilationUnitSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.compilationunitsyntax)

<a id="3092068364"></a>

## WithMembers\(InterfaceDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;\) 

  
Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax interfaceDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**interfaceDeclaration** &ensp; [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

<a id="3435655262"></a>

## WithMembers\(InterfaceDeclarationSyntax, MemberDeclarationSyntax\) 

  
Creates a new [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax interfaceDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**interfaceDeclaration** &ensp; [InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[InterfaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.interfacedeclarationsyntax)

<a id="1938122577"></a>

## WithMembers\(NamespaceDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;\) 

  
Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax namespaceDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**namespaceDeclaration** &ensp; [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

<a id="1822379855"></a>

## WithMembers\(NamespaceDeclarationSyntax, MemberDeclarationSyntax\) 

  
Creates a new [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax namespaceDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**namespaceDeclaration** &ensp; [NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[NamespaceDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namespacedeclarationsyntax)

<a id="546601379"></a>

## WithMembers\(RecordDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;\) 

  
Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax recordDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**recordDeclaration** &ensp; [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

<a id="2335486806"></a>

## WithMembers\(RecordDeclarationSyntax, MemberDeclarationSyntax\) 

  
Creates a new [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax recordDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**recordDeclaration** &ensp; [RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[RecordDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.recorddeclarationsyntax)

<a id="3849653050"></a>

## WithMembers\(StructDeclarationSyntax, IEnumerable&lt;MemberDeclarationSyntax&gt;\) 

  
Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax structDeclaration, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members)
```

### Parameters

**structDeclaration** &ensp; [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

**members** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)&lt;[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)&gt;

### Returns

[StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

<a id="2059906427"></a>

## WithMembers\(StructDeclarationSyntax, MemberDeclarationSyntax\) 

  
Creates a new [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax) with the members updated\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax WithMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax structDeclaration, Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**structDeclaration** &ensp; [StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[StructDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.structdeclarationsyntax)

