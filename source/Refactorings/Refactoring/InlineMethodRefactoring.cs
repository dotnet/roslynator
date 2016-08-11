// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class InlineMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (CheckSpan(invocation, context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IMethodSymbol methodSymbol = GetMethodSymbol(invocation, semanticModel, context.CancellationToken);

                MethodDeclarationSyntax method = await GetMethodAsync(methodSymbol, context.CancellationToken);

                if (method != null)
                {
                    ExpressionSyntax expression = GetMethodExpression(method, context.CancellationToken);

                    if (expression != null)
                    {
                        List<ParameterInfo> parameterInfos = GetParameterInfos(invocation.ArgumentList, semanticModel, context.CancellationToken);

                        if (parameterInfos != null)
                        {
                            if (methodSymbol.IsReducedExtension())
                            {
                                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                                parameterInfos.Add(new ParameterInfo(memberAccess.Expression, methodSymbol.ReducedFrom.Parameters[0]));
                            }

                            context.RegisterRefactoring(
                                "Inline method",
                                c => InlineAsync(context.Document, invocation, expression, parameterInfos, c));

                            if (!method.Contains(invocation))
                            {
                                context.RegisterRefactoring(
                                    "Inline and remove method",
                                    c => InlineAndRemoveAsync(context.Document, invocation, method, expression, parameterInfos, c));
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
                    if (invocation.Span.Equals(span))
                    {
                        return true;
                    }
                    else if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        SimpleNameSyntax name = memberAccess.Name;

                        if (name != null)
                        {
                            return span.Start == name.Span.Start
                                && span.End == invocation.Span.End;
                        }
                    }
                    else if (span.Start == expression.Span.Start
                        && span.End == invocation.Span.End)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static ExpressionSyntax GetMethodExpression(MethodDeclarationSyntax method, CancellationToken cancellationToken)
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
                SyntaxNode node = await reference.GetSyntaxAsync(cancellationToken);

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
                        return methodSymbol;
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

        private static async Task<Document> InlineAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            ExpressionSyntax expression,
            List<ParameterInfo> parameterInfos,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            Solution solution = document.Project.Solution;

            expression = await RewriteExpressionAsync(parameterInfos, expression, solution, cancellationToken);

            root = root.ReplaceNode(invocation, expression);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Solution> InlineAndRemoveAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            MethodDeclarationSyntax methodDeclaration,
            ExpressionSyntax expression,
            List<ParameterInfo> parameterInfos,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation.SyntaxTree.Equals(methodDeclaration.SyntaxTree))
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken);

                Solution solution = document.Project.Solution;

                expression = await RewriteExpressionAsync(parameterInfos, expression, solution, cancellationToken);

                editor.ReplaceNode(invocation, expression);

                editor.RemoveNode(methodDeclaration);

                document = editor.GetChangedDocument();

                return document.Project.Solution;
            }
            else
            {
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                Solution solution = document.Project.Solution;

                expression = await RewriteExpressionAsync(parameterInfos, expression, solution, cancellationToken);

                root = root.ReplaceNode(invocation, expression);

                document = document.WithSyntaxRoot(root);

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

        private static async Task<ExpressionSyntax> RewriteExpressionAsync(
            List<ParameterInfo> parameterInfos,
            ExpressionSyntax expression,
            Solution solution,
            CancellationToken cancellationToken)
        {
            var dic = new Dictionary<IdentifierNameSyntax, ExpressionSyntax>();

            TextSpan expressionSpan = expression.Span;

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(parameterInfo.ParameterSymbol, solution, cancellationToken);

                foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
                {
                    foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                    {
                        if (!referenceLocation.IsImplicit)
                        {
                            TextSpan span = referenceLocation.Location.SourceSpan;

                            if (expressionSpan.Contains(span))
                            {
                                IdentifierNameSyntax identifierName = expression
                                   .FindNode(span, getInnermostNodeForTie: true)
                                   .FirstAncestorOrSelf<IdentifierNameSyntax>();

                                dic.Add(identifierName, parameterInfo.Expression);
                            }
                        }
                    }
                }
            }

            var rewriter = new IdentifierNameSyntaxRewriter(dic);

            expression = (ExpressionSyntax)rewriter.Visit(expression);

            expression = WrapInParenthesesIfNecessary(expression);

            return expression.WithFormatterAnnotation();
        }

        private static ExpressionSyntax WrapInParenthesesIfNecessary(ExpressionSyntax expression)
        {
            switch (expression.Parent?.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    return expression;
            }

            if (!expression.IsKind(SyntaxKind.ParenthesizedExpression))
                expression = SyntaxFactory.ParenthesizedExpression(expression);

            return expression;
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
                    list.Add(new ParameterInfo(argument.Expression, parameterSymbol));
                }
                else
                {
                    return null;
                }
            }

            return list;
        }

        private class IdentifierNameSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly Dictionary<IdentifierNameSyntax, ExpressionSyntax> _dic;

            public IdentifierNameSyntaxRewriter(Dictionary<IdentifierNameSyntax, ExpressionSyntax> dic)
            {
                _dic = dic;
            }

            public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            {
                ExpressionSyntax expression;

                if (_dic.TryGetValue(node, out expression))
                {
                    _dic.Remove(node);

                    return expression;
                }
                else
                {
                    return base.VisitIdentifierName(node);
                }
            }
        }

        private struct ParameterInfo
        {
            public ParameterInfo(ExpressionSyntax expression, IParameterSymbol parameterSymbol)
            {
                Expression = expression;
                ParameterSymbol = parameterSymbol;
            }

            public ExpressionSyntax Expression { get; }

            public IParameterSymbol ParameterSymbol { get; }
        }
    }
}
