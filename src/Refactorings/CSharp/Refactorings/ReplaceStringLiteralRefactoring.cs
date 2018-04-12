// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpTypeFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceStringLiteralRefactoring
    {
        private const string Quote = "\"";
        private const string AmpersandQuote = "@" + Quote;
        private const string Backslash = @"\";

        public static Task<Document> ReplaceWithInterpolatedStringAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            int interpolationStartIndex = -1,
            int interpolationLength = 0,
            bool addNameOf = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = literalExpression.Token.Text;

            StringBuilder sb = StringBuilderCache.GetInstance();

            sb.Append('$');

            int length = sb.Length;
            sb.Append(s, 0, interpolationStartIndex);
            sb.Replace("{", "{{", length);
            sb.Replace("}", "}}", length);

            sb.Append('{');

            if (addNameOf)
            {
                sb.Append(
                    NameOfExpression(
                        StringLiteralParser.Parse(
                            s,
                            interpolationStartIndex,
                            interpolationLength,
                            isVerbatim: s.StartsWith("@"),
                            isInterpolatedText: false)));
            }

            int closeBracePosition = sb.Length;

            sb.Append('}');

            length = sb.Length;

            int startIndex = interpolationStartIndex + interpolationLength;
            sb.Append(s, startIndex, s.Length - startIndex);

            sb.Replace("{", "{{", length);
            sb.Replace("}", "}}", length);

            ExpressionSyntax newNode = ParseExpression(StringBuilderCache.GetStringAndFree(sb));

            SyntaxToken closeBrace = newNode.FindToken(closeBracePosition);

            newNode = newNode
                .ReplaceToken(closeBrace, closeBrace.WithNavigationAnnotation())
                .WithTriviaFrom(literalExpression);

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        public static bool CanReplaceWithStringEmpty(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 0;
        }

        public static Task<Document> ReplaceWithStringEmptyAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberAccessExpressionSyntax newNode = SimpleMemberAccessExpression(
                    StringType(),
                    IdentifierName("Empty"))
                .WithTriviaFrom(literalExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        public static Task<Document> ReplaceWithRegularStringLiteralAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = CreateRegularStringLiteral(literalExpression.Token.ValueText);

            LiteralExpressionSyntax newNode = ParseRegularStringLiteral(s)
                .WithTriviaFrom(literalExpression);

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        public static Task<Document> ReplaceWithRegularStringLiteralsAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BinaryExpressionSyntax newNode = CreateAddExpression(literalExpression.Token.ValueText)
                .WithTriviaFrom(literalExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        public static Task<Document> ReplaceWithVerbatimStringLiteralAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = literalExpression.Token.ValueText;

            s = s.Replace(Quote, Quote + Quote);

            var newNode = (LiteralExpressionSyntax)ParseExpression(AmpersandQuote + s + Quote)
                .WithTriviaFrom(literalExpression);

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        private static string CreateRegularStringLiteral(string text)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '"':
                        {
                            sb.Append(Backslash);
                            sb.Append(Quote);
                            break;
                        }
                    case '\\':
                        {
                            sb.Append(Backslash);
                            sb.Append(Backslash);
                            break;
                        }
                    case '\r':
                        {
                            sb.Append(@"\r");
                            break;
                        }
                    case '\n':
                        {
                            sb.Append(@"\n");
                            break;
                        }
                    default:
                        {
                            sb.Append(text[i]);
                            break;
                        }
                }
            }

            return StringBuilderCache.GetStringAndFree(sb);
        }

        private static List<string> CreateRegularStringLiterals(string text)
        {
            var values = new List<string>();

            StringBuilder sb = StringBuilderCache.GetInstance();

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '"':
                        {
                            sb.Append(Backslash);
                            sb.Append(Quote);
                            break;
                        }
                    case '\\':
                        {
                            sb.Append(Backslash);
                            sb.Append(Backslash);
                            break;
                        }
                    case '\r':
                        {
                            if (i < text.Length - 1
                                && text[i + 1] == '\n')
                            {
                                i++;
                            }

                            values.Add(sb.ToString());
                            sb.Clear();
                            break;
                        }
                    case '\n':
                        {
                            values.Add(sb.ToString());
                            sb.Clear();
                            break;
                        }
                    default:
                        {
                            sb.Append(text[i]);
                            break;
                        }
                }
            }

            values.Add(StringBuilderCache.GetStringAndFree(sb));

            return values;
        }

        private static BinaryExpressionSyntax CreateAddExpression(string text)
        {
            List<string> values = CreateRegularStringLiterals(text);

            BinaryExpressionSyntax binaryExpression = CreateAddExpression(
                ParseRegularStringLiteral(values[0]),
                ParseRegularStringLiteral(values[1]));

            for (int i = 2; i < values.Count; i++)
            {
                binaryExpression = CreateAddExpression(
                    binaryExpression,
                    ParseRegularStringLiteral(values[i]));
            }

            return binaryExpression;
        }

        private static BinaryExpressionSyntax CreateAddExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AddExpression(
                left,
                Token(SyntaxTriviaList.Empty, SyntaxKind.PlusToken, ParseTrailingTrivia("\r\n")),
                right);
        }

        private static LiteralExpressionSyntax ParseRegularStringLiteral(string text)
        {
            return (LiteralExpressionSyntax)ParseExpression(Quote + text + Quote);
        }
    }
}
