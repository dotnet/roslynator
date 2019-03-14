// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal class SymbolDefinitionComparer : IComparer<ISymbol>
    {
        private IComparer<INamespaceSymbol> _namespaceComparer;
        private IComparer<INamedTypeSymbol> _typeComparer;
        private IComparer<ISymbol> _memberComparer;

        internal SymbolDefinitionComparer(SymbolDefinitionSortOptions options = SymbolDefinitionSortOptions.None)
        {
            Options = options;
        }

        public static SymbolDefinitionComparer Default { get; } = new SymbolDefinitionComparer(SymbolDefinitionSortOptions.None);

        public static SymbolDefinitionComparer SystemFirst { get; } = new SymbolDefinitionComparer(SymbolDefinitionSortOptions.SystemFirst);

        public static SymbolDefinitionComparer SystemFirstOmitContainingNamespace { get; } = new SymbolDefinitionComparer(SymbolDefinitionSortOptions.SystemFirst | SymbolDefinitionSortOptions.OmitContainingNamespace);

        public SymbolDefinitionSortOptions Options { get; }

        public IComparer<INamespaceSymbol> NamespaceComparer
        {
            get
            {
                if (_namespaceComparer == null)
                    Interlocked.CompareExchange(ref _namespaceComparer, CreateNamespaceComparer(), null);

                return _namespaceComparer;
            }
        }

        public IComparer<INamedTypeSymbol> TypeComparer
        {
            get
            {
                if (_typeComparer == null)
                    Interlocked.CompareExchange(ref _typeComparer, CreateTypeComparer(), null);

                return _typeComparer;
            }
        }

        public IComparer<ISymbol> MemberComparer
        {
            get
            {
                if (_memberComparer == null)
                    Interlocked.CompareExchange(ref _memberComparer, CreateMemberComparer(), null);

                return _memberComparer;
            }
        }

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
            int diff = NamespaceComparer.Compare(typeSymbol1.ContainingNamespace, typeSymbol2.ContainingNamespace);

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

        public static int CompareName(ISymbol symbol1, ISymbol symbol2)
        {
            return string.Compare(symbol1.Name, symbol2.Name, StringComparison.Ordinal);
        }

        protected virtual IComparer<INamespaceSymbol> CreateNamespaceComparer()
        {
            return new NamespaceSymbolDefinitionComparer(this);
        }

        protected virtual IComparer<INamedTypeSymbol> CreateTypeComparer()
        {
            return new NamedTypeSymbolDefinitionComparer(this);
        }

        protected virtual IComparer<ISymbol> CreateMemberComparer()
        {
            return new MemberSymbolDefinitionComparer(this);
        }
    }
}
