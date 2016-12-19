// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class ISymbolExtensions
    {
        public static bool IsReducedExtension(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            return methodSymbol.MethodKind == MethodKind.ReducedExtension;
        }

        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind)
        {
            return methodSymbol?.MethodKind == methodKind;
        }

        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind1, MethodKind methodKind2)
        {
            if (methodSymbol == null)
                return false;

            MethodKind methodKind = methodSymbol.MethodKind;

            return methodKind == methodKind1
                || methodKind == methodKind2;
        }

        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind1, MethodKind methodKind2, MethodKind methodKind3)
        {
            if (methodSymbol == null)
                return false;

            MethodKind methodKind = methodSymbol.MethodKind;

            return methodKind == methodKind1
                || methodKind == methodKind2
                || methodKind == methodKind3;
        }

        internal static IMethodSymbol ReducedFromOrSelf(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.ReducedFrom ?? methodSymbol;
        }

        public static IEnumerable<IMethodSymbol> OverridenMethods(this IMethodSymbol methodSymbol)
        {
            IMethodSymbol overridenMethod = methodSymbol.OverriddenMethod;

            while (overridenMethod != null)
            {
                yield return overridenMethod;

                overridenMethod = overridenMethod.OverriddenMethod;
            }
        }

        public static bool IsConstructedFromNullableOf(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && IsConstructedFromNullableOf((INamedTypeSymbol)typeSymbol, specialType);
        }

        public static bool IsConstructedFromNullableOf(this ITypeSymbol typeSymbol, ITypeSymbol typeArgument)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && IsConstructedFromNullableOf((INamedTypeSymbol)typeSymbol, typeArgument);
        }

        public static bool IsConstructedFromNullableOf(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            return namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && namedTypeSymbol.TypeArguments[0].SpecialType == specialType;
        }

        public static bool IsConstructedFromNullableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeArgument)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (typeArgument == null)
                throw new ArgumentNullException(nameof(typeArgument));

            return namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && namedTypeSymbol.TypeArguments[0] == typeArgument;
        }

        public static bool IsAnyTypeArgumentAnonymousType(this INamedTypeSymbol namedType)
        {
            if (namedType == null)
                throw new ArgumentNullException(nameof(namedType));

            ImmutableArray<ITypeSymbol> typeArguments = namedType.TypeArguments;

            if (typeArguments.Length > 0)
            {
                var stack = new Stack<ITypeSymbol>(typeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    if (type.IsAnonymousType)
                        return true;

                    if (type.IsNamedType())
                    {
                        typeArguments = ((INamedTypeSymbol)type).TypeArguments;

                        for (int i = 0; i < typeArguments.Length; i++)
                            stack.Push(typeArguments[i]);
                    }
                }
            }

            return false;
        }

        public static IEnumerable<ITypeSymbol> GetAllTypeArguments(this INamedTypeSymbol namedType)
        {
            if (namedType == null)
                throw new ArgumentNullException(nameof(namedType));

            ImmutableArray<ITypeSymbol> typeArguments = namedType.TypeArguments;

            if (typeArguments.Length > 0)
            {
                var stack = new Stack<ITypeSymbol>(typeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    yield return type;

                    if (type.IsNamedType())
                    {
                        typeArguments = ((INamedTypeSymbol)type).TypeArguments;

                        for (int i = 0; i < typeArguments.Length; i++)
                            stack.Push(typeArguments[i]);
                    }
                }
            }
        }

        public static IEnumerable<ITypeSymbol> GetAllTypeArgumentsAndSelf(this INamedTypeSymbol namedType)
        {
            if (namedType == null)
                throw new ArgumentNullException(nameof(namedType));

            yield return namedType;

            foreach (ITypeSymbol typeSymbol in GetAllTypeArguments(namedType))
                yield return typeSymbol;
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

        public static IEnumerable<INamespaceSymbol> ContainingNamespaces(this INamespaceSymbol @namespace)
        {
            if (@namespace == null)
                throw new ArgumentNullException(nameof(@namespace));

            while (@namespace.ContainingNamespace != null)
            {
                yield return @namespace.ContainingNamespace;

                @namespace = @namespace.ContainingNamespace;
            }
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind)
        {
            return symbol?.Kind == kind;
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2;
        }

        public static ImmutableArray<IParameterSymbol> GetParameters(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).Parameters;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).Parameters;
                default:
                    return ImmutableArray<IParameterSymbol>.Empty;
            }
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

        public static bool HasDeclaredAccessibility(this ISymbol symbol, Accessibility accessibility)
        {
            return symbol?.DeclaredAccessibility == accessibility;
        }

        public static bool HasDeclaredAccessibility(this ISymbol symbol, Accessibility accessibility1, Accessibility accessibility2)
        {
            if (symbol == null)
                return false;

            Accessibility accessibility = symbol.DeclaredAccessibility;

            return accessibility == accessibility1
                || accessibility == accessibility2;
        }

        public static bool HasDeclaredAccessibility(this ISymbol symbol, Accessibility accessibility1, Accessibility accessibility2, Accessibility accessibility3)
        {
            if (symbol == null)
                return false;

            Accessibility accessibility = symbol.DeclaredAccessibility;

            return accessibility == accessibility1
                || accessibility == accessibility2
                || accessibility == accessibility3;
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
        public static bool IsAsyncMethod(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Method
                && ((IMethodSymbol)symbol).IsAsync;
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

        public static bool SupportsPrefixOrPostfixUnaryOperator(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Char:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Decimal:
                    return true;
            }

            if (typeSymbol.IsEnum())
                return true;

            return false;
        }

        public static bool SupportsExplicitDeclaration(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (!typeSymbol.IsAnonymousType)
            {
                switch (typeSymbol.Kind)
                {
                    case SymbolKind.TypeParameter:
                        return true;
                    case SymbolKind.ArrayType:
                        return SupportsExplicitDeclaration(((IArrayTypeSymbol)typeSymbol).ElementType);
                    case SymbolKind.NamedType:
                        return !((INamedTypeSymbol)typeSymbol).IsAnyTypeArgumentAnonymousType();
                }
            }

            return false;
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

        public static bool IsInt32(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Int32;
        }

        public static bool IsVoid(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Void;
        }

        public static bool IsBoolean(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Boolean;
        }

        public static bool IsString(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_String;
        }

        public static bool IsObject(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Object;
        }

        public static bool IsChar(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Char;
        }

        public static IEnumerable<INamedTypeSymbol> BaseTypes(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            INamedTypeSymbol current = typeSymbol.BaseType;

            while (current != null)
            {
                yield return current;
                current = current.BaseType;
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

        public static bool SupportsPredefinedType(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Object:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanBeConstantValue(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
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

        public static bool HasPublicIndexer(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            foreach (ISymbol symbol in typeSymbol.GetMembers("get_Item"))
            {
                if (symbol.IsMethod()
                    && !symbol.IsStatic
                    && symbol.IsPublic())
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                    if (parameters.Length == 1
                        && parameters[0].Type?.IsInt32() == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Class;
        }

        public static bool IsStaticClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.IsStatic && typeSymbol.IsClass();
        }

        public static bool IsInterface(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Interface;
        }

        public static bool IsStruct(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Struct;
        }

        public static bool IsEnum(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Enum;
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

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol.IsNamedType())
            {
                INamedTypeSymbol namedTypeSymbol = semanticModel
                    .Compilation
                    .GetTypeByMetadataName(fullyQualifiedMetadataName);

                return namedTypeSymbol != null
                    && ((INamedTypeSymbol)typeSymbol).ConstructedFrom?.Equals(namedTypeSymbol) == true;
            }

            return false;
        }

        public static bool IsIEnumerable(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Collections_IEnumerable;
        }

        public static bool IsConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsConstructedFrom(typeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        public static bool IsIEnumerableOrConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsIEnumerable(typeSymbol)
                || IsConstructedFromIEnumerableOfT(typeSymbol);
        }

        public static bool IsConstructedFromImmutableArrayOfT(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            return IsConstructedFrom(typeSymbol, MetadataNames.System_Collections_Immutable_ImmutableArray_T, semanticModel);
        }

        public static bool IsEventHandlerOrConstructedFromEventHandlerOfT(
            this ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return typeSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler))
                || typeSymbol.IsConstructedFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler_T));
        }

        public static bool IsException(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol.IsClass())
            {
                INamedTypeSymbol exceptionSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Exception);

                return typeSymbol
                    .BaseTypesAndSelf()
                    .Any(f => f.Equals(exceptionSymbol));
            }

            return false;
        }

        public static bool IsTaskOrDerivedFromTask(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol taskSymbol = semanticModel
                .Compilation
                .GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

            if (typeSymbol.Equals(taskSymbol))
                return true;

            return typeSymbol.IsNamedType()
                && ((INamedTypeSymbol)typeSymbol)
                    .ConstructedFrom
                    .BaseTypes()
                    .Any(baseType => baseType.Equals(taskSymbol));
        }

        public static bool IsConstructedFromTaskOfT(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol.IsNamedType())
            {
                INamedTypeSymbol constructedFrom = ((INamedTypeSymbol)typeSymbol).ConstructedFrom;

                INamedTypeSymbol taskOfT = semanticModel
                    .Compilation
                    .GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                return constructedFrom.Equals(taskOfT)
                    || constructedFrom.BaseTypes().Any(f => f.Equals(taskOfT));
            }

            return false;
        }

        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind)
        {
            return typeSymbol?.TypeKind == typeKind;
        }

        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2;
        }

        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2, TypeKind typeKind3)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2
                || typeKind == typeKind3;
        }

        public static bool IsSpecialType(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            return typeSymbol?.SpecialType == specialType;
        }
    }
}
