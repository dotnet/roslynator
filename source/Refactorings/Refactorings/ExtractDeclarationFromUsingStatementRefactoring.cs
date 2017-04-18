// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractDeclarationFromUsingStatementRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, UsingStatementSyntax usingStatement)
        {
            VariableDeclarationSyntax declaration = usingStatement.Declaration;

            if (declaration != null
                && usingStatement.IsParentKind(SyntaxKind.Block))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (semanticModel.ContainsCompilerDiagnostic(
                    CSharpErrorCodes.TypeUsedInUsingStatementMustBeImplicitlyConvertibleToIDisposable,
                    declaration.Span,
                    context.CancellationToken))
                {
                    if (context.Span.IsContainedInSpanOrBetweenSpans(declaration))
                        RegisterRefactoring(context, usingStatement);
                }
                else if (context.Span.IsBetweenSpans(declaration))
                {
                    RegisterRefactoring(context, usingStatement);
                }
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, UsingStatementSyntax usingStatement)
        {
            context.RegisterRefactoring(
                "Extract local declaration",
                cancellationToken => RefactorAsync(context.Document, usingStatement, cancellationToken));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var block = (BlockSyntax)usingStatement.Parent;

            int index = block.Statements.IndexOf(usingStatement);

            BlockSyntax newBlock = block.WithStatements(block.Statements.RemoveAt(index));

            var statements = new List<StatementSyntax>() { SyntaxFactory.LocalDeclarationStatement(usingStatement.Declaration) };

            statements.AddRange(GetStatements(usingStatement));

            if (statements.Count > 0)
            {
                statements[0] = statements[0]
                    .WithLeadingTrivia(usingStatement.GetLeadingTrivia());

                statements[statements.Count - 1] = statements[statements.Count - 1]
                    .WithTrailingTrivia(usingStatement.GetTrailingTrivia());
            }

            newBlock = newBlock
                .WithStatements(newBlock.Statements.InsertRange(index, statements))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }

        private static IEnumerable<StatementSyntax> GetStatements(UsingStatementSyntax usingStatement)
        {
            StatementSyntax statement = usingStatement.Statement;

            if (statement != null)
            {
                if (statement.IsKind(SyntaxKind.Block))
                {
                    foreach (StatementSyntax statement2 in ((BlockSyntax)statement).Statements)
                        yield return statement2;
                }
                else
                {
                    yield return statement;
                }
            }
        }
    }
}
