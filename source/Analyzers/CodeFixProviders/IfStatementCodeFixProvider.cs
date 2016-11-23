// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfStatementCodeFixProvider))]
    [Shared]
    public class IfStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement,
                    DiagnosticIdentifiers.SimplifyIfElseStatement);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            IfStatementSyntax ifStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IfStatementSyntax>();

            if (ifStatement == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Merge if with nested if",
                                cancellationToken =>
                                {
                                    return MergeIfStatementWithNestedIfStatementAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.SimplifyIfElseStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Simplify if-else",
                                cancellationToken =>
                                {
                                    return SimplifyIfElseStatementRefactoring.RefactorAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> MergeIfStatementWithNestedIfStatementAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax nestedIf = GetNestedIfStatement(ifStatement);

            BinaryExpressionSyntax newCondition = CSharpFactory.LogicalAndExpression(
                ifStatement.Condition,
                nestedIf.Condition,
                addParenthesesIfNecessary: true);

            IfStatementSyntax newNode = GetNewIfStatement(ifStatement, nestedIf)
                .WithCondition(newCondition)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(ifStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IfStatementSyntax GetNewIfStatement(IfStatementSyntax ifStatement, IfStatementSyntax ifStatement2)
        {
            if (ifStatement.Statement.IsKind(SyntaxKind.Block))
            {
                if (ifStatement2.Statement.IsKind(SyntaxKind.Block))
                {
                    return ifStatement.ReplaceNode(ifStatement2, ((BlockSyntax)ifStatement2.Statement).Statements);
                }
                else
                {
                    return ifStatement.ReplaceNode(ifStatement2, ifStatement2.Statement);
                }
            }
            else
            {
                return ifStatement.ReplaceNode(ifStatement.Statement, ifStatement2.Statement);
            }
        }

        private static IfStatementSyntax GetNestedIfStatement(IfStatementSyntax ifStatement)
        {
            switch (ifStatement.Statement.Kind())
            {
                case SyntaxKind.Block:
                    return (IfStatementSyntax)((BlockSyntax)ifStatement.Statement).Statements[0];
                default:
                    return (IfStatementSyntax)ifStatement.Statement;
            }
        }
    }
}
