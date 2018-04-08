// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.AddOrRemoveParameterName
{
    internal static class RemoveParameterNameRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ArgumentListSyntax argumentList,
            SeparatedSyntaxListSelection<ArgumentSyntax> selection)
        {
            if (!CanRefactor(selection))
                return;

            context.RegisterRefactoring(
                "Remove parameter name",
                cancellationToken => RefactorAsync(context.Document, argumentList, selection, cancellationToken));
        }

        private static bool CanRefactor(SeparatedSyntaxListSelection<ArgumentSyntax> selection)
        {
            for (int i = 0; i < selection.Count; i++)
            {
                NameColonSyntax nameColon = selection[i].NameColon;

                if (nameColon?.IsMissing == false)
                    return true;
            }

            return false;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ArgumentListSyntax argumentList,
            SeparatedSyntaxListSelection<ArgumentSyntax> selection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var rewriter = new RemoveParameterNameRewriter(selection.ToImmutableArray());

            SyntaxNode newNode = rewriter
                .Visit(argumentList)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(argumentList, newNode, cancellationToken);
        }
    }
}
