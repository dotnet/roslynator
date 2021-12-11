// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WhileStatementRefactoring
    {
        internal static readonly string ConvertWhileToDoWithoutIfEquivalenceKey = EquivalenceKey.Create(RefactoringDescriptors.ConvertWhileToDo, "WithoutIf");

        public static void ComputeRefactorings(RefactoringContext context, WhileStatementSyntax whileStatement)
        {
            Document document = context.Document;
            SyntaxToken whileKeyword = whileStatement.WhileKeyword;

            bool spanIsEmptyAndContainedInWhileKeyword = context.Span.IsEmptyAndContainedInSpan(whileKeyword);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertWhileToDo)
                && spanIsEmptyAndContainedInWhileKeyword)
            {
                context.RegisterRefactoring(
                    "Convert to 'do'",
                    ct => ConvertWhileToDoAsync(document, whileStatement, omitIfStatement: false, ct),
                    RefactoringDescriptors.ConvertWhileToDo);

                context.RegisterRefactoring(
                    "Convert to 'do' (without 'if')",
                    ct => ConvertWhileToDoAsync(document, whileStatement, omitIfStatement: true, ct),
                    ConvertWhileToDoWithoutIfEquivalenceKey);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertWhileToFor)
                && spanIsEmptyAndContainedInWhileKeyword)
            {
                context.RegisterRefactoring(
                    ConvertWhileToForRefactoring.Title,
                    ct => ConvertWhileToForRefactoring.RefactorAsync(document, whileStatement, ct),
                    RefactoringDescriptors.ConvertWhileToFor);
            }
        }

        private static Task<Document> ConvertWhileToDoAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            bool omitIfStatement = false,
            CancellationToken cancellationToken = default)
        {
            if (omitIfStatement)
            {
                DoStatementSyntax doStatement = DoStatement(
                    Token(
                        whileStatement.WhileKeyword.LeadingTrivia,
                        SyntaxKind.DoKeyword,
                        whileStatement.CloseParenToken.TrailingTrivia),
                    whileStatement.Statement.WithoutTrailingTrivia(),
                    Token(SyntaxKind.WhileKeyword),
                    whileStatement.OpenParenToken,
                    whileStatement.Condition,
                    whileStatement.CloseParenToken.WithoutTrailingTrivia(),
                    SemicolonToken());

                doStatement = doStatement
                    .WithTriviaFrom(whileStatement)
                    .WithFormatterAnnotation();

                return document.ReplaceNodeAsync(whileStatement, doStatement, cancellationToken);
            }
            else
            {
                DoStatementSyntax doStatement = DoStatement(
                    Token(SyntaxKind.DoKeyword),
                    whileStatement.Statement.WithoutTrailingTrivia(),
                    Token(SyntaxKind.WhileKeyword),
                    OpenParenToken(),
                    whileStatement.Condition,
                    CloseParenToken(),
                    SemicolonToken());

                IfStatementSyntax ifStatement = IfStatement(
                    Token(whileStatement.WhileKeyword.LeadingTrivia, SyntaxKind.IfKeyword, TriviaList()),
                    OpenParenToken(),
                    whileStatement.Condition,
                    CloseParenToken(),
                    Block(OpenBraceToken(), doStatement, Token(TriviaList(), SyntaxKind.CloseBraceToken, whileStatement.Statement.GetTrailingTrivia())),
                    default(ElseClauseSyntax));

                ifStatement = ifStatement.WithFormatterAnnotation();

                return document.ReplaceNodeAsync(whileStatement, ifStatement, cancellationToken);
            }
        }
    }
}
