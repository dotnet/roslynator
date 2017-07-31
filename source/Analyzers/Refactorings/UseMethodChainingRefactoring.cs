// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseMethodChainingRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpression memberInvocation)
        {
            InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;

            if (invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement)
                && !invocationExpression.SpanOrTrailingTriviaContainsDirectives())
            {
                StatementSyntax expressionStatement = (ExpressionStatementSyntax)invocationExpression.Parent;

                StatementContainer container;
                if (StatementContainer.TryCreate(expressionStatement, out container))
                {
                    SyntaxList<StatementSyntax> statements = container.Statements;

                    if (statements.Count > 1)
                    {
                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        INamedTypeSymbol symbol = context.GetTypeByMetadataName(MetadataNames.System_Text_StringBuilder);

                        if (symbol != null
                            && IsFixable(memberInvocation, symbol, semanticModel, cancellationToken))
                        {
                            ExpressionSyntax expression = GetFirstInvocationInMethodChain(memberInvocation, symbol, semanticModel, cancellationToken).Expression;

                            int i = statements.IndexOf(expressionStatement);

                            if (i == 0
                                || !IsFixable(statements[i - 1], expression, symbol, semanticModel, cancellationToken))
                            {
                                int j = i;
                                while (j < statements.Count - 1)
                                {
                                    if (!IsFixable(statements[j + 1], expression, symbol, semanticModel, cancellationToken))
                                        break;

                                    j++;
                                }

                                if (j > i)
                                {
                                    context.ReportDiagnostic(DiagnosticDescriptors.UseMethodChaining, expressionStatement);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsFixable(
            StatementSyntax statement,
            ExpressionSyntax expression,
            INamedTypeSymbol stringBuilderSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (statement.IsKind(SyntaxKind.ExpressionStatement)
                && !statement.SpanOrLeadingTriviaContainsDirectives())
            {
                var expressionStatement = (ExpressionStatementSyntax)statement;

                MemberInvocationExpression memberInvocation;

                return MemberInvocationExpression.TryCreate(expressionStatement.Expression, out memberInvocation)
                    && IsFixable(memberInvocation, stringBuilderSymbol, semanticModel, cancellationToken)
                    && SyntaxComparer.AreEquivalent(
                        expression,
                        GetFirstInvocationInMethodChain(memberInvocation, stringBuilderSymbol, semanticModel, cancellationToken).Expression,
                        requireNotNull: true);
            }

            return false;
        }

        private static bool IsFixable(
            MemberInvocationExpression memberInvocation,
            INamedTypeSymbol stringBuilderSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (SupportsMethodChaining(memberInvocation.NameText))
            {
                MethodInfo methodInfo;

                return semanticModel.TryGetMethodInfo(memberInvocation.InvocationExpression, out methodInfo, cancellationToken)
                    && !methodInfo.IsExtensionMethod
                    && !methodInfo.IsGenericMethod
                    && !methodInfo.IsStatic
                    && methodInfo.ContainingType?.Equals(stringBuilderSymbol) == true
                    && methodInfo.ReturnType.Equals(stringBuilderSymbol);
            }

            return false;
        }

        private static MemberInvocationExpression GetFirstInvocationInMethodChain(
            MemberInvocationExpression memberInvocation,
            INamedTypeSymbol stringBuilderSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            MemberInvocationExpression memberInvocation2;
            while (MemberInvocationExpression.TryCreate(memberInvocation.Expression, out memberInvocation2)
                && IsFixable(memberInvocation2, stringBuilderSymbol, semanticModel, cancellationToken))
            {
                memberInvocation = memberInvocation2;
            }

            return memberInvocation;
        }

        private static bool SupportsMethodChaining(string methodName)
        {
            switch (methodName)
            {
                case "Append":
                case "AppendFormat":
                case "AppendLine":
                case "Clear":
                case "Insert":
                case "Remove":
                case "Replace":
                    {
                        return true;
                    }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol symbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Text_StringBuilder);

            var invocationExpression = (InvocationExpressionSyntax)expressionStatement.Expression;

            MemberInvocationExpression memberInvocation = MemberInvocationExpression.Create(invocationExpression);

            ExpressionSyntax expression = GetFirstInvocationInMethodChain(memberInvocation, symbol, semanticModel, cancellationToken).Expression;

            StatementContainer statementContainer = StatementContainer.Create(expressionStatement);

            SyntaxList<StatementSyntax> statements = statementContainer.Statements;

            int index = statements.IndexOf(expressionStatement);

            string indentation = CSharpFormatter.GetIncreasedIndentation(expressionStatement, cancellationToken).ToString();

            var sb = new StringBuilder(invocationExpression.ToString());

            int j = index;
            while (j < statements.Count - 1)
            {
                StatementSyntax statement = statements[j + 1];

                if (!IsFixable(statement, expression, symbol, semanticModel, cancellationToken))
                    break;

                sb.AppendLine();
                sb.Append(indentation);
                sb.Append(GetTextToAppend((ExpressionStatementSyntax)statement, symbol, semanticModel, cancellationToken));

                j++;
            }

            StatementSyntax lastStatement = statements[j];

            SyntaxList<StatementSyntax> newStatements = statements;

            while (j > index)
            {
                newStatements = newStatements.RemoveAt(j);
                j--;
            }

            ExpressionSyntax newInvocationExpression = SyntaxFactory.ParseExpression(sb.ToString());

            SyntaxTriviaList trailingTrivia = statementContainer
                .Node
                .DescendantTrivia(TextSpan.FromBounds(invocationExpression.Span.End, lastStatement.Span.End))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace()
                .AddRange(lastStatement.GetTrailingTrivia());

            ExpressionStatementSyntax newExpressionStatement = expressionStatement
                .WithExpression(newInvocationExpression)
                .WithLeadingTrivia(expressionStatement.GetLeadingTrivia())
                .WithTrailingTrivia(trailingTrivia)
                .WithFormatterAndSimplifierAnnotations();

            newStatements = newStatements.ReplaceAt(index, newExpressionStatement);

            return await document.ReplaceNodeAsync(statementContainer.Node, statementContainer.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
        }

        private static string GetTextToAppend(
            ExpressionStatementSyntax expressionStatement,
            INamedTypeSymbol stringBuilderSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            MemberInvocationExpression memberInvocation = MemberInvocationExpression.Create((InvocationExpressionSyntax)expressionStatement.Expression);

            MemberInvocationExpression firstMemberInvocation = GetFirstInvocationInMethodChain(memberInvocation, stringBuilderSymbol, semanticModel, cancellationToken);

            InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;

            return invocationExpression
                .ToString()
                .Substring(firstMemberInvocation.OperatorToken.SpanStart - invocationExpression.SpanStart);
        }
    }
}