// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineConstantRefactoring
    {
        private static readonly SyntaxAnnotation _removeAnnotation = new SyntaxAnnotation();

        public static async Task<Solution> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            VariableDeclaratorSyntax variableDeclarator,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax value = variableDeclarator.Initializer.Value;

            ParenthesizedExpressionSyntax newValue = value.Parenthesize();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetDeclaredSymbol(variableDeclarator, cancellationToken);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(symbol, document.Solution(), cancellationToken).ConfigureAwait(false);

            var newDocuments = new List<KeyValuePair<DocumentId, SyntaxNode>>();

            foreach (IGrouping<Document, ReferenceLocation> grouping in referencedSymbols
                .First()
                .Locations
                .Where(f => !f.IsImplicit && !f.IsCandidateLocation)
                .GroupBy(f => f.Document))
            {
                SyntaxNode root = await grouping.Key.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode newRoot = root.ReplaceNodes(
                    GetNodesToReplace(grouping.AsEnumerable(), root, fieldDeclaration, variableDeclarator),
                    (f, _) =>
                    {
                        if (f.IsKind(SyntaxKind.FieldDeclaration, SyntaxKind.VariableDeclarator))
                            return f.WithAdditionalAnnotations(_removeAnnotation);

                        return newValue;
                    });

                SyntaxNode nodeToRemove = newRoot.GetAnnotatedNodes(_removeAnnotation).FirstOrDefault();

                if (nodeToRemove != null)
                    newRoot = newRoot.RemoveNode(nodeToRemove);

                newDocuments.Add(new KeyValuePair<DocumentId, SyntaxNode>(grouping.Key.Id, newRoot));
            }

            Solution newSolution = document.Solution();

            foreach (KeyValuePair<DocumentId, SyntaxNode> kvp in newDocuments)
                newSolution = newSolution.WithDocumentSyntaxRoot(kvp.Key, kvp.Value);

            return newSolution;
        }

        private static IEnumerable<SyntaxNode> GetNodesToReplace(
            IEnumerable<ReferenceLocation> referenceLocations,
            SyntaxNode root,
            FieldDeclarationSyntax fieldDeclaration,
            VariableDeclaratorSyntax variableDeclarator)
        {
            foreach (ReferenceLocation referenceLocation in referenceLocations)
            {
                if (!referenceLocation.IsImplicit
                    && !referenceLocation.IsCandidateLocation)
                {
                    SyntaxNode node = root.FindNode(referenceLocation.Location.SourceSpan, getInnermostNodeForTie: true);

                    if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        yield return node.Parent;
                    }
                    else
                    {
                        yield return node;
                    }
                }
            }

            if (variableDeclarator.SyntaxTree == root.SyntaxTree)
            {
                if (fieldDeclaration.Declaration.Variables.Count == 1)
                {
                    yield return fieldDeclaration;
                }
                else
                {
                    yield return variableDeclarator;
                }
            }
        }
    }
}
