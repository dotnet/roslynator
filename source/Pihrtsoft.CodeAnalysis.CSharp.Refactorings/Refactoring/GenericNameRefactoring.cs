// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class GenericNameRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, GenericNameSyntax genericName)
        {
            if (genericName.TypeArgumentList != null
                && genericName.TypeArgumentList.Arguments.Count == 1
                && genericName.TypeArgumentList.Arguments[0].Span.Contains(context.Span))
            {
                context.RegisterRefactoring(
                    "Extract generic type",
                    cancellationToken => ExtractGenericTypeAsync(context.Document, genericName, cancellationToken));
            }
        }

        private static async Task<Document> ExtractGenericTypeAsync(
            Document document,
            GenericNameSyntax genericName,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            TypeSyntax typeSyntax = genericName
                .TypeArgumentList
                .Arguments[0]
                .WithTriviaFrom(genericName);

            SyntaxNode newRoot = oldRoot.ReplaceNode(genericName, typeSyntax);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
