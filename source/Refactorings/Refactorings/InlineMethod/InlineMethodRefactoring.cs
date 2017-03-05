// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
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

                    if (method != null
                        && !invocation.Ancestors().Any(f => f == method))
                    {
                        ExpressionSyntax expression = GetMethodExpression(method);

                        if (expression != null)
                        {
                            List<ParameterInfo> parameterInfos = GetParameterInfos(invocation, methodSymbol, semanticModel, context.CancellationToken);

                            if (parameterInfos != null)
                            {
                                context.RegisterRefactoring(
                                    "Inline method",
                                    c => InlineMethodAsync(context.Document, invocation, expression, methodSymbol, parameterInfos.ToArray(), semanticModel, c));

                                context.RegisterRefactoring(
                                    "Inline and remove method",
                                    c => InlineAndRemoveMethodAsync(context.Document, invocation, method, expression, methodSymbol, parameterInfos.ToArray(), semanticModel, c));
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
                                        c => InlineMethodAsync(context.Document, expressionStatement, statements, methodSymbol, parameterInfos.ToArray(), semanticModel, c));

                                    context.RegisterRefactoring(
                                        "Inline and remove method",
                                        c => InlineAndRemoveMethodAsync(context.Document, expressionStatement, method, statements, methodSymbol, parameterInfos.ToArray(), semanticModel, c));
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
            var methodSymbol = semanticModel.GetSymbol(invocation, cancellationToken) as IMethodSymbol;

            if (methodSymbol?.Language == LanguageNames.CSharp)
            {
                MethodKind methodKind = methodSymbol.MethodKind;

                if (methodKind == MethodKind.Ordinary)
                {
                    if (methodSymbol.IsStatic)
                    {
                        return methodSymbol;
                    }
                    else
                    {
                        INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(invocation.SpanStart, cancellationToken);

                        if (methodSymbol.ContainingType?.Equals(enclosingType) == true)
                        {
                            ExpressionSyntax expression = invocation.Expression;

                            if (expression != null)
                            {
                                SyntaxKind kind = expression.Kind();

                                if (kind == SyntaxKind.ThisExpression
                                    || kind != SyntaxKind.SimpleMemberAccessExpression)
                                {
                                    return methodSymbol;
                                }
                            }
                        }
                    }
                }
                else if (methodKind == MethodKind.ReducedExtension
                    && invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    return methodSymbol;
                }
            }

            return null;
        }

        private static ExpressionSyntax GetExpression(MethodDeclarationSyntax methodDeclaration)
        {
            BlockSyntax body = methodDeclaration.Body;

            if (body != null)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Count == 1)
                {
                    StatementSyntax statement = statements[0];

                    switch (statement.Kind())
                    {
                        case SyntaxKind.ReturnStatement:
                            return ((ReturnStatementSyntax)statement).Expression;
                        case SyntaxKind.ExpressionStatement:
                            return ((ExpressionStatementSyntax)statement).Expression;
                    }
                }

                return null;
            }

            return methodDeclaration.ExpressionBody?.Expression;
        }

        private static Task<Document> InlineMethodAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            ExpressionSyntax expression,
            IMethodSymbol methodSymbol,
            ParameterInfo[] parameterInfos,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newExpression = RewriteExpression(
                expression,
                methodSymbol,
                parameterInfos,
                semanticModel,
                cancellationToken);

            return document.ReplaceNodeAsync(invocation, newExpression, cancellationToken);
        }

        private static Task<Document> InlineMethodAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            StatementSyntax[] statements,
            IMethodSymbol methodSymbol,
            ParameterInfo[] parameterInfos,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            statements = RewriteStatements(
                statements,
                methodSymbol,
                parameterInfos,
                semanticModel,
                cancellationToken);

            statements[0] = statements[0].WithLeadingTrivia(expressionStatement.GetLeadingTrivia());
            statements[statements.Length - 1] = statements[statements.Length - 1].WithTrailingTrivia(expressionStatement.GetTrailingTrivia());

            StatementContainer container;
            if (StatementContainer.TryCreate(expressionStatement, out container))
            {
                SyntaxNode newNode = container.NodeWithStatements(container.Statements.ReplaceRange(expressionStatement, statements));

                return document.ReplaceNodeAsync(container.Node, newNode, cancellationToken);
            }
            else
            {
                return document.ReplaceNodeAsync(expressionStatement, Block(statements), cancellationToken);
            }
        }

        private static async Task<Solution> InlineAndRemoveMethodAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            MethodDeclarationSyntax methodDeclaration,
            ExpressionSyntax expression,
            IMethodSymbol methodSymbol,
            ParameterInfo[] parameterInfos,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation.SyntaxTree.Equals(methodDeclaration.SyntaxTree))
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

                ExpressionSyntax newExpression = RewriteExpression(expression, methodSymbol, parameterInfos, semanticModel, cancellationToken);

                editor.ReplaceNode(invocation, newExpression);

                editor.RemoveNode(methodDeclaration);

                return editor.GetChangedDocument().Solution();
            }
            else
            {
                Document newDocument = await InlineMethodAsync(document, invocation, expression, methodSymbol, parameterInfos, semanticModel, cancellationToken).ConfigureAwait(false);

                DocumentId documentId = document.Solution().GetDocumentId(methodDeclaration.SyntaxTree);

                newDocument = await Remover.RemoveMemberAsync(newDocument.Solution().GetDocument(documentId), methodDeclaration, cancellationToken).ConfigureAwait(false);

                return newDocument.Solution();
            }
        }

        private static async Task<Solution> InlineAndRemoveMethodAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            MethodDeclarationSyntax methodDeclaration,
            StatementSyntax[] statements,
            IMethodSymbol methodSymbol,
            ParameterInfo[] parameterInfos,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expressionStatement.SyntaxTree.Equals(methodDeclaration.SyntaxTree))
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

                StatementSyntax[] newStatements = RewriteStatements(
                    statements,
                    methodSymbol,
                    parameterInfos,
                    semanticModel,
                    cancellationToken);

                newStatements[0] = newStatements[0].WithLeadingTrivia(expressionStatement.GetLeadingTrivia());
                newStatements[statements.Length - 1] = newStatements[statements.Length - 1].WithTrailingTrivia(expressionStatement.GetTrailingTrivia());

                StatementContainer container;
                if (StatementContainer.TryCreate(expressionStatement, out container))
                {
                    SyntaxNode newNode = container.NodeWithStatements(container.Statements.ReplaceRange(expressionStatement, newStatements));

                    editor.ReplaceNode(container.Node, newNode);
                }
                else
                {
                    editor.ReplaceNode(expressionStatement, Block(newStatements));
                }

                editor.RemoveNode(methodDeclaration);

                return editor.GetChangedDocument().Solution();
            }
            else
            {
                Document newDocument = await InlineMethodAsync(document, expressionStatement, statements, methodSymbol, parameterInfos, semanticModel, cancellationToken).ConfigureAwait(false);

                DocumentId documentId = document.Solution().GetDocumentId(methodDeclaration.SyntaxTree);

                newDocument = await Remover.RemoveMemberAsync(newDocument.Solution().GetDocument(documentId), methodDeclaration, cancellationToken).ConfigureAwait(false);

                return newDocument.Solution();
            }
        }

        private static ExpressionSyntax RewriteExpression(
            ExpressionSyntax expression,
            IMethodSymbol methodSymbol,
            ParameterInfo[] parameterInfos,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = RewriteNode(expression, methodSymbol, parameterInfos, semanticModel, cancellationToken);

            return newExpression
                .WithoutTrivia()
                .Parenthesize()
                .WithSimplifierAnnotation()
                .WithFormatterAnnotation();
        }

        private static StatementSyntax[] RewriteStatements(
            StatementSyntax[] statements,
            IMethodSymbol methodSymbol,
            ParameterInfo[] parameterInfos,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var newStatements = new StatementSyntax[statements.Length];

            for (int i = 0; i < statements.Length; i++)
            {
                StatementSyntax newStatement = RewriteNode(
                    statements[i],
                    methodSymbol,
                    parameterInfos,
                    semanticModel,
                    cancellationToken);

                newStatements[i] = newStatement.WithFormatterAnnotation();
            }

            return newStatements;
        }

        private static TNode RewriteNode<TNode>(
            TNode node,
            IMethodSymbol methodSymbol,
            ParameterInfo[] parameterInfos,
            SemanticModel semanticModel,
            CancellationToken cancellationToken) where TNode : SyntaxNode
        {
            var dic = new Dictionary<IdentifierNameSyntax, ExpressionSyntax>();

            foreach (SyntaxNode descendant in node.DescendantNodes(node.Span))
            {
                if (descendant.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)descendant;

                    ISymbol symbol = semanticModel.GetSymbol(identifierName, cancellationToken);

                    if (symbol != null)
                    {
                        if (symbol.IsParameter())
                        {
                            for (int i = 0; i < parameterInfos.Length; i++)
                            {
                                if (parameterInfos[i].ParameterSymbol.Equals(symbol))
                                {
                                    dic.Add(identifierName, parameterInfos[i].Expression);
                                    break;
                                }
                            }
                        }
                        else if (symbol.IsStatic
                            && !identifierName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                        {
                            INamedTypeSymbol containingType = symbol.ContainingType;

                            if (containingType != null)
                            {
                                if (containingType.Equals(methodSymbol.ContainingType))
                                {
                                    dic.Add(identifierName, CSharpFactory.SimpleMemberAccessExpression(containingType.ToTypeSyntax().WithSimplifierAnnotation(), identifierName));
                                }
                                else
                                {
                                    foreach (INamedTypeSymbol baseType in methodSymbol.ContainingType.BaseTypes())
                                    {
                                        dic.Add(identifierName, CSharpFactory.SimpleMemberAccessExpression(baseType.ToTypeSyntax().WithSimplifierAnnotation(), identifierName));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return node.ReplaceNodes(dic.Keys, (f, g) =>
            {
                ExpressionSyntax expression;
                if (dic.TryGetValue(f, out expression))
                {
                    return expression.Parenthesize(moveTrivia: true).WithSimplifierAnnotation();
                }
                else
                {
                    return g;
                }
            });
        }

        private static List<ParameterInfo> GetParameterInfos(
            InvocationExpressionSyntax invocation,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            List<ParameterInfo> parameterInfos = GetParameterInfos(invocation.ArgumentList, semanticModel, cancellationToken);

            if (parameterInfos != null)
            {
                foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
                {
                    if (!parameterInfos.Any(f => f.ParameterSymbol == parameterSymbol))
                    {
                        if (parameterSymbol.HasExplicitDefaultValue)
                        {
                            parameterInfos.Add(new ParameterInfo(parameterSymbol, parameterSymbol.ToDefaultExpression()));
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                if (methodSymbol.MethodKind == MethodKind.ReducedExtension)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                    var parameterInfo = new ParameterInfo(methodSymbol.ReducedFrom.Parameters[0], memberAccess.Expression);

                    parameterInfos.Add(parameterInfo);
                }
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
                IParameterSymbol parameterSymbol = semanticModel.DetermineParameter(argument, cancellationToken: cancellationToken);

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
