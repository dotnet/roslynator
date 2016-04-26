// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(GenericNameCodeRefactoringProvider))]
    public class GenericNameCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            GenericNameSyntax genericName = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<GenericNameSyntax>();

            if (genericName == null)
                return;

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
