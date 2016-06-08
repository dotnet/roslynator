// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(StatementCodeRefactoringProvider))]
    public class StatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            StatementSyntax statement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<StatementSyntax>();

            if (statement == null)
                return;

            if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                if (localDeclaration.Declaration?.Span.Contains(context.Span) == true)
                {
                    await VariableDeclarationCodeRefactoringProvider.ComputeRefactoringsAsync(
                        context,
                        localDeclaration.Declaration);
                }
            }
            else if (statement.IsKind(SyntaxKind.UsingStatement))
            {
                var usingStatement = (UsingStatementSyntax)statement;

                if (usingStatement.Declaration?.Span.Contains(context.Span) == true)
                {
                    await VariableDeclarationCodeRefactoringProvider.ComputeRefactoringsAsync(
                        context,
                        usingStatement.Declaration);
                }
            }

            AddBracesToEmbeddedStatementRefactoring.Refactor(context, statement);
            RemoveBracesFromStatementRefactoring.Refactor(context, statement);
            ExtractStatementRefactoring.Refactor(context, statement);
        }
    }
}
