// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ConvertStringLiteralToCharacterLiteralRefactoring
    {
        public static bool CanRefactor(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 1;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

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
