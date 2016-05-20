// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
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

            if (node?.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return;

            ExpressionChainRefactoring.FormatExpressionChain(context, node);

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                ConvertStringEmptyToEmptyStringLiteral(context, node, semanticModel);
            }
        }

        private static void ConvertStringEmptyToEmptyStringLiteral(CodeRefactoringContext context, MemberAccessExpressionSyntax memberAccess, SemanticModel semanticModel)
        {
            if (StringEmptyRefactoring.CanConvertStringEmptyToEmptyStringLiteral(memberAccess, semanticModel, context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Convert to \"\"",
                    cancellationToken =>
                    {
                        return StringEmptyRefactoring.ConvertStringEmptyToEmptyStringLiteralAsync(
                            context.Document,
                            memberAccess,
                            cancellationToken);
                    });
            }
            else
            {
                memberAccess = (MemberAccessExpressionSyntax)memberAccess
                    .FirstAncestor(SyntaxKind.SimpleMemberAccessExpression);

                if (memberAccess != null)
                    ConvertStringEmptyToEmptyStringLiteral(context, memberAccess, semanticModel);
            }
        }
    }
}
