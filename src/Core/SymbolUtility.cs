// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
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
            if (methodSymbol?.ReturnsVoid == true)
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                if (parameters.Length == 2
                    && parameters[0].Type.SpecialType == SpecialType.System_Object)
                {
                    ITypeSymbol type = parameters[1].Type;

                    if (type.Kind == SymbolKind.TypeParameter)
                        return type.Name.EndsWith("EventArgs", StringComparison.Ordinal);

                    return type.EqualsOrInheritsFrom(eventArgsSymbol);
                }
            }

            return false;
        }

        public static bool IsEventHandlerMethod(IMethodSymbol methodSymbol)
        {
            if (methodSymbol?.ReturnsVoid == true)
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                if (parameters.Length == 2
                    && parameters[0].Type.SpecialType == SpecialType.System_Object)
                {
                    ITypeSymbol type = parameters[1].Type;

                    if (type.Kind == SymbolKind.TypeParameter)
                        return type.Name.EndsWith("EventArgs", StringComparison.Ordinal);

                    return type.EqualsOrInheritsFrom(MetadataNames.System_EventArgs);
                }
            }

            return false;
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

            ITypeSymbol originalDefinition = typeSymbol.OriginalDefinition;

            if (!typeSymbol.Equals(originalDefinition))
            {
                hasIndexer = HasIndexer(originalDefinition.SpecialType);

                if (hasIndexer != null)
                    return hasIndexer.Value;
            }

            if (originalDefinition.ImplementsAny(
                SpecialType.System_Collections_Generic_IList_T,
                SpecialType.System_Collections_Generic_IReadOnlyList_T,
                allInterfaces: true))
            {
                if (originalDefinition.TypeKind == TypeKind.Interface)
                    return true;

                foreach (ISymbol symbol in typeSymbol.GetMembers("this[]"))
                {
                    if (semanticModel.IsAccessible(position, symbol))
                        return true;
                }
            }

            return false;

            bool? HasIndexer(SpecialType specialType)
            {
                switch (specialType)
                {
                    case SpecialType.System_String:
                    case SpecialType.System_Array:
                    case SpecialType.System_Collections_Generic_IList_T:
                    case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                        return true;
                    case SpecialType.None:
                        return null;
                }

                return false;
            }
        }

        public static string GetCountOrLengthPropertyName(
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            int position)
        {
            SymbolKind kind = typeSymbol.Kind;

            if (kind == SymbolKind.ErrorType)
                return null;

            if (kind == SymbolKind.ArrayType)
                return "Length";

            string propertyName = GetCountOrLengthPropertyName(typeSymbol.SpecialType);

            if (propertyName != null)
                return (propertyName.Length > 0) ? propertyName : null;

            ITypeSymbol originalDefinition = typeSymbol.OriginalDefinition;

            if (!typeSymbol.Equals(originalDefinition))
            {
                propertyName = GetCountOrLengthPropertyName(originalDefinition.SpecialType);

                if (propertyName != null)
                    return (propertyName.Length > 0) ? propertyName : null;
            }

            if (originalDefinition.ImplementsAny(
                SpecialType.System_Collections_Generic_ICollection_T,
                SpecialType.System_Collections_Generic_IReadOnlyCollection_T,
                allInterfaces: true))
            {
                if (originalDefinition.TypeKind == TypeKind.Interface)
                    return "Count";

                foreach (ISymbol symbol in typeSymbol.GetMembers())
                {
                    if (symbol.Kind == SymbolKind.Property
                        && StringUtility.Equals(symbol.Name, "Count", "Length")
                        && semanticModel.IsAccessible(position, symbol))
                    {
                        return symbol.Name;
                    }
                }
            }

            return null;

            string GetCountOrLengthPropertyName(SpecialType specialType)
            {
                switch (specialType)
                {
                    case SpecialType.System_String:
                    case SpecialType.System_Array:
                        return "Length";
                    case SpecialType.System_Collections_Generic_IList_T:
                    case SpecialType.System_Collections_Generic_ICollection_T:
                    case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                    case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                        return "Count";
                    case SpecialType.None:
                        return null;
                }

                return "";
            }
        }

        public static bool IsFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2)
        {
            if (!symbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Func_T2))
                return false;

            ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)symbol).TypeArguments;

            return typeArguments.Length == 2
                && typeArguments[0].Equals(parameter1)
                && typeArguments[1].Equals(parameter2);
        }

        public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter)
        {
            if (!symbol.HasMetadataName(MetadataNames.System_Func_T2))
                return false;

            ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)symbol).TypeArguments;

            return typeArguments.Length == 2
                && typeArguments[0].Equals(parameter)
                && typeArguments[1].SpecialType == SpecialType.System_Boolean;
        }

        public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2)
        {
            if (!symbol.HasMetadataName(MetadataNames.System_Func_T3))
                return false;

            ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)symbol).TypeArguments;

            return typeArguments.Length == 3
                && typeArguments[0].Equals(parameter1)
                && typeArguments[1].Equals(parameter2)
                && typeArguments[2].SpecialType == SpecialType.System_Boolean;
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
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT(methodSymbol, name, parameterCount: 1, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        internal static bool IsLinqElementAt(
            IMethodSymbol methodSymbol,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT(methodSymbol, "ElementAt", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension)
                && methodSymbol.Parameters[1].Type.SpecialType == SpecialType.System_Int32;
        }

        internal static bool IsLinqWhere(
            IMethodSymbol methodSymbol,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, "Where", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        internal static bool IsLinqWhereWithIndex(IMethodSymbol methodSymbol)
        {
            if (!IsLinqExtensionOfIEnumerableOfT(methodSymbol, "Where", parameterCount: 2, allowImmutableArrayExtension: false))
                return false;

            ITypeSymbol typeSymbol = methodSymbol.Parameters[1].Type;

            if (!typeSymbol.HasMetadataName(MetadataNames.System_Func_T3))
                return false;

            ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)typeSymbol).TypeArguments;

            return typeArguments.Length == 3
                && typeArguments[0].Equals(methodSymbol.TypeArguments[0])
                && typeArguments[1].SpecialType == SpecialType.System_Int32
                && typeArguments[2].SpecialType == SpecialType.System_Boolean;
        }

        internal static bool IsLinqSelect(IMethodSymbol methodSymbol, bool allowImmutableArrayExtension = false)
        {
            if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                return false;

            if (methodSymbol.ReturnType.OriginalDefinition.SpecialType != SpecialType.System_Collections_Generic_IEnumerable_T)
                return false;

            if (!methodSymbol.IsName("Select"))
                return false;

            if (methodSymbol.Arity != 2)
                return false;

            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType == null)
                return false;

            if (containingType.HasMetadataName(MetadataNames.System_Linq_Enumerable))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == 2
                    && parameters[0].Type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                    && IsFunc(parameters[1].Type, methodSymbol.TypeArguments[0], methodSymbol.TypeArguments[1]);
            }
            else if (allowImmutableArrayExtension
                && containingType.HasMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == 2
                    && IsImmutableArrayOfT(parameters[0].Type)
                    && IsFunc(parameters[1].Type, methodSymbol.TypeArguments[0], methodSymbol.TypeArguments[1]);
            }

            return false;
        }

        internal static bool IsLinqCast(IMethodSymbol methodSymbol)
        {
            return methodSymbol.DeclaredAccessibility == Accessibility.Public
                && methodSymbol.ReturnType.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                && methodSymbol.IsName("Cast")
                && methodSymbol.Arity == 1
                && methodSymbol.HasSingleParameter(SpecialType.System_Collections_IEnumerable)
                && methodSymbol.ContainingType?.HasMetadataName(MetadataNames.System_Linq_Enumerable) == true;
        }

        internal static bool IsLinqOfType(IMethodSymbol methodSymbol)
        {
            return methodSymbol.DeclaredAccessibility == Accessibility.Public
                && methodSymbol.ReturnType.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                && methodSymbol.IsName("OfType")
                && methodSymbol.Arity == 1
                && methodSymbol.HasSingleParameter(SpecialType.System_Collections_IEnumerable)
                && methodSymbol.ContainingType?.HasMetadataName(MetadataNames.System_Linq_Enumerable) == true;
        }

        internal static bool IsLinqExtensionOfIEnumerableOfT(
            IMethodSymbol methodSymbol,
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

            if (containingType.HasMetadataName(MetadataNames.System_Linq_Enumerable))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return (parameterCount == -1 || parameters.Length == parameterCount)
                    && parameters[0].Type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
            }
            else if (allowImmutableArrayExtension
                && containingType.HasMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return (parameterCount == -1 || parameters.Length == parameterCount)
                    && IsImmutableArrayOfT(parameters[0].Type);
            }

            return false;
        }

        internal static bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            IMethodSymbol methodSymbol,
            string name = null,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, name, parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        private static bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            IMethodSymbol methodSymbol,
            string name,
            int parameterCount,
            bool allowImmutableArrayExtension = false)
        {
            if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                return false;

            if (!StringUtility.IsNullOrEquals(name, methodSymbol.Name))
                return false;

            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType == null)
                return false;

            if (containingType.HasMetadataName(MetadataNames.System_Linq_Enumerable))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == parameterCount
                    && parameters[0].Type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                    && IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0]);
            }
            else if (allowImmutableArrayExtension
                && containingType.HasMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == parameterCount
                    && IsImmutableArrayOfT(parameters[0].Type)
                    && IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0]);
            }

            return false;
        }

        public static bool IsImmutableArrayOfT(ITypeSymbol typeSymbol)
        {
            return typeSymbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Collections_Immutable_ImmutableArray_T);
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

        // https://docs.microsoft.com/cs-cz/dotnet/csharp/programming-guide/main-and-command-args/
        public static bool CanBeEntryPoint(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.IsStatic
                && string.Equals(methodSymbol.Name, "Main", StringComparison.Ordinal)
                && methodSymbol.ContainingType?.TypeKind.Is(TypeKind.Class, TypeKind.Struct) == true
                && !methodSymbol.TypeParameters.Any())
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                int length = parameters.Length;

                if (length == 0)
                    return true;

                if (length == 1)
                {
                    IParameterSymbol parameter = parameters[0];

                    ITypeSymbol type = parameter.Type;

                    if (type.Kind == SymbolKind.ArrayType)
                    {
                        var arrayType = (IArrayTypeSymbol)type;

                        if (arrayType.ElementType.SpecialType == SpecialType.System_String)
                            return true;
                    }
                }
            }

            return false;
        }

        public static ulong GetEnumValueAsUInt64(object value, INamedTypeSymbol enumType)
        {
            switch (enumType.EnumUnderlyingType.SpecialType)
            {
                case SpecialType.System_SByte:
                    return (ulong)(sbyte)value;
                case SpecialType.System_Byte:
                    return (byte)value;
                case SpecialType.System_Int16:
                    return (ulong)(short)value;
                case SpecialType.System_UInt16:
                    return (ushort)value;
                case SpecialType.System_Int32:
                    return (ulong)(int)value;
                case SpecialType.System_UInt32:
                    return (uint)value;
                case SpecialType.System_Int64:
                    return (ulong)(long)value;
                case SpecialType.System_UInt64:
                    return (ulong)value;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static IMethodSymbol FindMethodThatRaisePropertyChanged(INamedTypeSymbol typeSymbol, int position, SemanticModel semanticModel)
        {
            do
            {
                IMethodSymbol methodSymbol = FindMethod(typeSymbol.GetMembers("RaisePropertyChanged"))
                    ?? FindMethod(typeSymbol.GetMembers("OnPropertyChanged"));

                if (methodSymbol != null)
                    return methodSymbol;

                typeSymbol = typeSymbol.BaseType;
            }
            while (typeSymbol != null
                && typeSymbol.SpecialType != SpecialType.System_Object);

            return null;

            IMethodSymbol FindMethod(ImmutableArray<ISymbol> symbols)
            {
                foreach (ISymbol symbol in symbols)
                {
                    if (symbol.Kind == SymbolKind.Method)
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        if (methodSymbol.Parameters.SingleOrDefault(shouldThrow: false)?.Type.SpecialType == SpecialType.System_String
                            && semanticModel.IsAccessible(position, methodSymbol))
                        {
                            return methodSymbol;
                        }
                    }
                }

                return null;
            }
        }

        public static bool IsAwaitable(ITypeSymbol typeSymbol, bool shouldCheckWindowsRuntimeTypes = false)
        {
            if (typeSymbol.Kind == SymbolKind.TypeParameter)
            {
                var typeParameterSymbol = (ITypeParameterSymbol)typeSymbol;

                typeSymbol = typeParameterSymbol.ConstraintTypes.SingleOrDefault(f => f.TypeKind == TypeKind.Class, shouldThrow: false);

                if (typeSymbol == null)
                    return false;
            }

            if (typeSymbol.IsTupleType)
                return false;

            if (typeSymbol.SpecialType != SpecialType.None)
                return false;

            if (!typeSymbol.TypeKind.Is(TypeKind.Class, TypeKind.Struct, TypeKind.Interface))
                return false;

            if (!(typeSymbol is INamedTypeSymbol namedTypeSymbol))
                return false;

            INamedTypeSymbol originalDefinition = namedTypeSymbol.OriginalDefinition;

            if (originalDefinition.HasMetadataName(MetadataNames.System_Threading_Tasks_ValueTask_T))
                return true;

            if (namedTypeSymbol.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_Task))
                return true;

            if (shouldCheckWindowsRuntimeTypes)
            {
                if (namedTypeSymbol.HasMetadataName(MetadataNames.WinRT.Windows_Foundation_IAsyncAction))
                    return true;

                if (namedTypeSymbol.Implements(MetadataNames.WinRT.Windows_Foundation_IAsyncAction, allInterfaces: true))
                    return true;

                if (namedTypeSymbol.Arity > 0
                    && namedTypeSymbol.TypeKind == TypeKind.Interface)
                {
                    if (originalDefinition.HasMetadataName(MetadataNames.WinRT.Windows_Foundation_IAsyncActionWithProgress_1))
                        return true;

                    if (originalDefinition.HasMetadataName(MetadataNames.WinRT.Windows_Foundation_IAsyncOperation_1))
                        return true;

                    if (originalDefinition.HasMetadataName(MetadataNames.WinRT.Windows_Foundation_IAsyncOperationWithProgress_2))
                        return true;
                }
            }

            return false;
        }
    }
}
