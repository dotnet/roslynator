// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class ISymbolExtensions
    {
        [DebuggerStepThrough]
        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind)
        {
            return methodSymbol?.MethodKind == methodKind;
        }

        [DebuggerStepThrough]
        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind1, MethodKind methodKind2)
        {
            if (methodSymbol == null)
                return false;

            MethodKind methodKind = methodSymbol.MethodKind;

            return methodKind == methodKind1
                || methodKind == methodKind2;
        }

        [DebuggerStepThrough]
        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind1, MethodKind methodKind2, MethodKind methodKind3)
        {
            if (methodSymbol == null)
                return false;

            MethodKind methodKind = methodSymbol.MethodKind;

            return methodKind == methodKind1
                || methodKind == methodKind2
                || methodKind == methodKind3;
        }

        public static IEnumerable<IMethodSymbol> OverridenMethods(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            IMethodSymbol overridenMethod = methodSymbol.OverriddenMethod;

            while (overridenMethod != null)
            {
                yield return overridenMethod;

                overridenMethod = overridenMethod.OverriddenMethod;
            }
        }

        public static IParameterSymbol SingleParameterOrDefault(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (parameters.Length == 1)
            {
                return parameters.First();
            }
            else
            {
                return null;
            }
        }

        public static bool IsNullableOf(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && IsNullableOf((INamedTypeSymbol)typeSymbol, specialType);
        }

        public static bool IsNullableOf(this ITypeSymbol typeSymbol, ITypeSymbol typeArgument)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && IsNullableOf((INamedTypeSymbol)typeSymbol, typeArgument);
        }

        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            return namedTypeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T)
                && namedTypeSymbol.TypeArguments[0].SpecialType == specialType;
        }

        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeArgument)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (typeArgument == null)
                throw new ArgumentNullException(nameof(typeArgument));

            return namedTypeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T)
                && namedTypeSymbol.TypeArguments[0] == typeArgument;
        }

        public static IEnumerable<INamespaceSymbol> ContainingNamespacesAndSelf(this INamespaceSymbol @namespace)
        {
            if (@namespace == null)
                throw new ArgumentNullException(nameof(@namespace));

            do
            {
                yield return @namespace;

                @namespace = @namespace.ContainingNamespace;

            } while (@namespace != null);
        }

        [DebuggerStepThrough]
        public static bool IsKind(this ISymbol symbol, SymbolKind kind)
        {
            return symbol?.Kind == kind;
        }

        [DebuggerStepThrough]
        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2;
        }

        [DebuggerStepThrough]
        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        [DebuggerStepThrough]
        public static bool IsPublic(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Public;
        }

        [DebuggerStepThrough]
        public static bool IsInternal(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Internal;
        }

        [DebuggerStepThrough]
        public static bool IsProtected(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Protected;
        }

        [DebuggerStepThrough]
        public static bool IsPrivate(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Private;
        }

        [DebuggerStepThrough]
        public static bool IsArrayType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.ArrayType;
        }

        [DebuggerStepThrough]
        public static bool IsDynamicType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.DynamicType;
        }

        [DebuggerStepThrough]
        public static bool IsErrorType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.ErrorType;
        }

        [DebuggerStepThrough]
        public static bool IsEvent(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Event;
        }

        [DebuggerStepThrough]
        public static bool IsField(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Field;
        }

        [DebuggerStepThrough]
        public static bool IsLocal(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Local;
        }

        [DebuggerStepThrough]
        public static bool IsMethod(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Method;
        }

        [DebuggerStepThrough]
        public static bool IsNamedType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.NamedType;
        }

        [DebuggerStepThrough]
        public static bool IsNamespace(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Namespace;
        }

        [DebuggerStepThrough]
        public static bool IsParameter(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Parameter;
        }

        [DebuggerStepThrough]
        public static bool IsProperty(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Property;
        }

        [DebuggerStepThrough]
        public static bool IsTypeParameter(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.TypeParameter;
        }

        public static IEnumerable<INamespaceSymbol> ContainingNamespaces(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            INamespaceSymbol containingNamespace = symbol.ContainingNamespace;

            do
            {
                yield return containingNamespace;

                containingNamespace = containingNamespace.ContainingNamespace;

            } while (containingNamespace != null);
        }

        public static bool IsPubliclyAccessible(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            Accessibility accessibility = typeSymbol.DeclaredAccessibility;

            return accessibility == Accessibility.Protected
                || accessibility == Accessibility.ProtectedOrInternal
                || accessibility == Accessibility.Public;
        }

        [DebuggerStepThrough]
        public static bool IsVoid(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Void;
        }

        [DebuggerStepThrough]
        public static bool IsInt32(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Int32;
        }

        [DebuggerStepThrough]
        public static bool IsBoolean(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Boolean;
        }

        [DebuggerStepThrough]
        public static bool IsString(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_String;
        }

        [DebuggerStepThrough]
        public static bool IsObject(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Object;
        }

        [DebuggerStepThrough]
        public static bool IsChar(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Char;
        }

        public static IEnumerable<INamedTypeSymbol> BaseTypes(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            INamedTypeSymbol baseType = typeSymbol.BaseType;

            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        public static IEnumerable<ITypeSymbol> BaseTypesAndSelf(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ITypeSymbol current = typeSymbol;

            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool Implements(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> allInterfaces = typeSymbol.AllInterfaces;

            for (int i = 0; i < allInterfaces.Length; i++)
            {
                if (allInterfaces[i].SpecialType == specialType)
                    return true;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Class;
        }

        [DebuggerStepThrough]
        public static bool IsInterface(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Interface;
        }

        [DebuggerStepThrough]
        public static bool IsStruct(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Struct;
        }

        [DebuggerStepThrough]
        public static bool IsEnum(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Enum;
        }

        public static bool IsConstructedFrom(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
        {
            return namedTypeSymbol?.ConstructedFrom?.SpecialType == specialType;
        }

        public static bool IsConstructedFrom(this INamedTypeSymbol namedTypeSymbol, ISymbol symbol)
        {
            return namedTypeSymbol?.ConstructedFrom?.Equals(symbol) == true;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            return typeSymbol?.IsNamedType() == true
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom?.SpecialType == specialType;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, ISymbol symbol)
        {
            return typeSymbol?.IsNamedType() == true
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom?.Equals(symbol) == true;
        }

        public static bool IsIEnumerable(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Collections_IEnumerable;
        }

        public static bool IsConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsConstructedFrom(typeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        public static bool IsConstructedFromIEnumerableOfT(this INamedTypeSymbol namedTypeSymbol)
        {
            return IsConstructedFrom(namedTypeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        public static bool IsIEnumerableOrConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsIEnumerable(typeSymbol)
                || IsConstructedFromIEnumerableOfT(typeSymbol);
        }

        public static bool EqualsOrDerivedFrom(this ITypeSymbol typeSymbol, ISymbol symbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.Equals(symbol)
                || typeSymbol.BaseTypes().Any(f => f.Equals(symbol));
        }

        [DebuggerStepThrough]
        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind)
        {
            return typeSymbol?.TypeKind == typeKind;
        }

        [DebuggerStepThrough]
        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2;
        }

        [DebuggerStepThrough]
        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2, TypeKind typeKind3)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2
                || typeKind == typeKind3;
        }

        public static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Field)
                    yield return (IFieldSymbol)members[i];
            }
        }

        public static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers(name);

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Field)
                    yield return (IFieldSymbol)members[i];
            }
        }

        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Method)
                    yield return (IMethodSymbol)members[i];
            }
        }

        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers(name);

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Method)
                    yield return (IMethodSymbol)members[i];
            }
        }
    }
}
