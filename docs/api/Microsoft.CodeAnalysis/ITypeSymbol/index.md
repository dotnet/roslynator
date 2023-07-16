---
sidebar_label: ITypeSymbol
---

# [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol) Interface Extensions

| Extension Method | Summary |
| ---------------- | ------- |
| [BaseTypes(ITypeSymbol)](../../Roslynator/SymbolExtensions/BaseTypes/index.md) | Gets a list of base types of this type\. |
| [BaseTypesAndSelf(ITypeSymbol)](../../Roslynator/SymbolExtensions/BaseTypesAndSelf/index.md) | Gets a list of base types of this type \(including this type\)\. |
| [EqualsOrInheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](../../Roslynator/SymbolExtensions/EqualsOrInheritsFrom/index.md#3013860381) | Returns true if the type is equal or inherits from a specified base type\. |
| [EqualsOrInheritsFrom(ITypeSymbol, MetadataName, Boolean)](../../Roslynator/SymbolExtensions/EqualsOrInheritsFrom/index.md#1803936331) | Returns true if the type is equal or inherits from a type with the specified name\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, Func&lt;TSymbol, Boolean&gt;)](../../Roslynator/SymbolExtensions/FindMember/index.md#2854901772) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, String, Func&lt;TSymbol, Boolean&gt;)](../../Roslynator/SymbolExtensions/FindMember/index.md#3171999706) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [GetDefaultValueSyntax(ITypeSymbol, DefaultSyntaxOptions, SymbolDisplayFormat)](../../Roslynator/CSharp/WorkspaceSymbolExtensions/GetDefaultValueSyntax/index.md#3187258133) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |
| [GetDefaultValueSyntax(ITypeSymbol, TypeSyntax, DefaultSyntaxOptions)](../../Roslynator/CSharp/WorkspaceSymbolExtensions/GetDefaultValueSyntax/index.md#2331338541) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |
| [HasAttribute(ITypeSymbol, INamedTypeSymbol, Boolean)](../../Roslynator/SymbolExtensions/HasAttribute/index.md#289352201) | Returns true if the type symbol has the specified attribute\. |
| [HasAttribute(ITypeSymbol, MetadataName, Boolean)](../../Roslynator/SymbolExtensions/HasAttribute/index.md#1814378823) | Returns true if the type symbol has attribute with the specified name\. |
| [Implements(ITypeSymbol, INamedTypeSymbol, Boolean)](../../Roslynator/SymbolExtensions/Implements/index.md#1804500735) | Returns true if the type implements specified interface\. |
| [Implements(ITypeSymbol, MetadataName, Boolean)](../../Roslynator/SymbolExtensions/Implements/index.md#3538366426) | Returns true if the type implements specified interface name\. |
| [Implements(ITypeSymbol, SpecialType, Boolean)](../../Roslynator/SymbolExtensions/Implements/index.md#2161671967) | Returns true if the type implements specified interface\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, Boolean)](../../Roslynator/SymbolExtensions/ImplementsAny/index.md#1018184594) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean)](../../Roslynator/SymbolExtensions/ImplementsAny/index.md#196953422) | Returns true if the type implements any of specified interfaces\. |
| [InheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](../../Roslynator/SymbolExtensions/InheritsFrom/index.md#2746876473) | Returns true if the type inherits from a specified base type\. |
| [InheritsFrom(ITypeSymbol, MetadataName, Boolean)](../../Roslynator/SymbolExtensions/InheritsFrom/index.md#3951984790) | Returns true if the type inherits from a type with the specified name\. |
| [IsIEnumerableOfT(ITypeSymbol)](../../Roslynator/SymbolExtensions/IsIEnumerableOfT/index.md) | Returns true if the type is [IEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsIEnumerableOrIEnumerableOfT(ITypeSymbol)](../../Roslynator/SymbolExtensions/IsIEnumerableOrIEnumerableOfT/index.md) | Returns true if the type is [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable) or [IEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsNullableOf(ITypeSymbol, ITypeSymbol)](../../Roslynator/SymbolExtensions/IsNullableOf/index.md#2277729142) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, SpecialType)](../../Roslynator/SymbolExtensions/IsNullableOf/index.md#467484347) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableType(ITypeSymbol)](../../Roslynator/SymbolExtensions/IsNullableType/index.md) | Returns true if the type is a nullable type\. |
| [IsObject(ITypeSymbol)](../../Roslynator/SymbolExtensions/IsObject/index.md) | Returns true if the type is [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\. |
| [IsReferenceTypeOrNullableType(ITypeSymbol)](../../Roslynator/SymbolExtensions/IsReferenceTypeOrNullableType/index.md) | Returns true if the type is a reference type or a nullable type\. |
| [IsString(ITypeSymbol)](../../Roslynator/SymbolExtensions/IsString/index.md) | Returns true if the type is [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\. |
| [IsVoid(ITypeSymbol)](../../Roslynator/SymbolExtensions/IsVoid/index.md) | Returns true if the type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [SupportsConstantValue(ITypeSymbol)](../../Roslynator/CSharp/SymbolExtensions/SupportsConstantValue/index.md) | Returns true if the specified type can be used to declare constant value\. |
| [SupportsExplicitDeclaration(ITypeSymbol)](../../Roslynator/SymbolExtensions/SupportsExplicitDeclaration/index.md) | Returns true if the type can be declared explicitly in a source code\. |
| [ToMinimalTypeSyntax(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](../../Roslynator/CSharp/SymbolExtensions/ToMinimalTypeSyntax/index.md#2161128311) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |
| [ToTypeSyntax(ITypeSymbol, SymbolDisplayFormat)](../../Roslynator/CSharp/SymbolExtensions/ToTypeSyntax/index.md#3779029411) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |

