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
    internal readonly struct MetadataName : IEquatable<MetadataName>
    {
        public MetadataName(IEnumerable<string> containingNamespaces, string name)
            : this(containingNamespaces, Array.Empty<string>(), name)
        {
            if (containingNamespaces == null)
                throw new ArgumentNullException(nameof(containingNamespaces));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContainingTypes = ImmutableArray<string>.Empty;
            ContainingNamespaces = containingNamespaces.ToImmutableArray();
        }

        public MetadataName(IEnumerable<string> containingNamespaces, IEnumerable<string> containingTypes, string name)
        {
            if (containingNamespaces == null)
                throw new ArgumentNullException(nameof(containingNamespaces));

            if (containingTypes == null)
                throw new ArgumentNullException(nameof(containingTypes));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContainingTypes = containingTypes.ToImmutableArray();
            ContainingNamespaces = containingNamespaces.ToImmutableArray();
        }

        public MetadataName(ImmutableArray<string> containingNamespaces, string name)
            : this(containingNamespaces, ImmutableArray<string>.Empty, name)
        {
        }

        public MetadataName(ImmutableArray<string> containingNamespaces, ImmutableArray<string> containingTypes, string name)
        {
            if (containingNamespaces.IsDefault)
                throw new ArgumentException("Containing namespaces are not initialized.", nameof(containingNamespaces));

            if (containingTypes.IsDefault)
                throw new ArgumentException("Containing types are not initialized.", nameof(containingTypes));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContainingTypes = containingTypes;
            ContainingNamespaces = containingNamespaces;
        }

        public ImmutableArray<string> ContainingNamespaces { get; }

        public ImmutableArray<string> ContainingTypes { get; }

        public string Name { get; }

        public bool IsDefault
        {
            get { return Name == null; }
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

        internal string ToString(SymbolDisplayTypeQualificationStyle typeQualificationStyle)
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
                            return string.Join("+", ContainingTypes) + "+" + Name;

                        return Name;
                    }
                case SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces:
                    {
                        if (ContainingNamespaces.Any())
                        {
                            string @namespace = string.Join(".", ContainingNamespaces);

                            if (ContainingTypes.Any())
                            {
                                return @namespace + "." + string.Join("+", ContainingTypes) + "+" + Name;
                            }
                            else
                            {
                                return @namespace + "." + Name;
                            }
                        }
                        else if (ContainingTypes.Any())
                        {
                            return string.Join("+", ContainingTypes) + "+" + Name;
                        }

                        return Name;
                    }
            }

            throw new ArgumentException($"Unknown enum value '{typeQualificationStyle}'.", nameof(typeQualificationStyle));
        }

        public override bool Equals(object obj)
        {
            return obj is MetadataName other
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

        public bool Equals(MetadataName other)
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

        public static MetadataName Parse(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            int length = name.Length;

            if (length == 0)
                throw new ArgumentException("Name cannot be empty.", nameof(name));

            string containingType = null;

            int prevIndex = 0;

            int containingNamespaceCount = 0;
            int containingTypeCount = 0;

            for (int i = 0; i < length; i++)
            {
                if (name[i] == '.')
                {
                    if (containingTypeCount > 0
                        || i == prevIndex
                        || i == length - 1)
                    {
                        throw new ArgumentException("Name is invalid.", nameof(name));
                    }

                    containingNamespaceCount++;

                    prevIndex = i + 1;
                }

                if (name[i] == '+')
                {
                    if (i == prevIndex
                        || i == length - 1)
                    {
                        throw new ArgumentException("Name is invalid.", nameof(name));
                    }

                    containingTypeCount++;

                    prevIndex = i + 1;
                }
            }

            ImmutableArray<string>.Builder containingNamespaces = (containingNamespaceCount > 0)
                ? ImmutableArray.CreateBuilder<string>(containingNamespaceCount)
                : null;

            ImmutableArray<string>.Builder containingTypes = (containingTypeCount > 1)
                ? ImmutableArray.CreateBuilder<string>(containingTypeCount)
                : null;

            prevIndex = 0;

            for (int i = 0; i < length; i++)
            {
                if (name[i] == '.')
                {
                    string n = name.Substring(prevIndex, i - prevIndex);

                    containingNamespaces.Add(n);

                    prevIndex = i + 1;
                }
                else if (name[i] == '+')
                {
                    string n = name.Substring(prevIndex, i - prevIndex);

                    if (containingTypes != null)
                    {
                        containingTypes.Add(n);
                    }
                    else
                    {
                        containingType = n;
                    }

                    prevIndex = i + 1;
                }
            }

            return new MetadataName(
                containingNamespaces?.MoveToImmutable() ?? ImmutableArray<string>.Empty,
                (containingType != null)
                    ? ImmutableArray.Create(containingType)
                    : containingTypes?.MoveToImmutable() ?? ImmutableArray<string>.Empty,
                name.Substring(prevIndex, length - prevIndex));
        }

        public static bool operator ==(in MetadataName info1, in MetadataName info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in MetadataName info1, in MetadataName info2)
        {
            return !(info1 == info2);
        }
    }
}
