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
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings.ReplacePropertyWithMethod
{
    internal static class ReplacePropertyWithMethodRefactoring
    {
        private static readonly string[] _prefixes = new string[]
        {
            "Is",
            "Has",
            "Are",
            "Can",
            "Allow",
            "Supports",
            "Should",
            "Get",
            "Set"
        };

        public static void ComputeRefactoring(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (CanRefactor(context, propertyDeclaration))
            {
                string propertyName = propertyDeclaration.Identifier.ValueText;

                string title = $"Replace '{propertyName}' with method";

                if (propertyDeclaration.AccessorList.Accessors.Count > 1)
                    title += "s";

                context.RegisterRefactoring(
                    title,
                    cancellationToken => RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }
        }

        public static bool CanRefactor(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

            if (accessorList != null)
            {
                SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

                if (accessors.Count == 1)
                {
                    AccessorDeclarationSyntax accessor = accessors.First();

                    if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        if (accessor.BodyOrExpressionBody() != null)
                        {
                            return true;
                        }
                        else if (context.SupportsCSharp6
                            && accessor.IsAutoGetter()
                            && propertyDeclaration.Initializer?.Value != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static async Task<Solution> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Solution solution = document.Project.Solution;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(property, cancellationToken);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(propertySymbol, solution, cancellationToken).ConfigureAwait(false);

            ReferenceLocation[] locations = referencedSymbols
                .SelectMany(f => f.Locations)
                .Where(f => !f.IsCandidateLocation && !f.IsImplicit)
                .ToArray();

            string methodName = GetMethodName(property);

            bool isPropertyReplaced = false;

            foreach (IGrouping<DocumentId, ReferenceLocation> grouping in locations
                .GroupBy(f => f.Document.Id))
            {
                Document document2 = solution.GetDocument(grouping.Key);

                SyntaxNode root = await document2.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                TextSpan[] spans = grouping.Select(f => f.Location.SourceSpan).ToArray();

                PropertyDeclarationSyntax property2 = null;

                if (document.Id == document2.Id)
                {
                    isPropertyReplaced = true;
                    property2 = property;
                }

                var rewriter = new ReplacePropertyWithMethodSyntaxRewriter(spans, methodName, property2);

                SyntaxNode newRoot = rewriter.Visit(root);

                solution = solution.WithDocumentSyntaxRoot(grouping.Key, newRoot);
            }

            if (!isPropertyReplaced)
            {
                document = solution.GetDocument(document.Id);

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                var rewriter = new ReplacePropertyWithMethodSyntaxRewriter(new TextSpan[0], methodName, property);

                SyntaxNode newRoot = rewriter.Visit(root);

                solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
            }

            return solution;
        }

        private static string GetMethodName(PropertyDeclarationSyntax propertyDeclaration)
        {
            string methodName = propertyDeclaration.Identifier.ValueText;

            if (!_prefixes.Any(prefix => StringUtility.HasPrefix(methodName, prefix)))
                methodName = "Get" + methodName;

            return methodName;
        }
    }
}
