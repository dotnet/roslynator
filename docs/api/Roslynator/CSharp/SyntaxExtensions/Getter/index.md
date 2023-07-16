---
sidebar_label: Getter
---

# SyntaxExtensions\.Getter Method

**Containing Type**: [SyntaxExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Getter(AccessorListSyntax)](#3749591364) | Returns a get accessor contained in the specified list\. |
| [Getter(IndexerDeclarationSyntax)](#2491107778) | Returns a get accessor that is contained in the specified indexer declaration\. |
| [Getter(PropertyDeclarationSyntax)](#2677777844) | Returns property get accessor, if any\. |

<a id="3749591364"></a>

## Getter\(AccessorListSyntax\) 

  
Returns a get accessor contained in the specified list\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Getter(this Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
```

### Parameters

**accessorList** &ensp; [AccessorListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessorlistsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

<a id="2491107778"></a>

## Getter\(IndexerDeclarationSyntax\) 

  
Returns a get accessor that is contained in the specified indexer declaration\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Getter(this Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax indexerDeclaration)
```

### Parameters

**indexerDeclaration** &ensp; [IndexerDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.indexerdeclarationsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

<a id="2677777844"></a>

## Getter\(PropertyDeclarationSyntax\) 

  
Returns property get accessor, if any\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Getter(this Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax propertyDeclaration)
```

### Parameters

**propertyDeclaration** &ensp; [PropertyDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.propertydeclarationsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

