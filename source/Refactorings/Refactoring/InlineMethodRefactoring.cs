// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class InlineMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol;

            if (symbol?.IsMethod() == true
                && !symbol.IsImplicitlyDeclared)
            {
                ExpressionSyntax expression = null;

                foreach (SyntaxReference reference in symbol.DeclaringSyntaxReferences)
                {
                    SyntaxNode node = await reference.GetSyntaxAsync(context.CancellationToken);

                    if (node.IsKind(SyntaxKind.MethodDeclaration))
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        var methodSymbol = (IMethodSymbol)symbol;

                        if (!methodSymbol.ReturnsVoid || invocation.Parent.IsKind(SyntaxKind.ExpressionStatement))
                        {
                            expression = GetExpression(methodDeclaration);

                            if (expression != null)
                            {
                                context.RegisterRefactoring(
                                    "Inline method",
                                    cancellationToken =>
                                    {
                                        return InlineAsync(
                                            context.Document,
                                            invocation,
                                            expression,
                                            cancellationToken);
                                    });

                                if (!methodDeclaration.Contains(invocation))
                                {
                                    context.RegisterRefactoring(
                                        "Inline and remove method",
                                        cancellationToken =>
                                        {
                                            return InlineAndRemoveAsync(
                                                context.Document,
                                                invocation,
                                                expression,
                                                methodDeclaration,
                                                cancellationToken);
                                        });
                                }

                                return;
                            }
                        }
                    }
                }
            }
        }

        private static ExpressionSyntax GetExpression(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.Body != null)
            {
                SyntaxList<StatementSyntax> statements = methodDeclaration.Body.Statements;

                if (statements.Count == 1)
                {
                    StatementSyntax statement = statements[0];

                    if (statement.IsKind(SyntaxKind.ReturnStatement))
                        return ((ReturnStatementSyntax)statement).Expression;
                }
            }

            return methodDeclaration.ExpressionBody?.Expression;
        }

        private static async Task<Document> InlineAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            root = root.ReplaceNode(invocation, expression);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Solution> InlineAndRemoveAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            ExpressionSyntax expression,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation.SyntaxTree.Equals(methodDeclaration.SyntaxTree))
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken);

                editor.ReplaceNode(invocation, expression);

                editor.RemoveNode(methodDeclaration);

                document = editor.GetChangedDocument();

                return document.Project.Solution;
            }
            else
            {
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode newRoot = root.ReplaceNode(invocation, expression);

                document = document.WithSyntaxRoot(newRoot);

                Solution solution = document.Project.Solution;

                DocumentId documentId = solution.GetDocumentId(methodDeclaration.SyntaxTree);

                solution = document.Project.Solution;

                return await RemoveMethodAsync(solution.GetDocument(documentId), methodDeclaration, cancellationToken);
            }
        }

        private static async Task<Solution> RemoveMethodAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            root = root.RemoveNode(methodDeclaration, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            document = document.WithSyntaxRoot(root);

            return document.Project.Solution;
        }
    }
}
