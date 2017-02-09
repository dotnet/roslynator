// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Formatting
{
    internal static class CSharpFormatter
    {
        public static async Task<Document> ToSingleLineAsync<TNode>(
            Document document,
            TNode condition,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            TNode newNode = ToSingleLine(condition);

            return await document.ReplaceNodeAsync(condition, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static TNode ToSingleLine<TNode>(
            TNode node,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            return Remover.RemoveWhitespaceOrEndOfLine(node, node.Span)
                .WithFormatterAnnotation();
        }

        public static async Task<Document> ToSingleLineAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            InitializerExpressionSyntax newInitializer = ToSingleLine(initializer, cancellationToken);

            SyntaxNode parent = initializer.Parent;
            SyntaxNode newParent = parent;

            switch (parent.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                    {
                        var expression = (ObjectCreationExpressionSyntax)parent;

                        expression = expression.WithInitializer(newInitializer);

                        ArgumentListSyntax argumentList = expression.ArgumentList;

                        if (argumentList != null)
                        {
                            newParent = expression.WithArgumentList(argumentList.WithoutTrailingTrivia());
                        }
                        else
                        {
                            newParent = expression.WithType(expression.Type.WithoutTrailingTrivia());
                        }

                        break;
                    }
                case SyntaxKind.ArrayCreationExpression:
                    {
                        var expression = (ArrayCreationExpressionSyntax)parent;

                        newParent = expression
                            .WithInitializer(newInitializer)
                            .WithType(expression.Type.WithoutTrailingTrivia());

                        break;
                    }
                case SyntaxKind.ImplicitArrayCreationExpression:
                    {
                        var expression = (ImplicitArrayCreationExpressionSyntax)parent;

                        newParent = expression
                            .WithInitializer(newInitializer)
                            .WithCloseBracketToken(expression.CloseBracketToken.WithoutTrailingTrivia());

                        break;
                    }
                default:
                    {
                        Debug.Assert(false, parent.Kind().ToString());

                        return await document.ReplaceNodeAsync(initializer, newInitializer, cancellationToken).ConfigureAwait(false);
                    }
            }

            return await document.ReplaceNodeAsync(parent, newParent, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ConditionalExpressionSyntax newNode = ToMultiLine(conditionalExpression, cancellationToken);

            return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static ConditionalExpressionSyntax ToMultiLine(ConditionalExpressionSyntax conditionalExpression, CancellationToken cancellationToken = default(CancellationToken))
        {
            string indent = GetIncreasedLineIndent(conditionalExpression.Parent, cancellationToken);

            SyntaxTriviaList leadingTrivia = ParseLeadingTrivia(Environment.NewLine + indent);

            return ConditionalExpression(
                    conditionalExpression.Condition.WithoutTrailingTrivia(),
                    Token(leadingTrivia, SyntaxKind.QuestionToken, TriviaList(Space)),
                    conditionalExpression.WhenTrue.WithoutTrailingTrivia(),
                    Token(leadingTrivia, SyntaxKind.ColonToken, TriviaList(Space)),
                    conditionalExpression.WhenFalse.WithoutTrailingTrivia());
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            ParameterListSyntax parameterList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParameterListSyntax newNode = ToMultiLine(parameterList);

            return await document.ReplaceNodeAsync(parameterList, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static ParameterListSyntax ToMultiLine(ParameterListSyntax parameterList, CancellationToken cancellationToken = default(CancellationToken))
        {
            string indent = GetIncreasedLineIndent(parameterList.Parent, cancellationToken);

            SyntaxTriviaList trivia = ParseLeadingTrivia(indent);

            var nodesAndTokens = new List<SyntaxNodeOrToken>();

            SeparatedSyntaxList<ParameterSyntax>.Enumerator en = parameterList.Parameters.GetEnumerator();

            if (en.MoveNext())
            {
                nodesAndTokens.Add(en.Current.WithLeadingTrivia(trivia));

                while (en.MoveNext())
                {
                    nodesAndTokens.Add(CommaToken().WithTrailingNewLine());

                    nodesAndTokens.Add(en.Current.WithLeadingTrivia(trivia));
                }
            }

            return ParameterList(
                OpenParenToken().WithTrailingNewLine(),
                SeparatedList<ParameterSyntax>(nodesAndTokens),
                parameterList.CloseParenToken);
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            InitializerExpressionSyntax newNode = ToMultiLine(initializer, cancellationToken)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(initializer, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static InitializerExpressionSyntax ToMultiLine(InitializerExpressionSyntax initializer, CancellationToken cancellationToken)
        {
            SyntaxNode parent = initializer.Parent;

            if (parent.IsKind(SyntaxKind.ObjectCreationExpression)
                && !initializer.IsKind(SyntaxKind.CollectionInitializerExpression))
            {
                return initializer
                    .WithExpressions(
                        SeparatedList(
                            initializer.Expressions.Select(expression => expression.WithLeadingTrivia(NewLineTrivia()))));
            }
            else
            {
                string indent = GetLineIndent(initializer, cancellationToken);

                SyntaxTriviaList braceTrivia = ParseLeadingTrivia(Environment.NewLine + indent);
                SyntaxTriviaList expressionTrivia = ParseLeadingTrivia(Environment.NewLine + IncreaseIndent(indent));

                return initializer
                    .WithExpressions(
                        SeparatedList(
                            initializer.Expressions.Select(expression => expression.WithLeadingTrivia(expressionTrivia))))
                    .WithOpenBraceToken(initializer.OpenBraceToken.WithLeadingTrivia(braceTrivia))
                    .WithCloseBraceToken(initializer.CloseBraceToken.WithLeadingTrivia(braceTrivia));
            }
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ArgumentListSyntax newNode = ToMultiLine(argumentList);

            return await document.ReplaceNodeAsync(argumentList, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static ArgumentListSyntax ToMultiLine(ArgumentListSyntax argumentList, CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = GetIncreasedLineIndent(argumentList.Parent, cancellationToken);

            SyntaxTriviaList leadingTrivia = ParseLeadingTrivia(s);

            var nodesAndTokens = new List<SyntaxNodeOrToken>();

            SeparatedSyntaxList<ArgumentSyntax>.Enumerator en = argumentList.Arguments.GetEnumerator();

            if (en.MoveNext())
            {
                nodesAndTokens.Add(en.Current
                    .TrimTrailingTrivia()
                    .WithLeadingTrivia(leadingTrivia));

                while (en.MoveNext())
                {
                    nodesAndTokens.Add(CommaToken().WithTrailingNewLine());

                    nodesAndTokens.Add(en.Current
                        .TrimTrailingTrivia()
                        .WithLeadingTrivia(leadingTrivia));
                }
            }

            return ArgumentList(
                OpenParenToken().WithTrailingNewLine(),
                SeparatedList<ArgumentSyntax>(nodesAndTokens),
                argumentList.CloseParenToken.WithoutLeadingTrivia());
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            MemberAccessExpressionSyntax[] expressions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberAccessExpressionSyntax expression = expressions[0];

            string indent = GetIncreasedLineIndent(expression, cancellationToken);

            SyntaxTriviaList triviaList = ParseLeadingTrivia(Environment.NewLine + indent);

            MemberAccessExpressionSyntax newNode = expression.ReplaceNodes(expressions, (node, node2) =>
            {
                SyntaxToken operatorToken = node.OperatorToken;

                if (!operatorToken.HasLeadingTrivia)
                {
                    return node2.WithOperatorToken(operatorToken.WithLeadingTrivia(triviaList));
                }
                else
                {
                    return node2;
                }
            });

            newNode = newNode.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(expression, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            AttributeArgumentListSyntax argumentList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            AttributeArgumentListSyntax newNode = ToMultiLine(argumentList, cancellationToken);

            return await document.ReplaceNodeAsync(argumentList, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static AttributeArgumentListSyntax ToMultiLine(AttributeArgumentListSyntax argumentList, CancellationToken cancellationToken = default(CancellationToken))
        {
            string indent = GetIncreasedLineIndent(argumentList.Parent, cancellationToken);

            SyntaxTriviaList leadingTrivia = ParseLeadingTrivia(indent);

            var nodesAndTokens = new List<SyntaxNodeOrToken>();

            SeparatedSyntaxList<AttributeArgumentSyntax>.Enumerator en = argumentList.Arguments.GetEnumerator();

            if (en.MoveNext())
            {
                nodesAndTokens.Add(en.Current
                    .TrimTrailingTrivia()
                    .WithLeadingTrivia(leadingTrivia));

                while (en.MoveNext())
                {
                    nodesAndTokens.Add(CommaToken().WithTrailingNewLine());

                    nodesAndTokens.Add(en.Current
                        .TrimTrailingTrivia()
                        .WithLeadingTrivia(leadingTrivia));
                }
            }

            return AttributeArgumentList(
                OpenParenToken().WithTrailingNewLine(),
                SeparatedList<AttributeArgumentSyntax>(nodesAndTokens),
                argumentList.CloseParenToken.WithoutLeadingTrivia());
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            BinaryExpressionSyntax condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string indent = GetIncreasedLineIndent(condition, cancellationToken);

            SyntaxTriviaList triviaList = ParseLeadingTrivia(Environment.NewLine + indent);

            var rewriter = new BinaryExpressionToMultiLineRewriter(triviaList);

            var newCondition = (ExpressionSyntax)rewriter.Visit(condition);

            return await document.ReplaceNodeAsync(condition, newCondition, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> ToMultiLineAsync(
            Document document,
            AccessorDeclarationSyntax accessor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            AccessorDeclarationSyntax newAccessor = ToMultiLine(accessor);

            return await document.ReplaceNodeAsync(accessor, newAccessor, cancellationToken).ConfigureAwait(false);
        }

        private static AccessorDeclarationSyntax ToMultiLine(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;
            SyntaxToken closeBrace = body.CloseBraceToken;

            AccessorDeclarationSyntax newAccessor = accessor
                .WithBody(
                    body.WithCloseBraceToken(
                        closeBrace.WithLeadingTrivia(
                            closeBrace.LeadingTrivia.Add(NewLineTrivia()))));

            return newAccessor.WithFormatterAnnotation();
        }

        private static string GetLineIndent(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTree syntaxTree = node.SyntaxTree;

            if (syntaxTree != null)
            {
                SourceText sourceText = syntaxTree.GetText(cancellationToken);

                int lineNumber = node.GetSpanStartLine(cancellationToken);
                TextLine line = sourceText.Lines[lineNumber];
                string s = line.ToString();

                int i = 0;
                while (i < s.Length
                    && char.IsWhiteSpace(s[i]))
                {
                    i++;
                }

                return s.Substring(0, i);
            }

            return string.Empty;
        }

        private static string GetIncreasedLineIndent(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = GetLineIndent(node, cancellationToken);

            return IncreaseIndent(s);
        }

        private static string IncreaseIndent(string s)
        {
            if (s.Length > 0 && ContainsOnlyTab(s))
            {
                return s + '\t';
            }
            else
            {
                return s + new string(' ', 4);
            }
        }

        private static bool ContainsOnlyTab(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '\t')
                    return false;
            }

            return true;
        }
    }
}
