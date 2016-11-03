// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.RemoveUsingAliasDirective
{
    public class RemoveUsingAliasDirectiveSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly UsingDirectiveSyntax _usingDirective;
        private readonly IdentifierNameSyntax[] _identifierNames;

        public RemoveUsingAliasDirectiveSyntaxRewriter(UsingDirectiveSyntax usingDirective, IdentifierNameSyntax[] identifierNames)
        {
            if (usingDirective == null)
                throw new ArgumentNullException(nameof(usingDirective));

            if (identifierNames == null)
                throw new ArgumentNullException(nameof(identifierNames));

            _usingDirective = usingDirective;
            _identifierNames = identifierNames;
        }

        public static async Task<Document> VisitAsync(
            Document document,
            UsingDirectiveSyntax usingDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (usingDirective == null)
                throw new ArgumentNullException(nameof(usingDirective));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxTree tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var aliasSymbol = semanticModel.GetDeclaredSymbol(usingDirective, cancellationToken) as IAliasSymbol;

            var identifierNames = new List<IdentifierNameSyntax>();

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(aliasSymbol, document.Project.Solution, cancellationToken).ConfigureAwait(false);

            foreach (TextSpan span in GetSymbolSpans(referencedSymbols, tree))
            {
                IdentifierNameSyntax identifierName = root
                   .FindNode(span, getInnermostNodeForTie: true)
                   .FirstAncestorOrSelf<IdentifierNameSyntax>();

                if (identifierName != null
                    && aliasSymbol == semanticModel.GetAliasInfo(identifierName, cancellationToken))
                {
                    identifierNames.Add(identifierName);
                }
            }

            var rewriter = new RemoveUsingAliasDirectiveSyntaxRewriter(usingDirective, identifierNames.ToArray());

            SyntaxNode newRoot = rewriter.Visit(root);

            usingDirective = (UsingDirectiveSyntax)newRoot.FindNode(usingDirective.Span);

            newRoot = newRoot.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IEnumerable<TextSpan> GetSymbolSpans(IEnumerable<ReferencedSymbol> referencedSymbols, SyntaxTree syntaxTree)
        {
            foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (!referenceLocation.IsImplicit)
                    {
                        Location location = referenceLocation.Location;

                        if (location.SourceTree == syntaxTree)
                            yield return location.SourceSpan;
                    }
                }
            }
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (Array.IndexOf(_identifierNames, node) != -1)
                return _usingDirective.Name
                    .WithTriviaFrom(node)
                    .WithSimplifierAnnotation();

            return base.VisitIdentifierName(node);
        }
    }
}
