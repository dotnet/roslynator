// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Roslynator
{
    internal static class SyntaxFinder
    {
        public static Task<ImmutableArray<SyntaxNode>> FindReferencesAsync(
            ISymbol symbol,
            Solution solution,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindReferencesAsync(symbol, solution, null, allowCandidate, cancellationToken);
        }

        public static async Task<ImmutableArray<SyntaxNode>> FindReferencesAsync(
            ISymbol symbol,
            Solution solution,
            IImmutableSet<Document> documents,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            List<SyntaxNode> nodes = null;

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(
                symbol,
                solution,
                documents,
                cancellationToken).ConfigureAwait(false))
            {
                foreach (IGrouping<Document, ReferenceLocation> grouping in referencedSymbol.Locations.GroupBy(f => f.Document))
                {
                    Document document = grouping.Key;

                    SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                    FindReferences(grouping, root, allowCandidate, ref nodes);
                }
            }

            return ToImmutableArray(nodes);
        }

        public static Task<ImmutableArray<DocumentReferenceInfo>> FindReferencesByDocumentAsync(
            ISymbol symbol,
            Solution solution,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindReferencesByDocumentAsync(symbol, solution, null, allowCandidate, cancellationToken);
        }

        public static async Task<ImmutableArray<DocumentReferenceInfo>> FindReferencesByDocumentAsync(
            ISymbol symbol,
            Solution solution,
            IImmutableSet<Document> documents,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            List<DocumentReferenceInfo> infos = null;

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(
                symbol,
                solution,
                documents,
                cancellationToken).ConfigureAwait(false))
            {
                foreach (IGrouping<Document, ReferenceLocation> grouping in referencedSymbol.Locations.GroupBy(f => f.Document))
                {
                    Document document = grouping.Key;

                    SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                    List<SyntaxNode> nodes = null;

                    FindReferences(grouping, root, allowCandidate, ref nodes);

                    if (nodes != null)
                    {
                        var info = new DocumentReferenceInfo(document, root, nodes.ToImmutableArray());

                        (infos ?? (infos = new List<DocumentReferenceInfo>())).Add(info);
                    }
                }
            }

            return ToImmutableArray(infos);
        }

        public static async Task<ImmutableArray<SyntaxNode>> FindReferencesAsync(
            ISymbol symbol,
            Document document,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            List<SyntaxNode> nodes = null;

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(
                symbol,
                document.Solution(),
                ImmutableHashSet.Create(document),
                cancellationToken).ConfigureAwait(false))
            {
                FindReferences(referencedSymbol.Locations, root, allowCandidate, ref nodes);
            }

            return ToImmutableArray(nodes);
        }

        private static void FindReferences(
            IEnumerable<ReferenceLocation> referenceLocations,
            SyntaxNode root,
            bool allowCandidate,
            ref List<SyntaxNode> nodes)
        {
            foreach (ReferenceLocation referenceLocation in referenceLocations)
            {
                if (!referenceLocation.IsImplicit
                    && (allowCandidate || !referenceLocation.IsCandidateLocation))
                {
                    Location location = referenceLocation.Location;

                    if (location.IsInSource)
                    {
                        SyntaxNode node = root.FindNode(location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

                        Debug.Assert(node != null);

                        (nodes ?? (nodes = new List<SyntaxNode>())).Add(node);
                    }
                }
            }
        }

        private static ImmutableArray<T> ToImmutableArray<T>(IEnumerable<T> nodes)
        {
            if (nodes != null)
            {
                return ImmutableArray.CreateRange(nodes);
            }
            else
            {
                return ImmutableArray<T>.Empty;
            }
        }
    }
}
