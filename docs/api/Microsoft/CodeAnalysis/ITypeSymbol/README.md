# [ITypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol) Interface Extensions

[Home](../../../README.md)

| Extension Method | Summary |
| ---------------- | ------- |
| [BaseTypes(ITypeSymbol)](../../../Roslynator/SymbolExtensions/BaseTypes/README.md) | Gets a list of base types of this type\. |
| [BaseTypesAndSelf(ITypeSymbol)](../../../Roslynator/SymbolExtensions/BaseTypesAndSelf/README.md) | Gets a list of base types of this type \(including this type\)\. |
| [EqualsOrInheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](../../../Roslynator/SymbolExtensions/EqualsOrInheritsFrom/README.md#3013860381) | Returns true if the type is equal or inherits from a specified base type\. |
| [EqualsOrInheritsFrom(ITypeSymbol, MetadataName, Boolean)](../../../Roslynator/SymbolExtensions/EqualsOrInheritsFrom/README.md#1803936331) | Returns true if the type is equal or inherits from a type with the specified name\. |
| [FindMember\<TSymbol\>(ITypeSymbol, Func\<TSymbol, Boolean\>)](../../../Roslynator/SymbolExtensions/FindMember/README.md#2854901772) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(ITypeSymbol, String, Func\<TSymbol, Boolean\>)](../../../Roslynator/SymbolExtensions/FindMember/README.md#3171999706) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [GetDefaultValueSyntax(ITypeSymbol, DefaultSyntaxOptions, SymbolDisplayFormat)](../../../Roslynator/CSharp/WorkspaceSymbolExtensions/GetDefaultValueSyntax/README.md#3187258133) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |
| [GetDefaultValueSyntax(ITypeSymbol, TypeSyntax, DefaultSyntaxOptions)](../../../Roslynator/CSharp/WorkspaceSymbolExtensions/GetDefaultValueSyntax/README.md#2331338541) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |
| [HasAttribute(ITypeSymbol, INamedTypeSymbol, Boolean)](../../../Roslynator/SymbolExtensions/HasAttribute/README.md#289352201) | Returns true if the type symbol has the specified attribute\. |
| [HasAttribute(ITypeSymbol, MetadataName, Boolean)](../../../Roslynator/SymbolExtensions/HasAttribute/README.md#1814378823) | Returns true if the type symbol has attribute with the specified name\. |
| [Implements(ITypeSymbol, INamedTypeSymbol, Boolean)](../../../Roslynator/SymbolExtensions/Implements/README.md#1804500735) | Returns true if the type implements specified interface\. |
| [Implements(ITypeSymbol, MetadataName, Boolean)](../../../Roslynator/SymbolExtensions/Implements/README.md#3538366426) | Returns true if the type implements specified interface name\. |
| [Implements(ITypeSymbol, SpecialType, Boolean)](../../../Roslynator/SymbolExtensions/Implements/README.md#2161671967) | Returns true if the type implements specified interface\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, Boolean)](../../../Roslynator/SymbolExtensions/ImplementsAny/README.md#1018184594) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean)](../../../Roslynator/SymbolExtensions/ImplementsAny/README.md#196953422) | Returns true if the type implements any of specified interfaces\. |
| [InheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](../../../Roslynator/SymbolExtensions/InheritsFrom/README.md#2746876473) | Returns true if the type inherits from a specified base type\. |
| [InheritsFrom(ITypeSymbol, MetadataName, Boolean)](../../../Roslynator/SymbolExtensions/InheritsFrom/README.md#3951984790) | Returns true if the type inherits from a type with the specified name\. |
| [IsIEnumerableOfT(ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsIEnumerableOfT/README.md) | Returns true if the type is [IEnumerable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsIEnumerableOrIEnumerableOfT(ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsIEnumerableOrIEnumerableOfT/README.md) | Returns true if the type is [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable) or [IEnumerable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsNullableOf(ITypeSymbol, ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsNullableOf/README.md#2277729142) | Returns true if the type is [Nullable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, SpecialType)](../../../Roslynator/SymbolExtensions/IsNullableOf/README.md#467484347) | Returns true if the type is [Nullable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableType(ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsNullableType/README.md) | Returns true if the type is a nullable type\. |
| [IsObject(ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsObject/README.md) | Returns true if the type is [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\. |
| [IsReferenceTypeOrNullableType(ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsReferenceTypeOrNullableType/README.md) | Returns true if the type is a reference type or a nullable type\. |
| [IsString(ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsString/README.md) | Returns true if the type is [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\. |
| [IsVoid(ITypeSymbol)](../../../Roslynator/SymbolExtensions/IsVoid/README.md) | Returns true if the type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [SupportsConstantValue(ITypeSymbol)](../../../Roslynator/CSharp/SymbolExtensions/SupportsConstantValue/README.md) | Returns true if the specified type can be used to declare constant value\. |
| [SupportsExplicitDeclaration(ITypeSymbol)](../../../Roslynator/SymbolExtensions/SupportsExplicitDeclaration/README.md) | Returns true if the type can be declared explicitly in a source code\. |
| [ToMinimalTypeSyntax(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](../../../Roslynator/CSharp/SymbolExtensions/ToMinimalTypeSyntax/README.md#2161128311) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |
| [ToTypeSyntax(ITypeSymbol, SymbolDisplayFormat)](../../../Roslynator/CSharp/SymbolExtensions/ToTypeSyntax/README.md#3779029411) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |

