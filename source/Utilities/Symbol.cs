// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class Symbol
    {
        public static bool IsPublicProperty(ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsProperty();
        }

        public static bool IsPublicInstanceProperty(ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && !symbol.IsStatic
                && symbol.IsProperty();
        }

        public static bool IsPublicStaticProperty(ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsStatic
                && symbol.IsProperty();
        }

        public static bool IsPrivateField(ISymbol symbol)
        {
            return symbol?.IsPrivate() == true
                && symbol.IsField();
        }

        public static bool IsInstanceField(ISymbol symbol)
        {
            return symbol?.IsStatic == false
                && symbol.IsField();
        }

        public static bool IsStaticField(ISymbol symbol)
        {
            return symbol?.IsStatic == true
                && symbol.IsField();
        }

        public static bool IsEnumField(ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Field
                && symbol.ContainingType?.TypeKind == TypeKind.Enum;
        }

        public static bool IsStaticClass(ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsStatic == true
                && typeSymbol.IsClass();
        }

        public static bool IsInstanceClass(ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsStatic == false
                && typeSymbol.IsClass();
        }

        public static bool IsSealedClass(ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsSealed == true
                && typeSymbol.IsClass();
        }

        public static bool IsVoidMethod(ISymbol typeSymbol)
        {
            return typeSymbol?.IsMethod() == true
                && ((IMethodSymbol)typeSymbol).ReturnsVoid;
        }

        public static bool IsAsyncMethod(ISymbol symbol)
        {
            return symbol?.IsMethod() == true
                && ((IMethodSymbol)symbol).IsAsync;
        }

        public static bool IsInstanceMethod(ISymbol symbol)
        {
            return symbol?.IsStatic == false
                && symbol.IsMethod();
        }

        public static bool IsStaticMethod(ISymbol symbol)
        {
            return symbol?.IsStatic == true
                && symbol.IsMethod();
        }

        public static bool IsPublicInstanceMethod(ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && !symbol.IsStatic
                && symbol.IsMethod();
        }

        public static bool IsPublicMethod(ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsMethod();
        }

        public static bool IsPublicStaticMethod(ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsStatic
                && symbol.IsMethod();
        }

        public static bool SupportsExplicitDeclaration(ITypeSymbol typeSymbol)
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
                        return !IsAnyTypeArgumentAnonymousType((INamedTypeSymbol)typeSymbol);
                }
            }

            return false;
        }

        private static bool IsAnyTypeArgumentAnonymousType(INamedTypeSymbol namedTypeSymbol)
        {
            ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

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

        public static bool SupportsPredefinedType(ITypeSymbol typeSymbol)
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

        public static bool SupportsConstantValue(ITypeSymbol typeSymbol)
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

        public static bool SupportsPrefixOrPostfixUnaryOperator(ITypeSymbol typeSymbol)
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

            return typeSymbol.IsEnum();
        }

        public static bool IsTaskOrDerivedFromTask(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol taskSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

            return typeSymbol.EqualsOrDerivedFrom(taskSymbol);
        }

        public static bool IsConstructedFromTaskOfT(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol.IsNamedType())
            {
                INamedTypeSymbol taskOfT = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                return ((INamedTypeSymbol)typeSymbol).ConstructedFrom.EqualsOrDerivedFrom(taskOfT);
            }

            return false;
        }

        public static bool IsEventHandlerOrConstructedFromEventHandlerOfT(
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return typeSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler))
                || typeSymbol.IsConstructedFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler_T));
        }

        public static bool IsConstructedFromImmutableArrayOfT(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            return IsConstructedFrom(typeSymbol, MetadataNames.System_Collections_Immutable_ImmutableArray_T, semanticModel);
        }

        public static bool IsConstructedFromImmutableArrayOfT(INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            return IsConstructedFrom(namedTypeSymbol, MetadataNames.System_Collections_Immutable_ImmutableArray_T, semanticModel);
        }

        public static bool IsConstructedFrom(ITypeSymbol typeSymbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol.IsNamedType())
            {
                INamedTypeSymbol symbol = semanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName);

                return symbol != null
                    && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.Equals(symbol);
            }

            return false;
        }

        public static bool IsConstructedFrom(INamedTypeSymbol namedTypeSymbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol symbol = semanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName);

            return symbol != null
                && namedTypeSymbol.ConstructedFrom.Equals(symbol);
        }

        public static bool HasPublicIndexerWithInt32Parameter(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            foreach (ISymbol symbol in typeSymbol.GetMembers("get_Item"))
            {
                if (IsPublicInstanceMethod(symbol))
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.SingleParameterOrDefault()?.Type.IsInt32() == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsException(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return typeSymbol.IsClass()
                && typeSymbol.EqualsOrDerivedFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Exception));
        }

        public static bool IsEnumerableMethod(
            IMethodSymbol methodSymbol,
            string methodName,
            SemanticModel semanticModel,
            Func<IMethodSymbol, bool> validate = null)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, methodName)
                && IsEnumerableMethod(methodSymbol, semanticModel)
                && validate?.Invoke(methodSymbol) != false;
        }

        public static bool IsImmutableArrayExtensionMethod(
            IMethodSymbol methodSymbol,
            string methodName,
            SemanticModel semanticModel,
            Func<IMethodSymbol, bool> validate = null)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, methodName)
                && IsImmutableArrayExtensionMethod(methodSymbol, semanticModel)
                && validate?.Invoke(methodSymbol) != false;
        }

        public static bool IsEnumerableOrImmutableArrayExtensionMethod(
            IMethodSymbol methodSymbol,
            string methodName,
            SemanticModel semanticModel,
            Func<IMethodSymbol, bool> validate = null)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, methodName)
                && (IsEnumerableMethod(methodSymbol, semanticModel) || IsImmutableArrayExtensionMethod(methodSymbol, semanticModel))
                && validate?.Invoke(methodSymbol) != false;
        }

        private static bool IsEnumerableMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (IsContainedInEnumerable(methodSymbol, semanticModel))
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                IParameterSymbol parameter = (reducedFrom != null)
                    ? reducedFrom.Parameters.First()
                    : methodSymbol.Parameters.First();

                return parameter.Type.IsConstructedFromIEnumerableOfT();
            }
            else
            {
                return false;
            }
        }

        private static bool IsImmutableArrayExtensionMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (IsContainedInImmutableArrayExtensions(methodSymbol, semanticModel))
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                IParameterSymbol parameter = (reducedFrom != null)
                    ? reducedFrom.Parameters.First()
                    : methodSymbol.Parameters.First();

                return IsConstructedFromImmutableArrayOfT(parameter.Type, semanticModel);
            }
            else
            {
                return false;
            }
        }

        public static bool IsEnumerableMethodWithoutParameters(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f => !f.Parameters.Any());
        }

        public static bool IsEnumerableOrImmutableArrayExtensionMethodWithoutParameters(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f => !f.Parameters.Any());
        }

        public static bool IsEnumerableMethodWithPredicate(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f =>
                {
                    IParameterSymbol parameter = f.SingleParameterOrDefault();

                    return parameter != null
                        && IsPredicateFunc(parameter.Type, f.TypeArguments[0], semanticModel);
                });
        }

        public static bool IsEnumerableMethodWithPredicateWithIndex(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f =>
                {
                    IParameterSymbol parameter = f.SingleParameterOrDefault();

                    return parameter != null
                        && IsPredicateFunc(parameter.Type, f.TypeArguments[0], semanticModel.Compilation.GetSpecialType(SpecialType.System_Int32), semanticModel);
                });
        }

        public static bool IsEnumerableCastMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, "Cast")
                && IsContainedInEnumerable(methodSymbol, semanticModel))
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null)
                    methodSymbol = reducedFrom;

                return methodSymbol.SingleParameterOrDefault()?.Type.IsIEnumerable() == true;
            }

            return false;
        }

        public static bool IsEnumerableWhereMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableMethodWithPredicate(methodSymbol, "Where", semanticModel);
        }

        public static bool IsEnumerableOrImmutableArrayExtensionWhereMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                "Where",
                semanticModel,
                f =>
                {
                    IParameterSymbol parameter = f.SingleParameterOrDefault();

                    return parameter != null
                        && IsPredicateFunc(parameter.Type, f.TypeArguments[0], semanticModel);
                });
        }

        public static bool IsEnumerableOrImmutableArrayExtensionSelectMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                "Select",
                semanticModel,
                f =>
                {
                    IParameterSymbol parameter = f.SingleParameterOrDefault();

                    return parameter != null
                        && IsFunc(parameter.Type, f.TypeArguments[0], f.TypeArguments[1], semanticModel);
                });
        }

        public static bool IsEnumerableOrImmutableArrayExtensionElementAtMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                "ElementAt",
                semanticModel,
                f => f.SingleParameterOrDefault()?.Type.IsInt32() == true);
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

            if (symbol.IsNamedType())
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

            if (symbol.IsNamedType())
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

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T2);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 2
                        && typeArguments[0].Equals(parameter)
                        && typeArguments[1].IsBoolean();
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

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T3);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 3
                        && typeArguments[0].Equals(parameter1)
                        && typeArguments[1].Equals(parameter2)
                        && typeArguments[2].IsBoolean();
                }
            }

            return false;
        }

        public static bool IsContainedInEnumerable(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsContainedIn(methodSymbol, MetadataNames.System_Linq_Enumerable, semanticModel);
        }

        public static bool IsContainedInImmutableArrayExtensions(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsContainedIn(methodSymbol, MetadataNames.System_Linq_ImmutableArrayExtensions, semanticModel);
        }

        private static bool IsContainedIn(IMethodSymbol methodSymbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol
                .ContainingType?
                .Equals(semanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName)) == true;
        }

        public static ImmutableArray<IParameterSymbol> GetMethodOrPropertyParameters(this ISymbol symbol)
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

        private static bool NameEquals(string name1, string name2)
        {
            return string.Equals(name1, name2, StringComparison.Ordinal);
        }
    }
}
