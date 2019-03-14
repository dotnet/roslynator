// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal sealed class MemberSymbolDefinitionComparer : IComparer<ISymbol>
    {
        internal MemberSymbolDefinitionComparer(SymbolDefinitionComparer symbolComparer)
        {
            SymbolComparer = symbolComparer;
        }

        public SymbolDefinitionComparer SymbolComparer { get; }

        public int Compare(ISymbol x, ISymbol y)
        {
            Debug.Assert(x.IsKind(SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.Property), x.Kind.ToString());
            Debug.Assert(y.IsKind(SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.Property), y.Kind.ToString());

            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff = 0;

            if ((SymbolComparer.Options & SymbolDefinitionSortOptions.OmitContainingNamespace) == 0)
            {
                diff = SymbolComparer.NamespaceComparer.Compare(x.ContainingNamespace, y.ContainingNamespace);

                if (diff != 0)
                    return diff;
            }

            if ((SymbolComparer.Options & SymbolDefinitionSortOptions.OmitContainingType) == 0)
            {
                diff = SymbolComparer.TypeComparer.Compare(x.ContainingType, y.ContainingType);

                if (diff != 0)
                    return diff;
            }

            MemberDeclarationKind kind1 = x.GetMemberDeclarationKind();
            MemberDeclarationKind kind2 = y.GetMemberDeclarationKind();

            diff = ((int)kind1).CompareTo((int)kind2);

            if (diff != 0)
                return diff;

            switch (kind1)
            {
                case MemberDeclarationKind.Constructor:
                case MemberDeclarationKind.Method:
                    {
                        return CompareMethods((IMethodSymbol)x, (IMethodSymbol)y);
                    }
                case MemberDeclarationKind.Indexer:
                case MemberDeclarationKind.Property:
                    {
                        return CompareProperties((IPropertySymbol)x, (IPropertySymbol)y);
                    }
                case MemberDeclarationKind.ExplicitlyImplementedEvent:
                    {
                        var e1 = (IEventSymbol)x;
                        var e2 = (IEventSymbol)y;

                        diff = CompareExplicitImplementations(e1.ExplicitInterfaceImplementations, e2.ExplicitInterfaceImplementations);

                        if (diff != 0)
                            return diff;

                        break;
                    }
                case MemberDeclarationKind.ExplicitlyImplementedMethod:
                    {
                        var m1 = (IMethodSymbol)x;
                        var m2 = (IMethodSymbol)y;

                        diff = CompareExplicitImplementations(m1.ExplicitInterfaceImplementations, m2.ExplicitInterfaceImplementations);

                        if (diff != 0)
                            return diff;

                        return CompareMethods(m1, m2);
                    }
                case MemberDeclarationKind.ExplicitlyImplementedProperty:
                    {
                        var p1 = (IPropertySymbol)x;
                        var p2 = (IPropertySymbol)y;

                        diff = CompareExplicitImplementations(p1.ExplicitInterfaceImplementations, p2.ExplicitInterfaceImplementations);

                        if (diff != 0)
                            return diff;

                        return CompareProperties(p1, p2);
                    }
            }

            return SymbolDefinitionComparer.CompareName(x, y);
        }

        private static int CompareMethods(IMethodSymbol methodSymbol1, IMethodSymbol methodSymbol2)
        {
            int diff = SymbolDefinitionComparer.CompareName(methodSymbol1, methodSymbol2);

            if (diff != 0)
                return diff;

            diff = methodSymbol1.TypeParameters.Length.CompareTo(methodSymbol2.TypeParameters.Length);

            if (diff != 0)
                return diff;

            return CompareParameters(methodSymbol1.Parameters, methodSymbol2.Parameters);
        }

        private static int CompareProperties(IPropertySymbol propertySymbol1, IPropertySymbol propertySymbol2)
        {
            int diff = SymbolDefinitionComparer.CompareName(propertySymbol1, propertySymbol2);

            if (diff != 0)
                return diff;

            return CompareParameters(propertySymbol1.Parameters, propertySymbol2.Parameters);
        }

        private int CompareExplicitImplementations<TSymbol>(ImmutableArray<TSymbol> x, ImmutableArray<TSymbol> y) where TSymbol : ISymbol
        {
            return SymbolComparer.TypeComparer.Compare(x[0].ContainingType, y[0].ContainingType);
        }

        private static int CompareParameters(ImmutableArray<IParameterSymbol> parameters1, ImmutableArray<IParameterSymbol> parameters2)
        {
            int length = parameters1.Length;

            int diff = length.CompareTo(parameters2.Length);

            if (diff != 0)
                return diff;

            for (int i = 0; i < length; i++)
            {
                diff = CompareParameter(parameters1[i], parameters2[i]);

                if (diff != 0)
                    return diff;
            }

            return 0;
        }

        private static int CompareParameter(IParameterSymbol x, IParameterSymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff = ((int)x.RefKind).CompareTo((int)y.RefKind);

            if (diff != 0)
                return diff;

            return SymbolDefinitionComparer.CompareName(x, y);
        }
    }
}
