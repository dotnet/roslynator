// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

#pragma warning disable RCS1223

namespace Roslynator
{
    /// <summary>
    /// Provides equality comparison for <typeparamref name="TSymbol"/> by comparing <see cref="ISymbol.MetadataName"/>,
    /// metadata name of <see cref="ISymbol.ContainingType"/>(s) and metadata name of <see cref="ISymbol.ContainingNamespace"/>(s).
    /// </summary>
    /// <typeparam name="TSymbol"></typeparam>
    public sealed class MetadataNameEqualityComparer<TSymbol> : EqualityComparer<TSymbol> where TSymbol : ISymbol
    {
        private MetadataNameEqualityComparer()
        {
        }

        /// <summary>
        /// Get the instance of <see cref="MetadataNameEqualityComparer{TSymbol}"/> for the specified <typeparamref name="TSymbol"/>.
        /// </summary>
        public static MetadataNameEqualityComparer<TSymbol> Instance { get; } = (MetadataNameEqualityComparer<TSymbol>)GetInstance();

        private static object GetInstance()
        {
            if (typeof(TSymbol) == typeof(IAliasSymbol))
                return new MetadataNameEqualityComparer<IAliasSymbol>();

            if (typeof(TSymbol) == typeof(IArrayTypeSymbol))
                return new MetadataNameEqualityComparer<IArrayTypeSymbol>();

            if (typeof(TSymbol) == typeof(IAssemblySymbol))
                return new MetadataNameEqualityComparer<IAssemblySymbol>();

            if (typeof(TSymbol) == typeof(IDiscardSymbol))
                return new MetadataNameEqualityComparer<IDiscardSymbol>();

            if (typeof(TSymbol) == typeof(IDynamicTypeSymbol))
                return new MetadataNameEqualityComparer<IDynamicTypeSymbol>();

            if (typeof(TSymbol) == typeof(IErrorTypeSymbol))
                return new MetadataNameEqualityComparer<IErrorTypeSymbol>();

            if (typeof(TSymbol) == typeof(IEventSymbol))
                return new MetadataNameEqualityComparer<IEventSymbol>();

            if (typeof(TSymbol) == typeof(IFieldSymbol))
                return new MetadataNameEqualityComparer<IFieldSymbol>();

            if (typeof(TSymbol) == typeof(ILabelSymbol))
                return new MetadataNameEqualityComparer<ILabelSymbol>();

            if (typeof(TSymbol) == typeof(ILocalSymbol))
                return new MetadataNameEqualityComparer<ILocalSymbol>();

            if (typeof(TSymbol) == typeof(IMethodSymbol))
                return new MetadataNameEqualityComparer<IMethodSymbol>();

            if (typeof(TSymbol) == typeof(IModuleSymbol))
                return new MetadataNameEqualityComparer<IModuleSymbol>();

            if (typeof(TSymbol) == typeof(INamedTypeSymbol))
                return new MetadataNameEqualityComparer<INamedTypeSymbol>();

            if (typeof(TSymbol) == typeof(INamespaceOrTypeSymbol))
                return new MetadataNameEqualityComparer<INamespaceOrTypeSymbol>();

            if (typeof(TSymbol) == typeof(INamespaceSymbol))
                return new MetadataNameEqualityComparer<INamespaceSymbol>();

            if (typeof(TSymbol) == typeof(IParameterSymbol))
                return new MetadataNameEqualityComparer<IParameterSymbol>();

            if (typeof(TSymbol) == typeof(IPointerTypeSymbol))
                return new MetadataNameEqualityComparer<IPointerTypeSymbol>();

            if (typeof(TSymbol) == typeof(IPreprocessingSymbol))
                return new MetadataNameEqualityComparer<IPreprocessingSymbol>();

            if (typeof(TSymbol) == typeof(IPropertySymbol))
                return new MetadataNameEqualityComparer<IPropertySymbol>();

            if (typeof(TSymbol) == typeof(IRangeVariableSymbol))
                return new MetadataNameEqualityComparer<IRangeVariableSymbol>();

            if (typeof(TSymbol) == typeof(ISourceAssemblySymbol))
                return new MetadataNameEqualityComparer<ISourceAssemblySymbol>();

            if (typeof(TSymbol) == typeof(ISymbol))
                return new MetadataNameEqualityComparer<ISymbol>();

            if (typeof(TSymbol) == typeof(ITypeParameterSymbol))
                return new MetadataNameEqualityComparer<ITypeParameterSymbol>();

            if (typeof(TSymbol) == typeof(ITypeSymbol))
                return new MetadataNameEqualityComparer<ITypeSymbol>();

            throw new InvalidOperationException();
        }

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type <typeparamref name="TSymbol" /> are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public override bool Equals(TSymbol x, TSymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (Default.Equals(x, default(TSymbol)))
                return false;

            if (Default.Equals(y, default(TSymbol)))
                return false;

            if (!StringComparer.Ordinal.Equals(x.MetadataName, y.MetadataName))
                return false;

            INamedTypeSymbol t1 = x.ContainingType;
            INamedTypeSymbol t2 = y.ContainingType;

            while (!object.ReferenceEquals(t1, t2))
            {
                if (t1 == null)
                    return false;

                if (t2 == null)
                    return false;

                if (!StringComparer.Ordinal.Equals(t1.MetadataName, t2.MetadataName))
                    return false;

                t1 = t1.ContainingType;
                t2 = t2.ContainingType;
            }

            INamespaceSymbol n1 = x.ContainingNamespace;
            INamespaceSymbol n2 = y.ContainingNamespace;

            while (!object.ReferenceEquals(n1, n2))
            {
                if (n1 == null)
                    return false;

                if (n2 == null)
                    return false;

                if (!StringComparer.Ordinal.Equals(n1.MetadataName, n2.MetadataName))
                    return false;

                n1 = n1.ContainingNamespace;
                n2 = n2.ContainingNamespace;
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for the specified symbol.
        /// </summary>
        /// <param name="obj">The symbol for which to get a hash code.</param>
        /// <returns>A hash code for the specified symbol.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <c>null</c>.</exception>
        public override int GetHashCode(TSymbol obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (Default.Equals(obj, default(TSymbol)))
                return 0;

            int hashCode = Hash.Create(MetadataName.GetHashCode(obj.MetadataName));

            INamedTypeSymbol t = obj.ContainingType;

            if (t != null)
            {
                hashCode = Combine(t);

                t = t.ContainingType;

                while (t != null)
                {
                    hashCode = Hash.Combine(MetadataName.PlusHashCode, hashCode);

                    hashCode = Combine(t);

                    t = t.ContainingType;
                }
            }

            INamespaceSymbol n = obj.ContainingNamespace;

            if (n != null)
            {
                hashCode = Combine(n);

                n = n.ContainingNamespace;

                while (n != null)
                {
                    hashCode = Hash.Combine(MetadataName.DotHashCode, hashCode);

                    hashCode = Combine(n);

                    n = n.ContainingNamespace;
                }
            }

            return hashCode;

            int Combine(ISymbol symbol)
            {
                return Hash.Combine(MetadataName.GetHashCode(symbol.MetadataName), hashCode);
            }
        }
    }
}
