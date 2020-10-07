// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReturnCompletedTaskInsteadOfNullCodeFixProvider2))]
    [Shared]
    public class ReturnCompletedTaskInsteadOfNullCodeFixProvider2 : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ReturnCompletedTaskInsteadOfNull); }
        }

        public override FixAllProvider GetFixAllProvider() => null;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindNode(root, context.Span, out ExpressionSyntax expression))
                return;

            if (!(expression.WalkUpParentheses() is ConditionalAccessExpressionSyntax conditionalAccess))
                return;

            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Use completed task",
                ct => RefactorAsync(context.Document, conditionalAccess, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ConditionalAccessExpressionSyntax conditionalAccess,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = conditionalAccess.Expression;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            int position = conditionalAccess.SpanStart;

            string localName = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.Variable, semanticModel, position);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            TypeSyntax type = (typeSymbol?.SupportsExplicitDeclaration() == true)
                ? typeSymbol.ToMinimalTypeSyntax(semanticModel, position)
                : VarType();

            LocalDeclarationStatementSyntax localStatement = LocalDeclarationStatement(
                type,
                Identifier(localName).WithRenameAnnotation(),
                expression);

            ExpressionSyntax newExpression = ReturnCompletedTaskInsteadOfNullCodeFixProvider.CreateCompletedTaskExpression(document, conditionalAccess, semanticModel, cancellationToken);

            IfStatementSyntax ifStatement = IfStatement(
                NotEqualsExpression(IdentifierName(localName), NullLiteralExpression()),
                Block(ReturnStatement(conditionalAccess
                    .WithExpression(IdentifierName(localName).WithTriviaFrom(conditionalAccess.Expression))
                    .RemoveOperatorToken())),
                ElseClause(Block(ReturnStatement(newExpression))));

            SyntaxList<StatementSyntax> statements = List(new StatementSyntax[] { localStatement, ifStatement });

            SyntaxNode parent = conditionalAccess.Parent;

            if (parent is ReturnStatementSyntax returnStatement)
            {
                statements = statements.WithTriviaFrom(returnStatement);

                if (returnStatement.IsEmbedded())
                {
                    return await document.ReplaceNodeAsync(returnStatement, Block(statements), cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return await document.ReplaceNodeAsync(returnStatement, statements, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (parent is SimpleLambdaExpressionSyntax simpleLambda)
            {
                SimpleLambdaExpressionSyntax newNode = simpleLambda
                    .WithBody(Block(statements))
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(simpleLambda, newNode, cancellationToken).ConfigureAwait(false);
            }
            else if (parent is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                ParenthesizedLambdaExpressionSyntax newNode = parenthesizedLambda
                    .WithBody(Block(statements))
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(parenthesizedLambda, newNode, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var arrowExpressionClause = (ArrowExpressionClauseSyntax)parent;

                SyntaxNode node = arrowExpressionClause.Parent;

                SyntaxNode newNode = CreateNewNode(node).WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(node, newNode, cancellationToken).ConfigureAwait(false);
            }

            SyntaxNode CreateNewNode(SyntaxNode node)
            {
                switch (node)
                {
                    case MethodDeclarationSyntax methodDeclaration:
                        {
                            return methodDeclaration
                                .WithExpressionBody(null)
                                .WithBody(Block(statements));
                        }
                    case LocalFunctionStatementSyntax localFunction:
                        {
                            return localFunction
                                .WithExpressionBody(null)
                                .WithBody(Block(statements));
                        }
                    case PropertyDeclarationSyntax propertyDeclaration:
                        {
                            return propertyDeclaration
                                .WithExpressionBody(null)
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithAccessorList(AccessorList(GetAccessorDeclaration(Block(statements))));
                        }
                    case IndexerDeclarationSyntax indexerDeclaration:
                        {
                            return indexerDeclaration
                                .WithExpressionBody(null)
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithAccessorList(AccessorList(GetAccessorDeclaration(Block(statements))));
                        }
                    case AccessorDeclarationSyntax accessorDeclaration:
                        {
                            return accessorDeclaration
                                .WithExpressionBody(null)
                                .WithSemicolonToken(default(SyntaxToken))
                                .WithBody(Block(statements));
                        }
                    default:
                        {
                            Debug.Fail(node.Kind().ToString());
                            return node;
                        }
                }
            }
        }
    }
}
