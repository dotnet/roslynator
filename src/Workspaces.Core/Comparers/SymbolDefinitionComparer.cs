// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal abstract class SymbolDefinitionComparer : IComparer<ISymbol>
    {
        internal SymbolDefinitionComparer(SymbolDefinitionSortOptions options = SymbolDefinitionSortOptions.None)
        {
            Options = options;
        }

        public static SymbolDefinitionComparer Default { get; } = new DefaultSymbolDefinitionComparer(SymbolDefinitionSortOptions.None);

        public static SymbolDefinitionComparer SystemFirst { get; } = new DefaultSymbolDefinitionComparer(SymbolDefinitionSortOptions.SystemFirst);

        public static SymbolDefinitionComparer OmitContainingNamespace { get; } = new DefaultSymbolDefinitionComparer(SymbolDefinitionSortOptions.OmitContainingNamespace);

        public static SymbolDefinitionComparer SystemFirstOmitContainingNamespace { get; } = new DefaultSymbolDefinitionComparer(SymbolDefinitionSortOptions.SystemFirst | SymbolDefinitionSortOptions.OmitContainingNamespace);

        public SymbolDefinitionSortOptions Options { get; }

        public abstract IComparer<INamespaceSymbol> NamespaceComparer { get; }

        public abstract IComparer<INamedTypeSymbol> TypeComparer { get; }

        public abstract IComparer<ISymbol> MemberComparer { get; }

        public int Compare(ISymbol x, ISymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            switch (x.Kind)
            {
                case SymbolKind.Namespace:
                    {
                        var namespaceSymbol = (INamespaceSymbol)x;

                        switch (y.Kind)
                        {
                            case SymbolKind.Namespace:
                                return NamespaceComparer.Compare(namespaceSymbol, (INamespaceSymbol)y);
                            case SymbolKind.NamedType:
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                            case SymbolKind.Property:
                                return -CompareSymbolAndNamespaceSymbol(y, namespaceSymbol);
                        }

                        break;
                    }
                case SymbolKind.NamedType:
                    {
                        var typeSymbol = (INamedTypeSymbol)x;

                        switch (y.Kind)
                        {
                            case SymbolKind.Namespace:
                                return CompareSymbolAndNamespaceSymbol(typeSymbol, (INamespaceSymbol)y);
                            case SymbolKind.NamedType:
                                return CompareNamedTypeSymbol(typeSymbol, (INamedTypeSymbol)y);
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                            case SymbolKind.Property:
                                return -CompareSymbolAndNamedTypeSymbol(y, typeSymbol);
                        }

                        break;
                    }
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Method:
                case SymbolKind.Property:
                    {
                        switch (y.Kind)
                        {
                            case SymbolKind.Namespace:
                                return CompareSymbolAndNamespaceSymbol(x, (INamespaceSymbol)y);
                            case SymbolKind.NamedType:
                                return CompareSymbolAndNamedTypeSymbol(x, (INamedTypeSymbol)y);
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                            case SymbolKind.Property:
                                return CompareMemberSymbol(x, y);
                        }

                        break;
                    }
            }

            throw new InvalidOperationException();
        }

        private int CompareNamedTypeSymbol(INamedTypeSymbol typeSymbol1, INamedTypeSymbol typeSymbol2)
        {
            int diff = CompareContainingNamespace(typeSymbol1, typeSymbol2);

            if (diff != 0)
                return diff;

            return TypeComparer.Compare(typeSymbol1, typeSymbol2);
        }

        private int CompareMemberSymbol(ISymbol symbol1, ISymbol symbol2)
        {
            int diff = CompareNamedTypeSymbol(symbol1.ContainingType, symbol2.ContainingType);

            if (diff != 0)
                return diff;

            return MemberComparer.Compare(symbol1, symbol2);
        }

        private int CompareSymbolAndNamespaceSymbol(ISymbol symbol, INamespaceSymbol namespaceSymbol)
        {
            int diff = NamespaceComparer.Compare(symbol.ContainingNamespace, namespaceSymbol);

            if (diff != 0)
                return diff;

            return 1;
        }

        private int CompareSymbolAndNamedTypeSymbol(ISymbol symbol, INamedTypeSymbol typeSymbol)
        {
            int diff = TypeComparer.Compare(symbol.ContainingType, typeSymbol);

            if (diff != 0)
                return diff;

            return 1;
        }

        internal int CompareContainingNamespace(ISymbol x, ISymbol y)
        {
            Debug.Assert(x.Kind != SymbolKind.Namespace, x.Kind.ToString());
            Debug.Assert(y.Kind != SymbolKind.Namespace, y.Kind.ToString());

            return NamespaceComparer.Compare(x.ContainingNamespace, y.ContainingNamespace);
        }

        public static int CompareName(ISymbol symbol1, ISymbol symbol2)
        {
            return string.CompareOrdinal(symbol1.Name, symbol2.Name);
        }

        private class DefaultSymbolDefinitionComparer : SymbolDefinitionComparer
        {
            private IComparer<INamespaceSymbol> _namespaceComparer;
            private IComparer<INamedTypeSymbol> _typeComparer;
            private IComparer<ISymbol> _memberComparer;

            internal DefaultSymbolDefinitionComparer(SymbolDefinitionSortOptions options = SymbolDefinitionSortOptions.None)
                : base(options)
            {
            }

            public override IComparer<INamespaceSymbol> NamespaceComparer
            {
                get
                {
                    if (_namespaceComparer == null)
                        Interlocked.CompareExchange(ref _namespaceComparer, CreateNamespaceComparer(), null);

                    return _namespaceComparer;

                    IComparer<INamespaceSymbol> CreateNamespaceComparer()
                    {
                        return new NamespaceSymbolDefinitionComparer(this);
                    }
                }
            }

            public override IComparer<INamedTypeSymbol> TypeComparer
            {
                get
                {
                    if (_typeComparer == null)
                        Interlocked.CompareExchange(ref _typeComparer, CreateTypeComparer(), null);

                    return _typeComparer;

                    IComparer<INamedTypeSymbol> CreateTypeComparer()
                    {
                        return new NamedTypeSymbolDefinitionComparer(this);
                    }
                }
            }

            public override IComparer<ISymbol> MemberComparer
            {
                get
                {
                    if (_memberComparer == null)
                        Interlocked.CompareExchange(ref _memberComparer, CreateMemberComparer(), null);

                    return _memberComparer;

                    IComparer<ISymbol> CreateMemberComparer()
                    {
                        return new MemberSymbolDefinitionComparer(this);
                    }
                }
            }
        }
    }
}
