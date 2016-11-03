// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analyzers;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceIfStatementWithReturnStatementCodeFixProvider))]
    [Shared]
    public class ReplaceIfStatementWithReturnStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.ReplaceIfStatementWithReturnStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var ifStatement = (IfStatementSyntax)root.DescendantNodes(context.Span)
                .FirstOrDefault(f => f.IsKind(SyntaxKind.IfStatement) && context.Span.Contains(f.Span));

            if (ifStatement == null)
                return;

            ReturnStatementSyntax returnStatement = CreateReturnStatement(ifStatement);

            CodeAction codeAction = CodeAction.Create(
                $"Replace if with '{returnStatement}'",
                cancellationToken => RefactorAsync(context.Document, ifStatement, returnStatement, cancellationToken),
                DiagnosticIdentifiers.ReplaceIfStatementWithReturnStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static ReturnStatementSyntax CreateReturnStatement(IfStatementSyntax ifStatement)
        {
            LiteralExpressionSyntax booleanLiteral = SimplifyIfStatementToReturnStatementAnalyzer.GetBooleanLiteral(ifStatement.Statement);

            ExpressionSyntax expression = ifStatement.Condition;

            if (booleanLiteral.IsKind(SyntaxKind.FalseLiteralExpression))
                expression = expression.Negate();

            return SyntaxFactory.ReturnStatement(
                SyntaxFactory.Token(SyntaxKind.ReturnKeyword).WithTrailingSpace(),
                expression,
                SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax newReturnStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = GetNewRoot(oldRoot, ifStatement, newReturnStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode GetNewRoot(
            SyntaxNode root,
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax newReturnStatement)
        {
            if (ifStatement.Else != null)
            {
                newReturnStatement = newReturnStatement.WithTriviaFrom(ifStatement);

                return root.ReplaceNode(ifStatement, newReturnStatement);
            }
            else
            {
                var block = (BlockSyntax)ifStatement.Parent;

                int index = block.Statements.IndexOf(ifStatement);

                var returnStatement = (ReturnStatementSyntax)block.Statements[index + 1];

                newReturnStatement = newReturnStatement
                    .WithLeadingTrivia(ifStatement.GetLeadingTrivia())
                    .WithTrailingTrivia(returnStatement.GetTrailingTrivia());

                SyntaxList<StatementSyntax> statements = block.Statements
                    .RemoveAt(index);

                statements = statements
                    .Replace(statements[index], newReturnStatement);

                return root.ReplaceNode(block, block.WithStatements(statements));
            }
        }
    }
}
