// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceStringLiteralRefactoring
    {
        private const string Quote = "\"";
        private const string AmpersandQuote = "@" + Quote;
        private const string Backslash = @"\";

        public static async Task<Document> ReplaceWithInterpolatedStringAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            int interpolationStartIndex = -1,
            int interpolationLength = 0,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            string s = literalExpression.Token.Text.ToString();

            if (interpolationStartIndex != -1)
            {
                s = EscapeBraces(s.Substring(0, interpolationStartIndex)) +
                   "{" +
                   s.Substring(interpolationStartIndex, interpolationLength) +
                   "}" +
                   EscapeBraces(s.Substring(interpolationStartIndex + interpolationLength));
            }

            var interpolatedString = (InterpolatedStringExpressionSyntax)ParseExpression("$" + s)
                .WithTriviaFrom(literalExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, interpolatedString);

            return document.WithSyntaxRoot(newRoot);
        }

        private static string EscapeBraces(string s)
        {
            return s
                .Replace("{", "{{")
                .Replace("}", "}}");
        }

        public static bool CanReplaceWithStringEmpty(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 0;
        }

        public static async Task<Document> ReplaceWithStringEmptyAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberAccessExpressionSyntax newNode = MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    PredefinedType(Token(SyntaxKind.StringKeyword)),
                    IdentifierName("Empty"))
                .WithTriviaFrom(literalExpression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceWithRegularStringLiteralAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            string s = CreateRegularStringLiteral(literalExpression.Token.ValueText);

            LiteralExpressionSyntax newNode = ParseRegularStringLiteral(s)
                .WithTriviaFrom(literalExpression);

            root = root.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> ReplaceWithRegularStringLiteralsAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            BinaryExpressionSyntax newNode = CreateAddExpression(literalExpression.Token.ValueText)
                .WithTriviaFrom(literalExpression)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> ReplaceWithVerbatimStringLiteralAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            string s = literalExpression.Token.ValueText;

            s = s.Replace(Quote, Quote + Quote);

            var newNode = (LiteralExpressionSyntax)ParseExpression(AmpersandQuote + s + Quote)
                .WithTriviaFrom(literalExpression);

            root = root.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static string CreateRegularStringLiteral(string text)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '"':
                        sb.Append(Backslash + Quote);
                        break;
                    case '\\':
                        sb.Append(Backslash + Backslash);
                        break;
                    case '\r':
                        sb.Append(@"\r");
                        break;
                    case '\n':
                        sb.Append(@"\n");
                        break;
                    default:
                        sb.Append(text[i]);
                        break;
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
            return BinaryExpression(
                SyntaxKind.AddExpression,
                left,
                Token(SyntaxTriviaList.Empty, SyntaxKind.PlusToken, ParseTrailingTrivia("\r\n")),
                right);
        }

        private static LiteralExpressionSyntax ParseRegularStringLiteral(string text)
        {
            return (LiteralExpressionSyntax)ParseExpression(Quote + text + Quote);
        }

        public static bool CanReplaceWithCharacterLiteral(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 1;
        }

        public static async Task<Document> ReplaceWithCharacterLiteralAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var charLiteralExpression = (LiteralExpressionSyntax)ParseExpression($"'{GetCharLiteralText(literalExpression)}'")
                .WithTriviaFrom(literalExpression);

            root = root.ReplaceNode(literalExpression, charLiteralExpression);

            return document.WithSyntaxRoot(root);
        }

        private static string GetCharLiteralText(LiteralExpressionSyntax literalExpression)
        {
            string s = literalExpression.Token.ValueText;

            switch (s[0])
            {
                case '\'':
                    return @"\'";
                case '\"':
                    return @"\""";
                case '\\':
                    return @"\\";
                case '\0':
                    return @"\0";
                case '\a':
                    return @"\a";
                case '\b':
                    return @"\b";
                case '\f':
                    return @"\f";
                case '\n':
                    return @"\n";
                case '\r':
                    return @"\r";
                case '\t':
                    return @"\t";
                case '\v':
                    return @"\v";
                default:
                    return s;
            }
        }
    }
}
