// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MemberAccessExpressionCodeRefactoringProvider))]
    public class MemberAccessExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            MemberAccessExpressionSyntax node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberAccessExpressionSyntax>();

            if (node == null)
                return;

            ExpressionChainRefactoring.FormatExpressionChain(context, node);

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel == null)
                return;

            if (EmptyStringRefactoring.CanConvertStringEmptyToEmptyStringLiteral(node, semanticModel))
            {
                context.RegisterRefactoring(
                    "Convert to \"\"",
                    cancellationToken => EmptyStringRefactoring.ConvertStringEmptyToEmptyStringLiteralAsync(context.Document, node, cancellationToken));
            }
        }
    }
}
