// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ForStatementCodeRefactoringProvider))]
    public class ForStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ForStatementSyntax forStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ForStatementSyntax>();

            if (forStatement == null)
                return;

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                bool canRefactor = await ForToForEachRefactoring.CanRefactorAsync(context, forStatement, semanticModel);

                if (canRefactor)
                {
                    context.RegisterRefactoring(
                        "Convert for to foreach",
                        cancellationToken => ForToForEachRefactoring.RefactorAsync(context.Document, forStatement, semanticModel, cancellationToken));
                }
            }

            if (ReverseForRefactoring.CanRefactor(forStatement))
            {
                context.RegisterRefactoring(
                    "Reverse for loop",
                    cancellationToken => ReverseForRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
            }
        }
    }
}
