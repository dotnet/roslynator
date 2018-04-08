// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SymbolUtility
    {
        public static bool IsPublicStaticReadOnly(IFieldSymbol fieldSymbol, string name = null)
        {
            return fieldSymbol?.DeclaredAccessibility == Accessibility.Public
                && fieldSymbol.IsStatic
                && fieldSymbol.IsReadOnly
                && StringUtility.IsNullOrEquals(name, fieldSymbol.Name);
        }

        public static bool IsPublicStaticNonGeneric(IMethodSymbol methodSymbol, string name = null)
        {
            return methodSymbol?.DeclaredAccessibility == Accessibility.Public
                && methodSymbol.IsStatic
                && !methodSymbol.IsGenericMethod
                && StringUtility.IsNullOrEquals(name, methodSymbol.Name);
        }

        public static bool IsPublicInstanceNonGeneric(IMethodSymbol methodSymbol, string name = null)
        {
            return methodSymbol?.DeclaredAccessibility == Accessibility.Public
                && !methodSymbol.IsStatic
                && !methodSymbol.IsGenericMethod
                && StringUtility.IsNullOrEquals(name, methodSymbol.Name);
        }

        public static bool IsPublicInstance(IPropertySymbol propertySymbol, string name = null)
        {
            return propertySymbol?.DeclaredAccessibility == Accessibility.Public
                && !propertySymbol.IsStatic
                && StringUtility.IsNullOrEquals(name, propertySymbol.Name);
        }

        public static bool IsStringAdditionOperator(IMethodSymbol methodSymbol)
        {
            return methodSymbol?.MethodKind == MethodKind.BuiltinOperator
                && methodSymbol.Name == WellKnownMemberNames.AdditionOperatorName
                && methodSymbol.IsContainingType(SpecialType.System_String);
        }

        public static bool IsEventHandlerMethod(IMethodSymbol methodSymbol, INamedTypeSymbol eventArgsSymbol)
        {
            if (methodSymbol == null)
                return false;

            if (!methodSymbol.ReturnsVoid)
                return false;

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return parameters.Length == 2
                && parameters[0].Type.IsObject()
                && parameters[1].Type.EqualsOrInheritsFrom(eventArgsSymbol);
        }

        public static bool HasAccessibleIndexer(
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            int position)
        {
            if (typeSymbol == null)
                return false;

            SymbolKind symbolKind = typeSymbol.Kind;

            if (symbolKind == SymbolKind.ErrorType)
                return false;

            if (symbolKind == SymbolKind.ArrayType)
                return true;

            bool? hasIndexer = HasIndexer(typeSymbol.SpecialType);

            if (hasIndexer != null)
                return hasIndexer.Value;

            if (symbolKind == SymbolKind.NamedType)
            {
                hasIndexer = HasIndexer(((INamedTypeSymbol)typeSymbol).ConstructedFrom.SpecialType);

                if (hasIndexer != null)
                    return hasIndexer.Value;
            }

            if (typeSymbol.ImplementsAny(
                SpecialType.System_Collections_Generic_IList_T,
                SpecialType.System_Collections_Generic_IReadOnlyList_T,
                allInterfaces: true))
            {
                if (typeSymbol.TypeKind == TypeKind.Interface)
                    return true;

                foreach (ISymbol symbol in typeSymbol.GetMembers("this[]"))
                {
                    if (semanticModel.IsAccessible(position, symbol))
                        return true;
                }
            }

            return false;
        }

        private static bool? HasIndexer(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.None:
                    return null;
                case SpecialType.System_String:
                case SpecialType.System_Array:
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                    return true;
            }

            return false;
        }

        public static bool IsFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter1 == null)
                throw new ArgumentNullException(nameof(parameter1));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.Kind == SymbolKind.NamedType)
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T2);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 2
                        && typeArguments[0].Equals(parameter1)
                        && typeArguments[1].Equals(parameter2);
                }
            }

            return false;
        }

        public static bool IsFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2, ITypeSymbol parameter3, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter1 == null)
                throw new ArgumentNullException(nameof(parameter1));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.Kind == SymbolKind.NamedType)
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T3);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 3
                        && typeArguments[0].Equals(parameter1)
                        && typeArguments[1].Equals(parameter2)
                        && typeArguments[2].Equals(parameter3);
                }
            }

            return false;
        }

        public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.Kind == SymbolKind.NamedType)
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T2);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 2
                        && typeArguments[0].Equals(parameter)
                        && typeArguments[1].SpecialType == SpecialType.System_Boolean;
                }
            }

            return false;
        }

        public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter1 == null)
                throw new ArgumentNullException(nameof(parameter1));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.Kind == SymbolKind.NamedType)
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T3);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 3
                        && typeArguments[0].Equals(parameter1)
                        && typeArguments[1].Equals(parameter2)
                        && typeArguments[2].SpecialType == SpecialType.System_Boolean;
                }
            }

            return false;
        }

        internal static bool IsPropertyOfNullableOfT(ISymbol symbol, string name)
        {
            return symbol?.Kind == SymbolKind.Property
                && symbol.ContainingType?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && string.Equals(symbol.Name, name, StringComparison.Ordinal);
        }

        internal static bool IsLinqExtensionOfIEnumerableOfTWithoutParameters(
            IMethodSymbol methodSymbol,
            string name,
            SemanticModel semanticModel,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT(methodSymbol, semanticModel, name, parameterCount: 1, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        internal static bool IsLinqElementAt(
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT(methodSymbol, semanticModel, "ElementAt", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension)
                && methodSymbol.Parameters[1].Type.SpecialType == SpecialType.System_Int32;
        }

        internal static bool IsLinqWhere(
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, "Where", parameterCount: 2, semanticModel: semanticModel, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        internal static bool IsLinqWhereWithIndex(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsLinqExtensionOfIEnumerableOfT(methodSymbol, semanticModel, "Where", parameterCount: 2, allowImmutableArrayExtension: false)
                && IsPredicateFunc(methodSymbol.Parameters[1].Type, methodSymbol.TypeArguments[0], semanticModel.Compilation.GetSpecialType(SpecialType.System_Int32), semanticModel);
        }

        internal static bool IsLinqSelect(IMethodSymbol methodSymbol, SemanticModel semanticModel, bool allowImmutableArrayExtension = false)
        {
            if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                return false;

            if (!methodSymbol.ReturnType.OriginalDefinition.IsIEnumerableOfT())
                return false;

            if (!methodSymbol.IsName("Select"))
                return false;

            if (methodSymbol.Arity != 2)
                return false;

            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType == null)
                return false;

            if (containingType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == 2
                    && parameters[0].Type.OriginalDefinition.IsIEnumerableOfT()
                    && IsFunc(parameters[1].Type, methodSymbol.TypeArguments[0], methodSymbol.TypeArguments[1], semanticModel);
            }
            else if (allowImmutableArrayExtension
                && containingType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == 2
                    && IsImmutableArrayOfT(parameters[0].Type, semanticModel)
                    && IsFunc(parameters[1].Type, methodSymbol.TypeArguments[0], methodSymbol.TypeArguments[1], semanticModel);
            }

            return false;
        }

        internal static bool IsLinqCast(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return methodSymbol.DeclaredAccessibility == Accessibility.Public
                && methodSymbol.ReturnType.OriginalDefinition.IsIEnumerableOfT()
                && methodSymbol.IsName("Cast")
                && methodSymbol.Arity == 1
                && methodSymbol.HasSingleParameter(SpecialType.System_Collections_IEnumerable)
                && methodSymbol
                    .ContainingType?
                    .Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)) == true;
        }

        internal static bool IsLinqExtensionOfIEnumerableOfT(
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            string name = null,
            int parameterCount = -1,
            bool allowImmutableArrayExtension = false)
        {
            if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                return false;

            if (!StringUtility.IsNullOrEquals(name, methodSymbol.Name))
                return false;

            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType == null)
                return false;

            if (containingType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return (parameterCount == -1 || parameters.Length == parameterCount)
                    && parameters[0].Type.OriginalDefinition.IsIEnumerableOfT();
            }
            else if (allowImmutableArrayExtension
                && containingType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return (parameterCount == -1 || parameters.Length == parameterCount)
                    && IsImmutableArrayOfT(parameters[0].Type, semanticModel);
            }

            return false;
        }

        internal static bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            string name = null,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, name, parameterCount: 2, semanticModel: semanticModel, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        private static bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            IMethodSymbol methodSymbol,
            string name,
            int parameterCount,
            SemanticModel semanticModel,
            bool allowImmutableArrayExtension = false)
        {
            if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                return false;

            if (!StringUtility.IsNullOrEquals(name, methodSymbol.Name))
                return false;

            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType == null)
                return false;

            if (containingType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == parameterCount
                    && parameters[0].Type.OriginalDefinition.IsIEnumerableOfT()
                    && IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0], semanticModel);
            }
            else if (allowImmutableArrayExtension
                && containingType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == parameterCount
                    && IsImmutableArrayOfT(parameters[0].Type, semanticModel)
                    && IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0], semanticModel);
            }

            return false;
        }

        public static bool IsImmutableArrayOfT(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            return typeSymbol?.OriginalDefinition.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Immutable_ImmutableArray_T)) == true;
        }

        public static bool SupportsSwitchExpression(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.Kind == SymbolKind.ErrorType)
                return false;

            if (typeSymbol.TypeKind == TypeKind.Enum)
                return true;

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
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
            }

            if ((typeSymbol is INamedTypeSymbol namedTypeSymbol)
                && namedTypeSymbol.IsNullableType())
            {
                switch (namedTypeSymbol.TypeArguments[0].SpecialType)
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
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                        return true;
                }
            }

            return false;
        }
    }
}
