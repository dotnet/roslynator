// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Rename;

namespace Pihrtsoft.CodeAnalysis
{
    public static class ISymbolExtensions
    {
        public static async Task<Solution> RenameAsync(
            this ISymbol symbol,
            string newName,
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Solution solution = document.Project.Solution;

            return await Renamer.RenameSymbolAsync(
                solution,
                symbol,
                newName,
                solution.Workspace.Options,
                cancellationToken);
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

        public static bool IsGenericIEnumerable(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol?.IsNamedType() == true
                && ((INamedTypeSymbol)symbol).ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
        }

        public static bool IsGenericImmutableArray(this ISymbol symbol, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol?.IsNamedType() == true)
            {
                INamedTypeSymbol namedTypeSymbol = semanticModel
                    .Compilation
                    .GetTypeByMetadataName("System.Collections.Immutable.ImmutableArray`1");

                return namedTypeSymbol != null
                    && ((INamedTypeSymbol)symbol).ConstructedFrom.Equals(namedTypeSymbol);
            }

            return false;
        }

        public static bool IsInt32(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.IsNamedType()
                && ((INamedTypeSymbol)symbol).SpecialType == SpecialType.System_Int32;
        }

        [DebuggerStepThrough]
        public static bool IsErrorType(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.ErrorType;
        }

        [DebuggerStepThrough]
        public static bool IsNamedType(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.NamedType;
        }

        [DebuggerStepThrough]
        public static bool IsArrayType(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.ArrayType;
        }

        [DebuggerStepThrough]
        public static bool IsMethod(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.Method;
        }

        [DebuggerStepThrough]
        public static bool IsAsyncMethod(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.Method
                && ((IMethodSymbol)symbol).IsAsync;
        }

        [DebuggerStepThrough]
        public static bool IsProperty(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.Property;
        }

        [DebuggerStepThrough]
        public static bool IsField(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.Field;
        }

        [DebuggerStepThrough]
        public static bool IsTypeParameter(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.TypeParameter;
        }

        [DebuggerStepThrough]
        public static bool IsLocal(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.Kind == SymbolKind.Local;
        }

        [DebuggerStepThrough]
        public static bool IsPublic(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.DeclaredAccessibility == Accessibility.Public;
        }

        [DebuggerStepThrough]
        public static bool IsInternal(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.DeclaredAccessibility == Accessibility.Internal;
        }

        [DebuggerStepThrough]
        public static bool IsProtected(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.DeclaredAccessibility == Accessibility.Protected;
        }

        [DebuggerStepThrough]
        public static bool IsPrivate(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return symbol.DeclaredAccessibility == Accessibility.Private;
        }
    }
}
