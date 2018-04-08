// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.AddOrRemoveParameterName
{
    internal static class AddParameterNameRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ArgumentListSyntax argumentList,
            SeparatedSyntaxListSelection<ArgumentSyntax> selection,
            SemanticModel semanticModel)
        {
            if (!CanRefactor(selection, semanticModel, context.CancellationToken))
                return;

            context.RegisterRefactoring(
                "Add parameter name",
                cancellationToken => RefactorAsync(context.Document, argumentList, selection, cancellationToken));
        }

        private static bool CanRefactor(
            SeparatedSyntaxListSelection<ArgumentSyntax> selection,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < selection.Count; i++)
            {
                ArgumentSyntax argument = selection[i];

                NameColonSyntax nameColon = argument.NameColon;

                if (nameColon == null
                    || nameColon.IsMissing)
                {
                    IParameterSymbol parameterSymbol = semanticModel.DetermineParameter(
                        argument,
                        allowParams: false,
                        cancellationToken: cancellationToken);

                    if (parameterSymbol != null)
                        return true;
                }
            }

            return false;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ArgumentListSyntax argumentList,
            SeparatedSyntaxListSelection<ArgumentSyntax> selection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new AddParameterNameRewriter(selection.ToImmutableArray(), semanticModel);

            SyntaxNode newNode = rewriter
                .Visit(argumentList)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(argumentList, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
