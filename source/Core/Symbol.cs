// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.Extensions;

namespace Roslynator
{
    public static class Symbol
    {
#if DEBUG
        public static bool IsMethod(
            IMethodSymbol methodSymbol,
            INamedTypeSymbol containingType,
            Accessibility accessibility,
            bool isStatic,
            ITypeSymbol returnType,
            string name,
            int arity)
        {
            return methodSymbol != null
                && string.Equals(methodSymbol.Name, name, StringComparison.Ordinal)
                && methodSymbol.ContainingType?.Equals(containingType) == true
                && methodSymbol.DeclaredAccessibility == accessibility
                && methodSymbol.IsStatic == isStatic
                && methodSymbol.ReturnType.Equals(returnType)
                && methodSymbol.Arity == arity;
        }

        public static bool IsProperty(
            IPropertySymbol propertySymbol,
            INamedTypeSymbol containingType,
            Accessibility accessibility,
            bool isStatic,
            ITypeSymbol type,
            string name,
            bool isReadOnly)
        {
            return propertySymbol != null
                && string.Equals(propertySymbol.Name, name, StringComparison.Ordinal)
                && !propertySymbol.IsIndexer
                && propertySymbol.ContainingType?.Equals(containingType) == true
                && propertySymbol.DeclaredAccessibility == accessibility
                && propertySymbol.IsStatic == isStatic
                && propertySymbol.Type.Equals(type)
                && propertySymbol.IsReadOnly == isReadOnly;
        }

        public static bool IsIndexer(
            IPropertySymbol propertySymbol,
            INamedTypeSymbol containingType,
            Accessibility accessibility,
            bool isStatic,
            ITypeSymbol type,
            string name,
            bool isReadOnly)
        {
            return propertySymbol != null
                && string.Equals(propertySymbol.Name, name, StringComparison.Ordinal)
                && propertySymbol.IsIndexer
                && propertySymbol.ContainingType?.Equals(containingType) == true
                && propertySymbol.DeclaredAccessibility == accessibility
                && propertySymbol.IsStatic == isStatic
                && propertySymbol.Type.Equals(type)
                && propertySymbol.IsReadOnly == isReadOnly;
        }

        public static bool IsField(
            IFieldSymbol fieldSymbol,
            INamedTypeSymbol containingType,
            Accessibility accessibility,
            bool isStatic,
            bool isReadOnly,
            ITypeSymbol type,
            string name)
        {
            return fieldSymbol != null
                && string.Equals(fieldSymbol.Name, name, StringComparison.Ordinal)
                && fieldSymbol.ContainingType?.Equals(containingType) == true
                && fieldSymbol.DeclaredAccessibility == accessibility
                && fieldSymbol.IsStatic == isStatic
                && fieldSymbol.IsReadOnly == isReadOnly
                && fieldSymbol.Type.Equals(type);
        }

        public static bool IsConst(
            IFieldSymbol fieldSymbol,
            INamedTypeSymbol containingType,
            Accessibility accessibility,
            ITypeSymbol type,
            string name)
        {
            return fieldSymbol != null
                && fieldSymbol.IsConst
                && string.Equals(fieldSymbol.Name, name, StringComparison.Ordinal)
                && fieldSymbol.ContainingType?.Equals(containingType) == true
                && fieldSymbol.DeclaredAccessibility == accessibility
                && fieldSymbol.IsStatic
                && !fieldSymbol.IsReadOnly
                && fieldSymbol.Type.Equals(type);
        }
#endif

        public static bool IsEnumWithFlagsAttribute(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return typeSymbol.IsEnum()
                && typeSymbol
                    .GetAttributes()
                    .Any(f => f.AttributeClass.Equals(semanticModel.Compilation.GetTypeByMetadataName(MetadataNames.System_FlagsAttribute)));
        }

        public static IMethodSymbol FindGetItemMethodWithInt32Parameter(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            foreach (ISymbol symbol in typeSymbol.GetMembers("get_Item"))
            {
                if (!symbol.IsStatic
                    && symbol.IsMethod())
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.SingleParameterOrDefault()?.Type.IsInt32() == true)
                        return methodSymbol;
                }
            }

            return null;
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

        public static bool IsException(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return typeSymbol.IsClass()
                && typeSymbol.EqualsOrInheritsFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Exception));
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

        public static bool ImplementsINotifyPropertyChanged(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol != null)
            {
                INamedTypeSymbol notifyPropertyChanged = semanticModel.GetTypeByMetadataName(MetadataNames.System_ComponentModel_INotifyPropertyChanged);

                return notifyPropertyChanged != null
                    && typeSymbol.AllInterfaces.Contains(notifyPropertyChanged);
            }

            return false;
        }

        public static bool ImplementsICollectionOfT(ITypeSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol
                .AllInterfaces
                .Any(f => f.IsConstructedFrom(SpecialType.System_Collections_Generic_ICollection_T));
        }

        private static bool IsContainingType(ISymbol symbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            return symbol
                .ContainingType?
                .Equals(semanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName)) == true;
        }
    }
}
