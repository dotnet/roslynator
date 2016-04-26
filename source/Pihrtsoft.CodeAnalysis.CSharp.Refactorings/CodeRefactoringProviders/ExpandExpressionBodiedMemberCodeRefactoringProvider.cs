// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ExpandExpressionBodiedMemberCodeRefactoringProvider))]
    public class ExpandExpressionBodiedMemberCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ArrowExpressionClauseSyntax arrowExpressionClause = root
                .FindNode(context.Span)?
                .FirstAncestorOrSelf<ArrowExpressionClauseSyntax>();

            if (arrowExpressionClause?.Parent?.SupportsExpressionBody() == true)
            {
                context.RegisterRefactoring(
                    "Expand expression-bodied member",
                    cancellationToken => ExpandExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, arrowExpressionClause, cancellationToken));
            }
        }
    }
}