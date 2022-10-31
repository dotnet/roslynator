# SyntaxExtensions\.Setter Method

[Home](../../../../README.md)

**Containing Type**: [SyntaxExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Setter(AccessorListSyntax)](#Roslynator_CSharp_SyntaxExtensions_Setter_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorListSyntax_) | Returns a set accessor contained in the specified list\. |
| [Setter(IndexerDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_Setter_Microsoft_CodeAnalysis_CSharp_Syntax_IndexerDeclarationSyntax_) | Returns a set accessor that is contained in the specified indexer declaration\. |
| [Setter(PropertyDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_Setter_Microsoft_CodeAnalysis_CSharp_Syntax_PropertyDeclarationSyntax_) | Returns property set accessor, if any\. |

## Setter\(AccessorListSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_Setter_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorListSyntax_"></a>

\
Returns a set accessor contained in the specified list\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Setter(this Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList)
```

### Parameters

**accessorList** &ensp; [AccessorListSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessorlistsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

## Setter\(IndexerDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_Setter_Microsoft_CodeAnalysis_CSharp_Syntax_IndexerDeclarationSyntax_"></a>

\
Returns a set accessor that is contained in the specified indexer declaration\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Setter(this Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax indexerDeclaration)
```

### Parameters

**indexerDeclaration** &ensp; [IndexerDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.indexerdeclarationsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

## Setter\(PropertyDeclarationSyntax\) <a id="Roslynator_CSharp_SyntaxExtensions_Setter_Microsoft_CodeAnalysis_CSharp_Syntax_PropertyDeclarationSyntax_"></a>

\
Returns property set accessor, if any\.

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax Setter(this Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax propertyDeclaration)
```

### Parameters

**propertyDeclaration** &ensp; [PropertyDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.propertydeclarationsyntax)

### Returns

[AccessorDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.accessordeclarationsyntax)

