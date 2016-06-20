// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RegionDirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, RegionDirectiveTriviaSyntax regionDirectiveTrivia)
        {
            context.RegisterRefactoring(
                "Remove all regions",
                cancellationToken => RemoveAllRegionsAsync(context.Document, cancellationToken));
        }

        private static async Task<Document> RemoveAllRegionsAsync(
            Document document,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            CompilationUnitSyntax newRoot = RegionRemover.RemoveFrom((CompilationUnitSyntax)oldRoot)
                .WithAdditionalAnnotations(Formatter.Annotation);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
