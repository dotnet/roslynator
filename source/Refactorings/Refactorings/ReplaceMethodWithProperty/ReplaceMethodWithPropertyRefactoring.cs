// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.ReplaceMethodWithProperty
{
    internal static class ReplaceMethodWithPropertyRefactoring
    {
        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.ParameterList?.Parameters.Count == 0
                && methodDeclaration.TypeParameterList == null;
        }

        public static async Task<Solution> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Solution solution = document.Project.Solution;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(methodSymbol, solution, cancellationToken).ConfigureAwait(false);

            ReferenceLocation[] locations = referencedSymbols
                .SelectMany(f => f.Locations)
                .Where(f => !f.IsCandidateLocation && !f.IsImplicit)
                .ToArray();

            string propertyName = GetPropertyName(methodDeclaration);

            bool isMethodReplaced = false;

            foreach (IGrouping<DocumentId, ReferenceLocation> grouping in locations
                .GroupBy(f => f.Document.Id))
            {
                Document document2 = solution.GetDocument(grouping.Key);

                SyntaxNode root = await document2.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                TextSpan[] spans = grouping.Select(f => f.Location.SourceSpan).ToArray();

                MethodDeclarationSyntax methodDeclaration2 = null;

                if (document.Id == document2.Id)
                {
                    isMethodReplaced = true;
                    methodDeclaration2 = methodDeclaration;
                }

                var rewriter = new ReplaceMethodWithPropertySyntaxRewriter(spans, propertyName, methodDeclaration2);

                SyntaxNode newRoot = rewriter.Visit(root);

                solution = solution.WithDocumentSyntaxRoot(grouping.Key, newRoot);
            }

            if (!isMethodReplaced)
            {
                document = solution.GetDocument(document.Id);

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                var rewriter = new ReplaceMethodWithPropertySyntaxRewriter(new TextSpan[0], propertyName, methodDeclaration);

                SyntaxNode newRoot = rewriter.Visit(root);

                solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
            }

            return solution;
        }

        private static string GetPropertyName(MethodDeclarationSyntax methodDeclaration)
        {
            string methodName = methodDeclaration.Identifier.ValueText;

            if (TextUtility.HasPrefix(methodName, "Get"))
            {
                return methodName.Substring(3);
            }
            else
            {
                return methodName;
            }
        }
    }
}
