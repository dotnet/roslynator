// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class StringLiteralRefactoring
    {
        private const string Quote = "\"";
        private const string AmpersandQuote = "@" + Quote;
        private const string Backslash = @"\";

        public static void ComputeRefactorings(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            context.RegisterRefactoring(
                "Convert to interpolated string",
                cancellationToken =>
                {
                    int startIndex = -1;
                    int length = 0;

                    if (context.Span.End < literalExpression.Span.End)
                    {
                        startIndex = GetInterpolationStartIndex(context.Span.Start, literalExpression);

                        if (startIndex != 1)
                            length = context.Span.Length;
                    }

                    return ConvertStringLiteralToInterpolatedStringAsync(
                        context.Document,
                        literalExpression,
                        startIndex,
                        length,
                        cancellationToken);
                });

            if (literalExpression.Span.Equals(context.Span))
            {
                if (literalExpression.IsVerbatimStringLiteral())
                {
                    context.RegisterRefactoring(
                        "Convert to regular string literal",
                        cancellationToken =>
                        {
                            return ConvertToRegularStringLiteralAsync(
                                context.Document,
                                literalExpression,
                                cancellationToken);
                        });

                    if (literalExpression.Token.ValueText.Contains("\n"))
                    {
                        context.RegisterRefactoring(
                            "Convert to regular string literals",
                            cancellationToken =>
                            {
                                return ConvertToRegularStringLiteralsAsync(
                                    context.Document,
                                    literalExpression,
                                    cancellationToken);
                            });
                    }
                }
                else
                {
                    context.RegisterRefactoring(
                        "Convert to verbatim string literal",
                        cancellationToken =>
                        {
                            return ConvertToVerbatimStringLiteralAsync(
                                context.Document,
                                literalExpression,
                                cancellationToken);
                        });
                }
            }

            if (CanConvertStringLiteralToStringEmpty(literalExpression))
            {
                context.RegisterRefactoring(
                    "Convert to string.Empty",
                    cancellationToken =>
                    {
                        return ConvertStringLiteralToStringEmptyAsync(
                            context.Document,
                            literalExpression,
                            cancellationToken);
                    });
            }

            if (ConvertStringLiteralToCharacterLiteralRefactoring.CanRefactor(literalExpression))
            {
                context.RegisterRefactoring(
                    "Convert to character literal",
                    cancellationToken =>
                    {
                        return ConvertStringLiteralToCharacterLiteralRefactoring.RefactorAsync(
                            context.Document,
                            literalExpression,
                            cancellationToken);
                    });
            }
        }

        private static int GetInterpolationStartIndex(int spanStartIndex, LiteralExpressionSyntax literalExpression)
        {
            string s = literalExpression.Token.Text;

            int index = spanStartIndex - literalExpression.Span.Start;

            if (s.StartsWith("@", StringComparison.Ordinal))
            {
                if (index > 1)
                    return index;
            }
            else if (index > 0)
            {
                return index;
            }

            return -1;
        }

        public static bool CanConvertStringLiteralToStringEmpty(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 0;
        }

        public static async Task<Document> ConvertStringLiteralToStringEmptyAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            MemberAccessExpressionSyntax newNode = MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    PredefinedType(Token(SyntaxKind.StringKeyword)),
                    IdentifierName("Empty"))
                .WithTriviaFrom(literalExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ConvertStringLiteralToInterpolatedStringAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            int interpolationStartIndex = -1,
            int interpolationLength = 0,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            string s = literalExpression.Token.Text.ToString();

            if (interpolationStartIndex != -1)
            {
                s = s.Substring(0, interpolationStartIndex) +
                   "{" +
                   s.Substring(interpolationStartIndex, interpolationLength) +
                   "}" +
                   s.Substring(interpolationStartIndex + interpolationLength);
            }

            var interpolatedString = (InterpolatedStringExpressionSyntax)ParseExpression("$" + s)
                .WithTriviaFrom(literalExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, interpolatedString);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ConvertToRegularStringLiteralAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            string s = CreateRegularStringLiteral(literalExpression.Token.ValueText);

            LiteralExpressionSyntax newNode = ParseRegularStringLiteral(s)
                .WithTriviaFrom(literalExpression);

            root = root.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> ConvertToRegularStringLiteralsAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            BinaryExpressionSyntax newNode = CreateAddExpression(literalExpression.Token.ValueText)
                .WithTriviaFrom(literalExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> ConvertToVerbatimStringLiteralAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

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
    }
}
