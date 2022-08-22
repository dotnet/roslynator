// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    //TODO: make public
    /// <summary>
    /// Provides methods to obtain an unique C# identifier.
    /// </summary>
    internal class CSharpNameGenerator
    {
        private static readonly NameGenerator _generator = new NumberSuffixCSharpNameGenerator();

        /// <summary>
        /// Default implementation of <see cref="CSharpNameGenerator"/> that adds number suffix to ensure uniqueness.
        /// </summary>
        public static CSharpNameGenerator Default { get; } = new();

        /// <summary>
        /// Returns a name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        public string EnsureUniqueName(string baseName, SemanticModel semanticModel, int position)
        {
            return _generator.EnsureUniqueName(baseName, semanticModel, position);
        }

        /// <summary>
        /// Returns unique enum member name for a specified enum type.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="enumType"></param>
        public string EnsureUniqueEnumMemberName(string baseName, INamedTypeSymbol enumType)
        {
            return _generator.EnsureUniqueEnumMemberName(baseName, enumType, isCaseSensitive: true);
        }

        /// <summary>
        /// Return a local name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        /// <param name="cancellationToken"></param>
        public string EnsureUniqueLocalName(
            string baseName,
            SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken = default)
        {
            return _generator.EnsureUniqueLocalName(baseName, semanticModel, position, isCaseSensitive: true, cancellationToken);
        }

        /// <summary>
        /// Return a local names that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        public ImmutableArray<string> EnsureUniqueLocalNames(
            string baseName,
            SemanticModel semanticModel,
            int position,
            int count,
            CancellationToken cancellationToken = default)
        {
            return _generator.EnsureUniqueLocalNames(baseName, semanticModel, position, count, isCaseSensitive: true, cancellationToken);
        }

        /// <summary>
        /// Return a parameter name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="containingSymbol"></param>
        /// <param name="semanticModel"></param>
        /// <param name="cancellationToken"></param>
        public string EnsureUniqueParameterName(
            string baseName,
            ISymbol containingSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            return _generator.EnsureUniqueParameterName(baseName, containingSymbol, semanticModel, isCaseSensitive: true, cancellationToken);
        }

        internal string CreateUniqueLocalName(
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken = default)
        {
            return _generator.CreateUniqueLocalName(typeSymbol, semanticModel, position, isCaseSensitive: true, cancellationToken);
        }

        internal string CreateUniqueLocalName(
            ITypeSymbol typeSymbol,
            string oldName,
            SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken = default)
        {
            return _generator.CreateUniqueLocalName(typeSymbol, oldName, semanticModel, position, isCaseSensitive: true, cancellationToken);
        }

        internal string CreateUniqueParameterName(
            string oldName,
            IParameterSymbol parameterSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            return _generator.CreateUniqueParameterName(oldName, parameterSymbol, semanticModel, isCaseSensitive: true, cancellationToken);
        }

        private class NumberSuffixCSharpNameGenerator : NameGenerator
        {
            public override string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
            {
                int suffix = 1;

                string name = baseName;

                while (!IsUniqueName(name, reservedNames, isCaseSensitive))
                {
                    suffix++;
                    name = baseName + suffix.ToString();
                }

                return CheckKeyword(name);
            }

            public override string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
            {
                int suffix = 1;

                string name = baseName;

                while (!IsUniqueName(name, symbols, isCaseSensitive))
                {
                    suffix++;
                    name = baseName + suffix.ToString();
                }

                return CheckKeyword(name);
            }

            private string CheckKeyword(string name)
            {
                return (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None) ? "@" + name : name;
            }
        }
    }
}
