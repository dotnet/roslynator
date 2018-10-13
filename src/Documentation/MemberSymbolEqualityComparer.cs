// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal class MemberSymbolEqualityComparer : EqualityComparer<ISymbol>
    {
        public static MemberSymbolEqualityComparer Instance { get; } = new MemberSymbolEqualityComparer();

        public override bool Equals(ISymbol x, ISymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (x == null)
                return false;

            if (y == null)
                return false;

            if (!string.Equals(x.Name, y.Name, StringComparison.Ordinal))
                return false;

            switch (x.Kind)
            {
                case SymbolKind.Event:
                case SymbolKind.Field:
                    {
                        switch (y.Kind)
                        {
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                            case SymbolKind.Property:
                                return true;
                            default:
                                return false;
                        }
                    }
                case SymbolKind.Method:
                    {
                        switch (y.Kind)
                        {
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Property:
                                {
                                    return true;
                                }
                            case SymbolKind.Method:
                                {
                                    var a = (IMethodSymbol)x;
                                    var b = (IMethodSymbol)y;

                                    return a.MethodKind == MethodKind.Ordinary
                                        && b.MethodKind == MethodKind.Ordinary
                                        && a.TypeParameters.Length == b.TypeParameters.Length
                                        && ParameterEqualityComparer.ParametersEqual(a.Parameters, b.Parameters);
                                }
                            default:
                                {
                                    return false;
                                }
                        }
                    }
                case SymbolKind.Property:
                    {
                        switch (y.Kind)
                        {
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                                {
                                    return true;
                                }
                            case SymbolKind.Property:
                                {
                                    var a = (IPropertySymbol)x;
                                    var b = (IPropertySymbol)y;

                                    return a.IsIndexer == b.IsIndexer
                                        && ParameterEqualityComparer.ParametersEqual(a.Parameters, b.Parameters);
                                }
                            default:
                                {
                                    return false;
                                }
                        }
                    }
                case SymbolKind.NamedType:
                    {
                        if (y.Kind != SymbolKind.NamedType)
                            return false;

                        var a = (INamedTypeSymbol)x;
                        var b = (INamedTypeSymbol)y;

                        if (a.TypeParameters.Length != b.TypeParameters.Length)
                            return false;

                        switch (a.TypeKind)
                        {
                            case TypeKind.Class:
                            case TypeKind.Delegate:
                            case TypeKind.Enum:
                            case TypeKind.Interface:
                            case TypeKind.Struct:
                                {
                                    switch (b.TypeKind)
                                    {
                                        case TypeKind.Class:
                                        case TypeKind.Delegate:
                                        case TypeKind.Enum:
                                        case TypeKind.Interface:
                                        case TypeKind.Struct:
                                            return true;
                                        default:
                                            return false;
                                    }
                                }
                            default:
                                {
                                    return false;
                                }
                        }
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public override int GetHashCode(ISymbol obj)
        {
            return StringComparer.Ordinal.GetHashCode(obj.Name);
        }

        private class ParameterEqualityComparer : EqualityComparer<IParameterSymbol>
        {
            public static ParameterEqualityComparer Instance { get; } = new ParameterEqualityComparer();

            public static bool ParametersEqual(ImmutableArray<IParameterSymbol> parameters1, ImmutableArray<IParameterSymbol> parameters2)
            {
                int length = parameters1.Length;

                if (length != parameters2.Length)
                    return false;

                for (int i = 0; i < length; i++)
                {
                    if (!Instance.Equals(parameters1[i], parameters2[i]))
                        return false;
                }

                return true;
            }

            public override bool Equals(IParameterSymbol x, IParameterSymbol y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return x.RefKind == y.RefKind
                    && x.Type == y.Type;
            }

            public override int GetHashCode(IParameterSymbol obj)
            {
                if (obj == null)
                    return 0;

                return Hash.Combine(obj.Type, (int)obj.RefKind);
            }
        }
    }
}
