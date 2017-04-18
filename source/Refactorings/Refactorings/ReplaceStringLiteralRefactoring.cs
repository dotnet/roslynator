// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = literalExpression.Token.Text;

            if (interpolationStartIndex != -1)
            {
                s = StringUtility.DoubleBraces(s.Substring(0, interpolationStartIndex)) +
                   "{" +
                   s.Substring(interpolationStartIndex, interpolationLength) +
                   "}" +
                   StringUtility.DoubleBraces(s.Substring(interpolationStartIndex + interpolationLength));
            }

            var interpolatedString = (InterpolatedStringExpressionSyntax)ParseExpression("$" + s)
                .WithTriviaFrom(literalExpression);

            return document.ReplaceNodeAsync(literalExpression, interpolatedString, cancellationToken);
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
            var sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '"':
                        {
                            sb.Append(Backslash + Quote);
                            break;
                        }
                    case '\\':
                        {
                            sb.Append(Backslash + Backslash);
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

            return sb.ToString();
        }

        private static List<string> CreateRegularStringLiterals(string text)
        {
            var values = new List<string>();

            var sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '"':
                        {
                            sb.Append(Backslash + Quote);
                            break;
                        }
                    case '\\':
                        {
                            sb.Append(Backslash + Backslash);
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

            values.Add(sb.ToString());

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
