---
sidebar_label: Setter
---

# SyntaxExtensions\.Setter Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Setter(AccessorListSyntax)](#1651493367) | Returns a set accessor contained in the specified list\. |
| [Setter(IndexerDeclarationSyntax)](#1041810977) | Returns a set accessor that is contained in the specified indexer declaration\. |
| [Setter(PropertyDeclarationSyntax)](#2111161647) | Returns property set accessor, if any\. |

<a id="1651493367"></a>

## Setter\(AccessorListSyntax\) 

  
Returns a set accessor contained in the specified list\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Setter(this Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
```

### Parameters

**accessorList** &ensp; [AccessorListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessorlistsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

<a id="1041810977"></a>

## Setter\(IndexerDeclarationSyntax\) 

  
Returns a set accessor that is contained in the specified indexer declaration\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Setter(this Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax indexerDeclaration)
```

### Parameters

**indexerDeclaration** &ensp; [IndexerDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.indexerdeclarationsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

<a id="2111161647"></a>

## Setter\(PropertyDeclarationSyntax\) 

  
Returns property set accessor, if any\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Setter(this Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax propertyDeclaration)
```

### Parameters

**propertyDeclaration** &ensp; [PropertyDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.propertydeclarationsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

