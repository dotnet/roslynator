// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.InlineMethod
{
    internal static class InlineMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (CheckSpan(invocation, context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IMethodSymbol methodSymbol = GetMethodSymbol(invocation, semanticModel, context.CancellationToken);

                if (methodSymbol != null)
                {
                    MethodDeclarationSyntax method = await GetMethodAsync(methodSymbol, context.CancellationToken).ConfigureAwait(false);

                    if (method != null)
                    {
                        ExpressionSyntax expression = GetMethodExpression(method);

                        if (expression != null)
                        {
                            List<ParameterInfo> parameterInfos = GetParameterInfos(invocation, methodSymbol, semanticModel, context.CancellationToken);

                            if (parameterInfos != null)
                            {
                                context.RegisterRefactoring(
                                    "Inline method",
                                    c => InlineMethodAsync(context.Document, invocation, expression, parameterInfos.ToArray(), c));

                                if (!method.Contains(invocation))
                                {
                                    context.RegisterRefactoring(
                                        "Inline and remove method",
                                        c => InlineAndRemoveMethodAsync(context.Document, invocation, method, expression, parameterInfos.ToArray(), c));
                                }
                            }
                        }
                        else if (methodSymbol.ReturnsVoid
                            && invocation.IsParentKind(SyntaxKind.ExpressionStatement))
                        {
                            StatementSyntax[] statements = method.Body?.Statements.ToArray();

                            if (statements?.Length > 0)
                            {
                                List<ParameterInfo> parameterInfos = GetParameterInfos(invocation, methodSymbol, semanticModel, context.CancellationToken);

                                if (parameterInfos != null)
                                {
                                    var expressionStatement = (ExpressionStatementSyntax)invocation.Parent;

                                    context.RegisterRefactoring(
                                        "Inline method",
                                        c => InlineMethodAsync(context.Document, expressionStatement, statements, parameterInfos.ToArray(), c));

                                    if (!method.Contains(invocation))
                                    {
                                        context.RegisterRefactoring(
                                            "Inline and remove method",
                                            c => InlineAndRemoveMethodAsync(context.Document, expressionStatement, method, statements, parameterInfos.ToArray(), c));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckSpan(InvocationExpressionSyntax invocation, TextSpan span)
        {
            ExpressionSyntax expression = invocation.Expression;

            if (expression != null)
            {
                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList != null)
                {
                    if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        return ((MemberAccessExpressionSyntax)expression).Name?.Span.Contains(span) == true;
                    }
                    else if (expression.Span.Contains(span))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static ExpressionSyntax GetMethodExpression(MethodDeclarationSyntax method)
        {
            BlockSyntax body = method.Body;

            if (body != null)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Count == 1)
                {
                    StatementSyntax statement = statements[0];

                    if (statement.IsKind(SyntaxKind.ReturnStatement))
                        return ((ReturnStatementSyntax)statement).Expression;

                    if (statement.IsKind(SyntaxKind.ExpressionStatement))
                        return ((ExpressionStatementSyntax)statement).Expression;
                }

                return null;
            }

            return method.ExpressionBody?.Expression;
        }

        public static async Task<MethodDeclarationSyntax> GetMethodAsync(IMethodSymbol methodSymbol, CancellationToken cancellationToken)
        {
            foreach (SyntaxReference reference in methodSymbol.DeclaringSyntaxReferences)
            {
                SyntaxNode node = await reference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

                if (node.IsKind(SyntaxKind.MethodDeclaration))
                {
                    var method = (MethodDeclarationSyntax)node;

                    if (method.Body != null
                        || method.ExpressionBody != null)
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        public static IMethodSymbol GetMethodSymbol(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol;

            if (symbol?.IsMethod() == true
                && !symbol.IsImplicitlyDeclared)
            {
                var methodSymbol = (IMethodSymbol)symbol;

                if (methodSymbol.IsOrdinary())
                {
                    if (methodSymbol.IsStatic)
                    {
                        return methodSymbol;
                    }
                    else
                    {
                        TypeDeclarationSyntax containingType = invocation.GetContainingType();

                        if (containingType != null)
                        {
                            ISymbol containingTypeSymbol = semanticModel.GetDeclaredSymbol(containingType, cancellationToken);

                            if (methodSymbol.ContainingType?.Equals(containingTypeSymbol) == true)
                                return methodSymbol;
                        }
                    }
                }
                else if (methodSymbol.IsReducedExtension()
                    && invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    return methodSymbol;
                }
            }

            return null;
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

                    if (statement.IsKind(SyntaxKind.ExpressionStatement))
                        return ((ExpressionStatementSyntax)statement).Expression;
                }

                return null;
            }

            return methodDeclaration.ExpressionBody?.Expression;
        }

        private static async Task<Document> InlineMethodAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            ExpressionSyntax expression,
            ParameterInfo[] parameterInfos,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            expression = await ReplaceParameterExpressionWithArgumentExpressionAsync(
                parameterInfos,
                invocation,
                expression,
                document.Project.Solution,
                cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(invocation, expression);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> InlineMethodAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            StatementSyntax[] statements,
            ParameterInfo[] parameterInfos,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            statements = await ReplaceParameterExpressionWithArgumentExpressionAsync(
                parameterInfos,
                statements,
                document.Project.Solution,
                cancellationToken).ConfigureAwait(false);

            statements[0] = statements[0].WithLeadingTrivia(expressionStatement.GetLeadingTrivia());
            statements[statements.Length - 1] = statements[statements.Length - 1].WithTrailingTrivia(expressionStatement.GetTrailingTrivia());

            SyntaxNode newRoot = null;

            if (expressionStatement.Parent?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)expressionStatement.Parent;

                BlockSyntax newBlock = block.WithStatements(block.Statements.ReplaceRange(expressionStatement, statements));

                newRoot = root.ReplaceNode(block, newBlock);
            }
            else
            {
                newRoot = root.ReplaceNode(expressionStatement, Block(statements));
            }

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Solution> InlineAndRemoveMethodAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            MethodDeclarationSyntax methodDeclaration,
            ExpressionSyntax expression,
            ParameterInfo[] parameterInfos,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation.SyntaxTree.Equals(methodDeclaration.SyntaxTree))
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

                Solution solution = document.Project.Solution;

                expression = await ReplaceParameterExpressionWithArgumentExpressionAsync(parameterInfos, invocation, expression, solution, cancellationToken).ConfigureAwait(false);

                editor.ReplaceNode(invocation, expression);

                editor.RemoveNode(methodDeclaration);

                document = editor.GetChangedDocument();

                return document.Project.Solution;
            }
            else
            {
                Solution solution = document.Project.Solution;

                document = await InlineMethodAsync(document, invocation, expression, parameterInfos, cancellationToken).ConfigureAwait(false);

                DocumentId documentId = solution.GetDocumentId(methodDeclaration.SyntaxTree);

                solution = document.Project.Solution;

                return await RemoveMethodAsync(solution.GetDocument(documentId), methodDeclaration, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<Solution> InlineAndRemoveMethodAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            MethodDeclarationSyntax methodDeclaration,
            StatementSyntax[] statements,
            ParameterInfo[] parameterInfos,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expressionStatement.SyntaxTree.Equals(methodDeclaration.SyntaxTree))
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

                Solution solution = document.Project.Solution;

                StatementSyntax[] newStatements = await ReplaceParameterExpressionWithArgumentExpressionAsync(
                    parameterInfos,
                    statements,
                    solution,
                    cancellationToken).ConfigureAwait(false);

                newStatements[0] = newStatements[0].WithLeadingTrivia(expressionStatement.GetLeadingTrivia());
                newStatements[statements.Length - 1] = newStatements[statements.Length - 1].WithTrailingTrivia(expressionStatement.GetTrailingTrivia());

                if (expressionStatement.Parent.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)expressionStatement.Parent;

                    BlockSyntax newBlock = block.WithStatements(block.Statements.ReplaceRange(expressionStatement, newStatements));

                    editor.ReplaceNode(block, newBlock);
                }
                else
                {
                    editor.ReplaceNode(expressionStatement, Block(newStatements));
                }

                editor.RemoveNode(methodDeclaration);

                document = editor.GetChangedDocument();

                return document.Project.Solution;
            }
            else
            {
                Solution solution = document.Project.Solution;

                document = await InlineMethodAsync(document, expressionStatement, statements, parameterInfos, cancellationToken).ConfigureAwait(false);

                DocumentId documentId = solution.GetDocumentId(methodDeclaration.SyntaxTree);

                solution = document.Project.Solution;

                return await RemoveMethodAsync(solution.GetDocument(documentId), methodDeclaration, cancellationToken).ConfigureAwait(false);
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

        private static async Task<ExpressionSyntax> ReplaceParameterExpressionWithArgumentExpressionAsync(
            ParameterInfo[] parameterInfos,
            InvocationExpressionSyntax invocation,
            ExpressionSyntax expression,
            Solution solution,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = await ReplaceParameterExpressionWithArgumentExpressionAsync(parameterInfos, expression, solution, cancellationToken).ConfigureAwait(false);

            if (!SyntaxUtility.AreParenthesesRedundantOrInvalid(invocation))
            {
                newExpression = newExpression
                   .WithoutTrivia()
                   .Parenthesize();
            }

            return newExpression
                .WithTriviaFrom(invocation)
                .WithFormatterAnnotation();
        }

        private static async Task<StatementSyntax[]> ReplaceParameterExpressionWithArgumentExpressionAsync(
            ParameterInfo[] parameterInfos,
            StatementSyntax[] statements,
            Solution solution,
            CancellationToken cancellationToken)
        {
            var newStatements = new List<StatementSyntax>();

            for (int i = 0; i < statements.Length; i++)
            {
                StatementSyntax newStatement = await ReplaceParameterExpressionWithArgumentExpressionAsync(
                    parameterInfos,
                    statements[i],
                    solution,
                    cancellationToken).ConfigureAwait(false);

                newStatements.Add(newStatement.WithFormatterAnnotation());
            }

            return newStatements.ToArray();
        }

        private static async Task<TNode> ReplaceParameterExpressionWithArgumentExpressionAsync<TNode>(
            ParameterInfo[] parameterInfos,
            TNode node,
            Solution solution,
            CancellationToken cancellationToken) where TNode : SyntaxNode
        {
            var dic = new Dictionary<IdentifierNameSyntax, ExpressionSyntax>();

            TextSpan expressionSpan = node.Span;

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(parameterInfo.ParameterSymbol, solution, cancellationToken).ConfigureAwait(false);

                foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
                {
                    foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                    {
                        if (!referenceLocation.IsImplicit)
                        {
                            TextSpan span = referenceLocation.Location.SourceSpan;

                            if (expressionSpan.Contains(span))
                            {
                                IdentifierNameSyntax identifierName = node
                                   .FindNode(span, getInnermostNodeForTie: true)
                                   .FirstAncestorOrSelf<IdentifierNameSyntax>();

                                dic.Add(identifierName, parameterInfo.Expression);
                            }
                        }
                    }
                }
            }

            var rewriter = new IdentifierNameSyntaxRewriter(dic);

            return (TNode)rewriter.Visit(node);
        }

        private static List<ParameterInfo> GetParameterInfos(
            InvocationExpressionSyntax invocation,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            List<ParameterInfo> parameterInfos = GetParameterInfos(invocation.ArgumentList, semanticModel, cancellationToken);

            if (parameterInfos != null
                && methodSymbol.IsReducedExtension())
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                var parameterInfo = new ParameterInfo(methodSymbol.ReducedFrom.Parameters[0], memberAccess.Expression);

                parameterInfos.Add(parameterInfo);
            }

            return parameterInfos;
        }

        private static List<ParameterInfo> GetParameterInfos(
            ArgumentListSyntax argumentList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var list = new List<ParameterInfo>();

            foreach (ArgumentSyntax argument in argumentList.Arguments)
            {
                IParameterSymbol parameterSymbol = argument.DetermineParameter(semanticModel, cancellationToken: cancellationToken);

                if (parameterSymbol != null)
                {
                    list.Add(new ParameterInfo(parameterSymbol, argument.Expression));
                }
                else
                {
                    return null;
                }
            }

            return list;
        }
    }
}
