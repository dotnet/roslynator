---
sidebar_label: SymbolExtensions
---

# SymbolExtensions Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
A set of extension methods for [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol) and its derived types\.

```csharp
public static class SymbolExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [BaseTypes(ITypeSymbol)](BaseTypes/index.md) | Gets a list of base types of this type\. |
| [BaseTypesAndSelf(ITypeSymbol)](BaseTypesAndSelf/index.md) | Gets a list of base types of this type \(including this type\)\. |
| [EqualsOrInheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](EqualsOrInheritsFrom/index.md#3013860381) | Returns true if the type is equal or inherits from a specified base type\. |
| [EqualsOrInheritsFrom(ITypeSymbol, MetadataName, Boolean)](EqualsOrInheritsFrom/index.md#1803936331) | Returns true if the type is equal or inherits from a type with the specified name\. |
| [FindMember&lt;TSymbol&gt;(INamedTypeSymbol, Func&lt;TSymbol, Boolean&gt;, Boolean)](FindMember/index.md#996682075) | Searches for a member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(INamedTypeSymbol, String, Func&lt;TSymbol, Boolean&gt;, Boolean)](FindMember/index.md#358208601) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, Func&lt;TSymbol, Boolean&gt;)](FindMember/index.md#2854901772) | Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindMember&lt;TSymbol&gt;(ITypeSymbol, String, Func&lt;TSymbol, Boolean&gt;)](FindMember/index.md#3171999706) | Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean)](FindTypeMember/index.md#931525377) | Searches for a type member that matches the conditions defined by the specified predicate and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, String, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean)](FindTypeMember/index.md#4255324844) | Searches for a type member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [FindTypeMember(INamedTypeSymbol, String, Int32, Func&lt;INamedTypeSymbol, Boolean&gt;, Boolean)](FindTypeMember/index.md#3885424205) | Searches for a type member that has the specified name, arity and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members\. |
| [GetAttribute(ISymbol, INamedTypeSymbol)](GetAttribute/index.md#1998351864) | Returns the attribute for the symbol that matches the specified attribute class, or null if the symbol does not have the specified attribute\. |
| [GetAttribute(ISymbol, MetadataName)](GetAttribute/index.md#596707890) | Returns the attribute for the symbol that matches the specified name, or null if the symbol does not have the specified attribute\. |
| [HasAttribute(ISymbol, INamedTypeSymbol)](HasAttribute/index.md#3062983091) | Returns true if the symbol has the specified attribute\. |
| [HasAttribute(ISymbol, MetadataName)](HasAttribute/index.md#151999350) | Returns true if the symbol has attribute with the specified name\. |
| [HasAttribute(ITypeSymbol, INamedTypeSymbol, Boolean)](HasAttribute/index.md#289352201) | Returns true if the type symbol has the specified attribute\. |
| [HasAttribute(ITypeSymbol, MetadataName, Boolean)](HasAttribute/index.md#1814378823) | Returns true if the type symbol has attribute with the specified name\. |
| [HasConstantValue(IFieldSymbol, Boolean)](HasConstantValue/index.md#517191633) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Byte)](HasConstantValue/index.md#632566675) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Decimal)](HasConstantValue/index.md#1766013282) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Double)](HasConstantValue/index.md#3830902967) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Char)](HasConstantValue/index.md#2422502686) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Int16)](HasConstantValue/index.md#2355822098) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Int32)](HasConstantValue/index.md#73986555) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Int64)](HasConstantValue/index.md#1055217514) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, SByte)](HasConstantValue/index.md#423896798) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, Single)](HasConstantValue/index.md#3527128280) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, String)](HasConstantValue/index.md#4220380739) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, UInt16)](HasConstantValue/index.md#1356363310) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, UInt32)](HasConstantValue/index.md#3116719099) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasConstantValue(IFieldSymbol, UInt64)](HasConstantValue/index.md#193564189) | Get a value indicating whether the field symbol has specified constant value\. |
| [HasMetadataName(ISymbol, MetadataName)](HasMetadataName/index.md) | Returns true if a symbol has the specified [MetadataName](../MetadataName/index.md)\. |
| [Implements(ITypeSymbol, INamedTypeSymbol, Boolean)](Implements/index.md#1804500735) | Returns true if the type implements specified interface\. |
| [Implements(ITypeSymbol, MetadataName, Boolean)](Implements/index.md#3538366426) | Returns true if the type implements specified interface name\. |
| [Implements(ITypeSymbol, SpecialType, Boolean)](Implements/index.md#2161671967) | Returns true if the type implements specified interface\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, Boolean)](ImplementsAny/index.md#1018184594) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean)](ImplementsAny/index.md#196953422) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsInterfaceMember(ISymbol, Boolean)](ImplementsInterfaceMember/index.md#1947636977) | Returns true if the symbol implements any interface member\. |
| [ImplementsInterfaceMember(ISymbol, INamedTypeSymbol, Boolean)](ImplementsInterfaceMember/index.md#1539225690) | Returns true if the symbol implements any member of the specified interface\. |
| [ImplementsInterfaceMember&lt;TSymbol&gt;(ISymbol, Boolean)](ImplementsInterfaceMember/index.md#270427832) | Returns true if the symbol implements any interface member\. |
| [ImplementsInterfaceMember&lt;TSymbol&gt;(ISymbol, INamedTypeSymbol, Boolean)](ImplementsInterfaceMember/index.md#2598799324) | Returns true if the symbol implements any member of the specified interface\. |
| [InheritsFrom(ITypeSymbol, ITypeSymbol, Boolean)](InheritsFrom/index.md#2746876473) | Returns true if the type inherits from a specified base type\. |
| [InheritsFrom(ITypeSymbol, MetadataName, Boolean)](InheritsFrom/index.md#3951984790) | Returns true if the type inherits from a type with the specified name\. |
| [IsAsyncMethod(ISymbol)](IsAsyncMethod/index.md) | Returns true if the symbol is an async method\. |
| [IsErrorType(ISymbol)](IsErrorType/index.md) | Returns true if the symbol represents an error\. |
| [IsIEnumerableOfT(ITypeSymbol)](IsIEnumerableOfT/index.md) | Returns true if the type is [IEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsIEnumerableOrIEnumerableOfT(ITypeSymbol)](IsIEnumerableOrIEnumerableOfT/index.md) | Returns true if the type is [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable) or [IEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind, SymbolKind)](IsKind/index.md#2562543075) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind, SymbolKind)](IsKind/index.md#3941599818) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind, SymbolKind)](IsKind/index.md#144279932) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind, SymbolKind)](IsKind/index.md#2288796010) | Returns true if the symbol is one of the specified kinds\. |
| [IsKind(ISymbol, SymbolKind)](IsKind/index.md#2241854371) | Returns true if the symbol is the specified kind\. |
| [IsNullableOf(INamedTypeSymbol, ITypeSymbol)](IsNullableOf/index.md#831430666) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(INamedTypeSymbol, SpecialType)](IsNullableOf/index.md#1928104294) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, ITypeSymbol)](IsNullableOf/index.md#2277729142) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, SpecialType)](IsNullableOf/index.md#467484347) | Returns true if the type is [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableType(ITypeSymbol)](IsNullableType/index.md) | Returns true if the type is a nullable type\. |
| [IsObject(ITypeSymbol)](IsObject/index.md) | Returns true if the type is [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\. |
| [IsOrdinaryExtensionMethod(IMethodSymbol)](IsOrdinaryExtensionMethod/index.md) | Returns true if this method is an ordinary extension method \(i\.e\. "this" parameter has not been removed\)\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType, SpecialType)](IsParameterArrayOf/index.md#527099619) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType)](IsParameterArrayOf/index.md#2792278798) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType)](IsParameterArrayOf/index.md#3634009028) | Returns true if the parameter was declared as a parameter array that has a specified element type\. |
| [IsPubliclyVisible(ISymbol)](IsPubliclyVisible/index.md) | Return true if the specified symbol is publicly visible\. |
| [IsReducedExtensionMethod(IMethodSymbol)](IsReducedExtensionMethod/index.md) | Returns true if this method is a reduced extension method\. |
| [IsReferenceTypeOrNullableType(ITypeSymbol)](IsReferenceTypeOrNullableType/index.md) | Returns true if the type is a reference type or a nullable type\. |
| [IsRefOrOut(IParameterSymbol)](IsRefOrOut/index.md) | Returns true if the parameter was declared as "ref" or "out" parameter\. |
| [IsString(ITypeSymbol)](IsString/index.md) | Returns true if the type is [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\. |
| [IsVoid(ITypeSymbol)](IsVoid/index.md) | Returns true if the type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReducedFromOrSelf(IMethodSymbol)](ReducedFromOrSelf/index.md) | If this method is a reduced extension method, returns the definition of extension method from which this was reduced\. Otherwise, returns this symbol\. |
| [SupportsExplicitDeclaration(ITypeSymbol)](SupportsExplicitDeclaration/index.md) | Returns true if the type can be declared explicitly in a source code\. |

