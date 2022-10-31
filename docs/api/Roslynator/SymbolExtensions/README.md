# SymbolExtensions Class

[Home](../../README.md) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator](../README.md)

**Assembly**: Roslynator\.Core\.dll

  
A set of extension methods for [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol) and its derived types\.

```csharp
public static class SymbolExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [BaseTypes(ITypeSymbol)](BaseTypes/README.md) | Gets a list of base types of this type\. |
| [BaseTypesAndSelf(ITypeSymbol)](BaseTypesAndSelf/README.md) | Gets a list of base types of this type \(including this type\)\. |
| [EqualsOrInheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](EqualsOrInheritsFrom/README.md#3013860381) | Returns true if the type is equal or inherits from a specified base type\. |
| [EqualsOrInheritsFrom(ITypeSymbol, MetadataName, Boolean)](EqualsOrInheritsFrom/README.md#1803936331) | Returns true if the type is equal or inherits from a type with the specified name\. |
| [FindMember\<TSymbol\>(INamedTypeSymbol, Func\<TSymbol, Boolean\>, Boolean)](FindMember/README.md#996682075) | Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(INamedTypeSymbol, String, Func\<TSymbol, Boolean\>, Boolean)](FindMember/README.md#358208601) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(ITypeSymbol, Func\<TSymbol, Boolean\>)](FindMember/README.md#2854901772) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember\<TSymbol\>(ITypeSymbol, String, Func\<TSymbol, Boolean\>)](FindMember/README.md#3171999706) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, Func\<INamedTypeSymbol, Boolean\>, Boolean)](FindTypeMember/README.md#931525377) | Searches for a type member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, String, Func\<INamedTypeSymbol, Boolean\>, Boolean)](FindTypeMember/README.md#4255324844) | Searches for a type member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, String, Int32, Func\<INamedTypeSymbol, Boolean\>, Boolean)](FindTypeMember/README.md#3885424205) | Searches for a type member that has the specified name, arity and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [GetAttribute(ISymbol, INamedTypeSymbol)](GetAttribute/README.md#1998351864) | Returns the attribute for the symbol that matches the specified attribute class, or null if the symbol does not have the specified attribute\. |
| [GetAttribute(ISymbol, MetadataName)](GetAttribute/README.md#596707890) | Returns the attribute for the symbol that matches the specified name, or null if the symbol does not have the specified attribute\. |
| [HasAttribute(ISymbol, INamedTypeSymbol)](HasAttribute/README.md#3062983091) | Returns true if the symbol has the specified attribute\. |
| [HasAttribute(ISymbol, MetadataName)](HasAttribute/README.md#151999350) | Returns true if the symbol has attribute with the specified name\. |
| [HasAttribute(ITypeSymbol, INamedTypeSymbol, Boolean)](HasAttribute/README.md#289352201) | Returns true if the type symbol has the specified attribute\. |
| [HasAttribute(ITypeSymbol, MetadataName, Boolean)](HasAttribute/README.md#1814378823) | Returns true if the type symbol has attribute with the specified name\. |
| [HasConstantValue(IFieldSymbol, Boolean)](HasConstantValue/README.md#517191633) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Byte)](HasConstantValue/README.md#632566675) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Decimal)](HasConstantValue/README.md#1766013282) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Double)](HasConstantValue/README.md#3830902967) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Char)](HasConstantValue/README.md#2422502686) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Int16)](HasConstantValue/README.md#2355822098) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Int32)](HasConstantValue/README.md#73986555) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Int64)](HasConstantValue/README.md#1055217514) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, SByte)](HasConstantValue/README.md#423896798) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Single)](HasConstantValue/README.md#3527128280) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, String)](HasConstantValue/README.md#4220380739) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, UInt16)](HasConstantValue/README.md#1356363310) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, UInt32)](HasConstantValue/README.md#3116719099) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, UInt64)](HasConstantValue/README.md#193564189) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasMetadataName(ISymbol, MetadataName)](HasMetadataName/README.md) | Returns true if a symbol has the specified [MetadataName](../MetadataName/README.md)\. |
| [Implements(ITypeSymbol, INamedTypeSymbol, Boolean)](Implements/README.md#1804500735) | Returns true if the type implements specified interface\. |
| [Implements(ITypeSymbol, MetadataName, Boolean)](Implements/README.md#3538366426) | Returns true if the type implements specified interface name\. |
| [Implements(ITypeSymbol, SpecialType, Boolean)](Implements/README.md#2161671967) | Returns true if the type implements specified interface\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, Boolean)](ImplementsAny/README.md#1018184594) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean)](ImplementsAny/README.md#196953422) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsInterfaceMember(ISymbol, Boolean)](ImplementsInterfaceMember/README.md#1947636977) | Returns true if the symbol implements any interface member\. |
| [ImplementsInterfaceMember(ISymbol, INamedTypeSymbol, Boolean)](ImplementsInterfaceMember/README.md#1539225690) | Returns true if the symbol implements any member of the specified interface\. |
| [ImplementsInterfaceMember\<TSymbol\>(ISymbol, Boolean)](ImplementsInterfaceMember/README.md#270427832) | Returns true if the symbol implements any interface member\. |
| [ImplementsInterfaceMember\<TSymbol\>(ISymbol, INamedTypeSymbol, Boolean)](ImplementsInterfaceMember/README.md#2598799324) | Returns true if the symbol implements any member of the specified interface\. |
| [InheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](InheritsFrom/README.md#2746876473) | Returns true if the type inherits from a specified base type\. |
| [InheritsFrom(ITypeSymbol, MetadataName, Boolean)](InheritsFrom/README.md#3951984790) | Returns true if the type inherits from a type with the specified name\. |
| [IsAsyncMethod(ISymbol)](IsAsyncMethod/README.md) | Returns true if the symbol is an async method\. |
| [IsErrorType(ISymbol)](IsErrorType/README.md) | Returns true if the symbol represents an error\. |
| [IsIEnumerableOfT(ITypeSymbol)](IsIEnumerableOfT/README.md) | Returns true if the type is [IEnumerable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsIEnumerableOrIEnumerableOfT(ITypeSymbol)](IsIEnumerableOrIEnumerableOfT/README.md) | Returns true if the type is [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable) or [IEnumerable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind, SymbolKind)](IsKind/README.md#2562543075) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind)](IsKind/README.md#3941599818) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind)](IsKind/README.md#144279932) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind)](IsKind/README.md#2288796010) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind)](IsKind/README.md#2241854371) | Returns true if the symbol is the specified kind\. |
| [IsNullableOf(INamedTypeSymbol, ITypeSymbol)](IsNullableOf/README.md#831430666) | Returns true if the type is [Nullable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(INamedTypeSymbol, SpecialType)](IsNullableOf/README.md#1928104294) | Returns true if the type is [Nullable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, ITypeSymbol)](IsNullableOf/README.md#2277729142) | Returns true if the type is [Nullable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, SpecialType)](IsNullableOf/README.md#467484347) | Returns true if the type is [Nullable\<T\>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableType(ITypeSymbol)](IsNullableType/README.md) | Returns true if the type is a nullable type\. |
| [IsObject(ITypeSymbol)](IsObject/README.md) | Returns true if the type is [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\. |
| [IsOrdinaryExtensionMethod(IMethodSymbol)](IsOrdinaryExtensionMethod/README.md) | Returns true if this method is an ordinary extension method \(i\.e\. "this" parameter has not been removed\)\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType, SpecialType)](IsParameterArrayOf/README.md#527099619) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType)](IsParameterArrayOf/README.md#2792278798) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType)](IsParameterArrayOf/README.md#3634009028) | Returns true if the parameter was declared as a parameter array that has a specified element type\. |
| [IsPubliclyVisible(ISymbol)](IsPubliclyVisible/README.md) | Return true if the specified symbol is publicly visible\. |
| [IsReducedExtensionMethod(IMethodSymbol)](IsReducedExtensionMethod/README.md) | Returns true if this method is a reduced extension method\. |
| [IsReferenceTypeOrNullableType(ITypeSymbol)](IsReferenceTypeOrNullableType/README.md) | Returns true if the type is a reference type or a nullable type\. |
| [IsRefOrOut(IParameterSymbol)](IsRefOrOut/README.md) | Returns true if the parameter was declared as "ref" or "out" parameter\. |
| [IsString(ITypeSymbol)](IsString/README.md) | Returns true if the type is [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\. |
| [IsVoid(ITypeSymbol)](IsVoid/README.md) | Returns true if the type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReducedFromOrSelf(IMethodSymbol)](ReducedFromOrSelf/README.md) | If this method is a reduced extension method, returns the definition of extension method from which this was reduced\. Otherwise, returns this symbol\. |
| [SupportsExplicitDeclaration(ITypeSymbol)](SupportsExplicitDeclaration/README.md) | Returns true if the type can be declared explicitly in a source code\. |

