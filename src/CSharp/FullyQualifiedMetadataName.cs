// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct FullyQualifiedMetadataName : IEquatable<FullyQualifiedMetadataName>
    {
        public FullyQualifiedMetadataName(IEnumerable<string> containingNamespaces, string name)
            : this(containingNamespaces, Array.Empty<string>(), name)
        {
        }

        public FullyQualifiedMetadataName(IEnumerable<string> containingNamespaces, IEnumerable<string> containingTypes, string name)
            : this(containingNamespaces.ToImmutableArray(), containingTypes.ToImmutableArray(), name)
        {
        }

        public FullyQualifiedMetadataName(ImmutableArray<string> containingNamespaces, string name)
            : this(containingNamespaces, ImmutableArray<string>.Empty, name)
        {
        }

        public FullyQualifiedMetadataName(ImmutableArray<string> containingNamespaces, ImmutableArray<string> containingTypes, string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContainingTypes = containingTypes;
            ContainingNamespaces = containingNamespaces;
        }

        public ImmutableArray<string> ContainingNamespaces { get; }

        public ImmutableArray<string> ContainingTypes { get; }

        public string Name { get; }

        public bool IsDefault
        {
            get
            {
                return Name == null
                    && ContainingTypes.IsDefault
                    && ContainingNamespaces.IsDefault;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            return ToString(SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        }

        public string ToString(SymbolDisplayTypeQualificationStyle typeQualificationStyle)
        {
            if (IsDefault)
                return "";

            switch (typeQualificationStyle)
            {
                case SymbolDisplayTypeQualificationStyle.NameOnly:
                    {
                        return Name;
                    }
                case SymbolDisplayTypeQualificationStyle.NameAndContainingTypes:
                    {
                        if (ContainingTypes.Any())
                            return $"{string.Join("+", ContainingTypes)}+{Name}";

                        return Name;
                    }
                case SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces:
                    {
                        if (ContainingNamespaces.Any())
                        {
                            string @namespace = string.Join(".", ContainingNamespaces);

                            if (ContainingTypes.Any())
                            {
                                return $"{@namespace}.{string.Join("+", ContainingTypes)}+{Name}";
                            }
                            else
                            {
                                return $"{@namespace}.{Name}";
                            }
                        }
                        else if (ContainingTypes.Any())
                        {
                            return $"{string.Join("+", ContainingTypes)}+{Name}";
                        }

                        return Name;
                    }
            }

            throw new ArgumentException("", nameof(typeQualificationStyle));
        }

        public override bool Equals(object obj)
        {
            return obj is FullyQualifiedMetadataName other
                && Equals(other);
        }

        internal bool Equals(ISymbol symbol)
        {
            if (symbol == null)
                return false;

            if (!string.Equals(Name, symbol.MetadataName, StringComparison.Ordinal))
                return false;

            INamedTypeSymbol containingType = symbol.ContainingType;

            for (int i = ContainingTypes.Length - 1; i >= 0; i--)
            {
                if (containingType == null)
                    return false;

                if (!string.Equals(containingType.MetadataName, ContainingTypes[i], StringComparison.Ordinal))
                    return false;

                containingType = containingType.ContainingType;
            }

            if (containingType != null)
                return false;

            INamespaceSymbol containingNamespace = symbol.ContainingNamespace;

            for (int i = ContainingNamespaces.Length - 1; i >= 0; i--)
            {
                if (containingNamespace?.IsGlobalNamespace != false)
                    return false;

                if (!string.Equals(containingNamespace.Name, ContainingNamespaces[i], StringComparison.Ordinal))
                    return false;

                containingNamespace = containingNamespace.ContainingNamespace;
            }

            return containingNamespace?.IsGlobalNamespace == true;
        }

        public bool Equals(FullyQualifiedMetadataName other)
        {
            if (IsDefault)
                return other.IsDefault;

            if (other.IsDefault)
                return false;

            if (!string.Equals(Name, other.Name, StringComparison.Ordinal))
                return false;

            if (!ContainingTypes.SequenceEqual(other.ContainingTypes, StringComparer.Ordinal))
                return false;

            if (!ContainingNamespaces.SequenceEqual(other.ContainingNamespaces, StringComparer.Ordinal))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            if (IsDefault)
                return 0;

            return Hash.Combine(Hash.CombineValues(ContainingNamespaces, StringComparer.Ordinal),
                Hash.Combine(Hash.CombineValues(ContainingTypes, StringComparer.Ordinal),
                Hash.Create(Name)));
        }

        public static bool operator ==(in FullyQualifiedMetadataName info1, in FullyQualifiedMetadataName info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in FullyQualifiedMetadataName info1, in FullyQualifiedMetadataName info2)
        {
            return !(info1 == info2);
        }
    }
}
